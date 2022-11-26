//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

using Microsoft.Extensions.Logging;

using NKafka.Config;
using NKafka.Diagnostics;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Protocol;

namespace NKafka.Connection;

/// <summary>
/// 
/// </summary>
internal sealed partial class KafkaConnector: IKafkaConnector
{
    private readonly ArrayPool<byte> _arrayPool;
    private readonly ConcurrentDictionary<int, Task> _responsesTasks = new();
    private readonly EndPoint _endPoint;
    private readonly int _maxInflightRequests;
    private readonly int _messageMaxBytes;
    private readonly int _closeConnectionTimeoutMs;
    private readonly int _connectionsMaxIdleMs;
    private readonly int _requestTimeoutMs;
    private readonly SecurityProtocols _securityProtocol;
    private readonly SaslSettings _saslSettings;
    private readonly SslSettings _sslSettings;
    private readonly string _clientId;
    private readonly ILogger<KafkaConnector> _logger;
    private volatile int _requestId = -1;

    private readonly ConcurrentDictionary<int, ResponseTaskCompletionSource> _inFlightRequests;

    private readonly Socket _socket = default!;
    private Stream _stream = default!;

    private readonly CancellationTokenSource _responseProcessingTokenSource = new();

    private Task _processData = Task.CompletedTask;

    /*
     * Когда _globalTimeWaiting.ElapsedMiliseconds превысит параметр ConnectionsMaxIdleMs из конфигурации,
     * соединение с брокером автоматически закроется 
     */
    private readonly Stopwatch _globalTimeWaiting = new();

    // ReSharper disable once NotAccessedField.Local
    private readonly Timer _closeConnectionAfterTimeout;

    public State ConnectorState { get; private set; } = State.Closed;

    /// <summary>
    /// The current number of running inflight requests
    /// </summary>
    public int CurrentNumberInflightRequests => _inFlightRequests.Count;

    public KafkaConnector(
        EndPoint endPoint,
        int maxInflightRequests,
        int messageMaxBytes,
        int closeConnectionTimeoutMs,
        int connectionsMaxIdleMs,
        int requestTimeoutMs,
        SecurityProtocols securityProtocol,
        SaslSettings saslSettings,
        SslSettings sslSettings,
        string clientId,
        ILoggerFactory loggerFactory)
    {
        _endPoint = endPoint;
        _maxInflightRequests = maxInflightRequests;
        _messageMaxBytes = messageMaxBytes;
        _closeConnectionTimeoutMs = closeConnectionTimeoutMs;
        _connectionsMaxIdleMs = connectionsMaxIdleMs;
        _requestTimeoutMs = requestTimeoutMs;
        _securityProtocol = securityProtocol;
        _saslSettings = saslSettings;
        _sslSettings = sslSettings;
        _clientId = clientId;

        _arrayPool = ArrayPool<byte>.Create(messageMaxBytes, maxInflightRequests);
        _inFlightRequests = new ConcurrentDictionary<int, ResponseTaskCompletionSource>(Environment.ProcessorCount, _maxInflightRequests);
        _logger = loggerFactory.CreateLogger<KafkaConnector>();

        _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        _closeConnectionAfterTimeout = new Timer(
            _ =>
            {
                if (!_globalTimeWaiting.IsRunning || _globalTimeWaiting.ElapsedMilliseconds <= _connectionsMaxIdleMs)
                {
                    return;
                }

                ResetConnection();
                _globalTimeWaiting.Reset();
            });
    }

    private void ResetConnection()
    {
        _stream.Dispose();
        _socket.Close(_closeConnectionTimeoutMs);
    }

    public ValueTask OpenAsync(CancellationToken token)
    {
        return default;
    }

    async Task<TResponseMessage> IKafkaConnector.SendAsync<TResponseMessage, TRequestMessage>(
        TRequestMessage message,
        bool isInternalRequest,
        CancellationToken token)
    {
        if (_inFlightRequests.Count >= _maxInflightRequests)
        {
            throw new ProtocolKafkaException(
                ErrorCodes.None,
                $"Количество ожидающих ответов запросов к брокеру превысило ограничение в '{_maxInflightRequests}' единиц");
        }

        _globalTimeWaiting.Restart(); //Каждый новый запрос перезапускает таймер

        var contentVersion = ApiVersion.Version0; //content.ApiKey.GetApiVersion(_supportVersions);
        var headerVersion = message.ApiKey.GetHeaderVersion(contentVersion);
        var requestId = Interlocked.Increment(ref _requestId);

        using var activity = KafkaDiagnosticsSource.InternalSendMessage(message.ApiKey, contentVersion, requestId, -1, _endPoint);

        var request = new SendMessage(
            new RequestHeader
            {
                ClientId = _clientId,
                RequestApiVersion = (short)contentVersion,
                CorrelationId = requestId,
                RequestApiKey = (short)message.ApiKey
            },
            message,
            contentVersion);

        if (!isInternalRequest)
        {
            ThrowExceptionIfRequestNotValid(request, activity);
        }

        ThrowIfBrokerDontSupportApiVersion(request);

        await ReEstablishConnectionAsync(token);

        if (token.IsCancellationRequested)
        {
            return await Task.FromCanceled<TResponseMessage>(token);
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var taskCompletionSource = new ResponseTaskCompletionSource(
            (ApiKeys)request.Header.RequestApiKey,
            (ApiVersion)request.Header.RequestApiVersion);

        token.Register(
            state =>
            {
                var awaiter = state as ResponseTaskCompletionSource;
                awaiter?.TrySetCanceled();
            },
            taskCompletionSource,
            false);

        try
        {
            _inFlightRequests.TryAdd(requestId, taskCompletionSource);
            WakeupProcessingResponses(); //"пробуждаем" обработку ответов на запрос
            cts.CancelAfter(_requestTimeoutMs);

            if (_stream.CanWrite)
            {
                request.Write(_stream);
            }
        }
        catch (Exception exc)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exc.Message);

            _inFlightRequests.TryRemove(requestId, out _); //Если не удалось отправить запрос, то удаляем сообщение

            if (!taskCompletionSource.Task.IsCanceled || !taskCompletionSource.Task.IsCompleted)
            {
                taskCompletionSource.SetException(new ProtocolKafkaException(ErrorCodes.UnknownServerError, "Не удалось отправить запрос", exc));
            }
        }

        return (TResponseMessage)await taskCompletionSource.Task;
    }

    private void WakeupProcessingResponses()
    {
        if (_processData.Status == TaskStatus.RanToCompletion)
        {
            _processData = ResponseReaderTask();
        }
    }

    private async Task ReEstablishConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!_socket.Connected && !cancellationToken.IsCancellationRequested)
            {
                await _socket.ConnectAsync(_endPoint, cancellationToken);
                Stream stream = new NetworkStream(_socket, true);

                if (_securityProtocol is SecurityProtocols.Ssl or SecurityProtocols.SaslSsl)
                {
                    stream = new SslStream(stream, false, (_, _, _, _) => true); //skip server cert validation
                }

                _stream = stream;
                _globalTimeWaiting.Start();

                // if (_config.SecurityProtocol is SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl)
                // {
                //     await AuthenticateProcessAsync(cancellationToken);
                // }
            }
        }
        catch (SocketException exc)
        {
            Debug.WriteLine(exc.Message);

            throw;
        }
        catch (Exception exc)
        {
            Debug.WriteLine(exc.Message);
        }
    }

    private void ThrowExceptionIfRequestNotValid(SendMessage message, Activity? activity)
    {
        // if (message.RequestLength >= _config.MessageMaxBytes)
        // {
        //     var logMessage = $"Размер запроса превышает допустимый предел указанный в конфигурации {_config.MessageMaxBytes}";
        //     activity?.SetStatus(ActivityStatusCode.Error);
        //     activity?.AddTag("error.message", logMessage);
        //
        //     throw new ProtocolKafkaException(ErrorCodes.MessageTooLarge, logMessage);
        // }
    }

    private void ThrowIfBrokerDontSupportApiVersion(SendMessage message)
    {
    }

    private void ReleaseUnmanagedResources()
    {
        _stream.Dispose();
        _socket.Dispose();
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();

        if (disposing)
        {
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
    ~KafkaConnector()
    {
        Dispose(false);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        return default;
    }

    internal enum State
    {
        Open,
        Closing,
        Closed
    }

    private class ResponseTaskCompletionSource: TaskCompletionSource<IResponseMessage>
    {
        private readonly ApiKeys _apiKey;

        private readonly ApiVersion _version;

        public ResponseTaskCompletionSource(ApiKeys apiKey, ApiVersion version)
            : base(TaskCreationOptions.RunContinuationsAsynchronously)
        {
            _apiKey = apiKey;
            _version = version;
        }

        internal IResponseMessage BuildResponseMessage(byte[] span)
        {
            return ResponseBuilder.Build(_apiKey, _version, span);
        }
    }
}