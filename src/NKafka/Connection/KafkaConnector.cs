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
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Extensions.Logging;
using Microsoft.IO;

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
    private readonly bool _apiVersionRequest;
    private readonly ArrayPool<byte> _arrayPool;
    private readonly string _clientId;

    // ReSharper disable once NotAccessedField.Local
#pragma warning disable IDE0052

    //Нужно убрать этот механизм в KafkaConnectorPool
    private readonly Timer _closeConnectionAfterTimeout; //todo Для каждого соединения будет кушаться поток из пула.

#pragma warning restore IDE0052
    private readonly int _closeConnectionTimeoutMs;
    private readonly int _connectionsMaxIdleMs;

    /*
     * Когда _globalTimeWaiting.ElapsedMiliseconds превысит параметр ConnectionsMaxIdleMs из конфигурации,
     * соединение с брокером автоматически закроется 
     */
    private readonly Stopwatch _globalTimeWaiting = new();

    private readonly ConcurrentDictionary<int, ResponseTaskCompletionSource> _inFlightRequests;
    private readonly ILogger<KafkaConnector> _logger;
    private readonly int _maxInflightRequests;
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;
    private readonly int _messageMaxBytes;
    private readonly int _requestTimeoutMs;

    private readonly CancellationTokenSource _responseProcessingTokenSource = new();
    private readonly ConcurrentDictionary<int, Task> _responsesTasks = new();
    private readonly SaslSettings _saslSettings;
    private readonly SecurityProtocols _securityProtocol;
    private readonly ISocketFactory _socketFactory;

    private readonly ISocketProxy _socketProxy = default!;
    private readonly SslSettings _sslSettings;

    // todo со временем нужно убрать обработку сообщений из коннектора в KafkaConnectorPool
    // так будет тратиться меньше ресурсов ThreadPool, а сама обработка будет максимально утилизировать работу потоков
    private Task _processData = Task.CompletedTask;
    private volatile int _requestId = -1;
    private Stream _stream = Stream.Null;
    private Dictionary<ApiKeys, (short, short)> _supportVersions = new(0);

    /// <summary>
    /// Можно ли писать в текущий поток?
    /// </summary>
    private bool CanWrite => _stream != Stream.Null && _stream.CanWrite;

    internal KafkaConnector(
        EndPoint endPoint,
        int maxInflightRequests,
        int messageMaxBytes,
        int closeConnectionTimeoutMs,
        int connectionsMaxIdleMs,
        int requestTimeoutMs,
        int receiveBufferBytes,
        SecurityProtocols securityProtocol,
        SaslSettings saslSettings,
        SslSettings sslSettings,
        string clientId,
        bool apiVersionRequest,
        ISocketFactory socketFactory,
        RecyclableMemoryStreamManager memoryStreamManager,
        ILoggerFactory loggerFactory)
    {
        Endpoint = endPoint;
        _maxInflightRequests = maxInflightRequests;
        _messageMaxBytes = messageMaxBytes;
        _closeConnectionTimeoutMs = closeConnectionTimeoutMs;
        _connectionsMaxIdleMs = connectionsMaxIdleMs;
        _requestTimeoutMs = requestTimeoutMs;
        _securityProtocol = securityProtocol;
        _saslSettings = saslSettings;
        _sslSettings = sslSettings;
        _clientId = clientId;
        _apiVersionRequest = apiVersionRequest;
        _socketFactory = socketFactory;
        _memoryStreamManager = memoryStreamManager;

        _arrayPool = ArrayPool<byte>.Create(messageMaxBytes, maxInflightRequests);
        _inFlightRequests = new ConcurrentDictionary<int, ResponseTaskCompletionSource>(Environment.ProcessorCount, _maxInflightRequests);
        _logger = loggerFactory.CreateLogger<KafkaConnector>();

        _socketProxy = _socketFactory.CreateSocket(SocketType.Stream, ProtocolType.Tcp, receiveBufferBytes);

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

    /// <inheritdoc/>
    public int NodeId { get; set; } = Node.NoNode.Id;

    /// <inheritdoc/>
    public State ConnectorState { get; private set; } = State.Closed;

    /// <inheritdoc/>
    public int CurrentNumberInflightRequests => _inFlightRequests.Count;

    /// <inheritdoc/>
    public EndPoint Endpoint { get; }

    public async ValueTask OpenAsync(CancellationToken token)
    {
        await ReEstablishConnectionAsync(token);
        await TryRequestApiSupportVersionsAsync(token);

        ConnectorState = State.Open;
    }

    async Task<TResponseMessage> IKafkaConnector.SendAsync<TResponseMessage, TRequestMessage>(
        TRequestMessage message,
        bool isInternalRequest,
        CancellationToken token)
    {
        _logger.LogTrace("Send {Request} to {Brocker}", message, NodeId);

        if (_inFlightRequests.Count >= _maxInflightRequests)
        {
            throw new ProtocolKafkaException(
                ErrorCodes.None,
                $"Количество ожидающих ответов запросов к брокеру превысило ограничение в '{_maxInflightRequests}' единиц");
        }

        _globalTimeWaiting.Restart(); //Каждый новый запрос перезапускает таймер

        var contentVersion = message.ApiKey.GetSupportApiVersion(_supportVersions);
        var headerVersion = message.ApiKey.GetRequestHeaderVersion(contentVersion);
        var requestId = Interlocked.Increment(ref _requestId);

        using var activity = KafkaDiagnosticsSource.InternalSendMessage(message.ApiKey, contentVersion, requestId, NodeId, Endpoint);

        var request = new SendMessage(
            new RequestHeader
            {
                ClientId = _clientId,
                RequestApiVersion = (short)contentVersion,
                CorrelationId = requestId,
                RequestApiKey = (short)message.ApiKey
            },
            message,
            contentVersion,
            headerVersion,
            _memoryStreamManager);

        if (!isInternalRequest)
        {
            ThrowExceptionIfRequestNotValid(request, activity);
        }

        if (message is not ApiVersionsRequestMessage)
        {
            await ReEstablishConnectionAsync(token);
        }

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

            if (CanWrite)
            {
                request.Write(_stream, true, _messageMaxBytes);
            }
            else
            {
                throw new ConnectionKafkaException($"Текущее соединение по адресу {Endpoint} к брокеру {NodeId} не может отправлять запросы");
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

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous dispose operation.
    /// </returns>
    public async ValueTask DisposeAsync()
    {
        await _stream.DisposeAsync();
        _socketProxy.Dispose();
    }

    private void ResetConnection()
    {
        _logger.ConnectionReset(Endpoint, NodeId, _connectionsMaxIdleMs);

        ConnectorState = State.Closing;
        _stream.Dispose();
        _socketProxy.Close(_closeConnectionTimeoutMs);
        ConnectorState = State.Closed;
    }

    private void WakeupProcessingResponses()
    {
        if (_processData.Status == TaskStatus.RanToCompletion)
        {
            _processData = ResponseReaderTask();
        }
    }

    private async Task ReEstablishConnectionAsync(CancellationToken token)
    {
        try
        {
            if (!_socketProxy.Connected && !token.IsCancellationRequested && ConnectorState == State.Closed)
            {
                await _socketProxy.ConnectAsync(Endpoint, token);
                var stream = _socketFactory.CreateNetworkStream(_socketProxy.Socket, true);

                if (_securityProtocol is SecurityProtocols.Ssl or SecurityProtocols.SaslSsl)
                {
                    _stream = _socketFactory.CreateSslStream(stream);
                    await ((SslStream)_stream).AuthenticateAsClientAsync("", null, _sslSettings.Protocols, _sslSettings.CheckCertificateRevocation);
                }
                else
                {
                    _stream = stream;
                }

                _globalTimeWaiting.Start();

                if (_securityProtocol is SecurityProtocols.SaslPlaintext or SecurityProtocols.SaslSsl)
                {
                    await AuthenticateProcessAsync(token);
                }

                ConnectorState = State.Open;
            }
        }
        catch (AuthenticationException exc)
        {
            Debug.WriteLine(exc.Message);

            throw;
        }
        catch (SocketException exc)
        {
            Debug.WriteLine(exc.Message);

            throw;
        }
        catch (Exception exc)
        {
            Debug.WriteLine(exc.Message);

            throw;
        }
    }

    private async Task TryRequestApiSupportVersionsAsync(CancellationToken token)
    {
        if (_supportVersions.Count != 0 || !_apiVersionRequest)
        {
            return;
        }

        var request = new ApiVersionsRequestMessage();
        var response = await ((IKafkaConnector)this).SendAsync<ApiVersionsResponseMessage, ApiVersionsRequestMessage>(
            request,
            true,
            token);

        ((IResponseMessage)response).ThrowIfError();

        if (response is { Code: ErrorCodes.None, ApiKeys.Count: 0 })
        {
            _supportVersions = SupportVersionsExtensions.Default;
        }
        else
        {
            var supportVersions = new Dictionary<ApiKeys, (short, short)>(response.ApiKeys.Count);

            foreach (var apiKey in response.ApiKeys)
            {
                supportVersions.Add((ApiKeys)apiKey.ApiKey, (apiKey.MinVersion, apiKey.MaxVersion));
            }

            _supportVersions = supportVersions;
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

    private void ReleaseUnmanagedResources()
    {
        _stream.Dispose();
        _socketProxy.Dispose();
    }

    private void Dispose(bool disposing)
    {
        ConnectorState = State.Closing;
        ReleaseUnmanagedResources();

        if (disposing)
        {
        }

        ConnectorState = State.Closed;
    }

    /// <summary>
    /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~KafkaConnector()
    {
        Dispose(false);
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