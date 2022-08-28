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

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
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
internal sealed class KafkaConnector: IKafkaConnector
{
    private readonly int _maxInflightRequests;
    private readonly int _messageMaxBytes;
    private readonly int _closeConnectionTimeoutMs;
    private readonly int _connectionsMaxIdleMs;
    private readonly SecurityProtocols _securityProtocols;
    private readonly ILogger<KafkaConnector> _logger;

    private readonly ConcurrentDictionary<int, Task> _inFlightRequests;

    private readonly Socket _socket = default!;
    private Stream _stream = default!;

    private readonly CancellationTokenSource _responseProcessingTokenSource = new();

    /*
 * Когда _globalTimeWaiting.ElapsedMiliseconds превысит параметр ConnectionsMaxIdleMs из конфигурации,
 * соединение с брокером автоматически закроется 
 */
    private readonly Stopwatch _globalTimeWaiting = new();
    private readonly Timer _closeConnectionAfterTimeout;

    public State ConnectorState { get; private set; } = State.Closed;

    /// <summary>
    /// The current number of running inflight requests
    /// </summary>
    public int CurrentNumberInflightRequests => _inFlightRequests.Count;

    public KafkaConnector(
        int maxInflightRequests,
        int messageMaxBytes,
        int closeConnectionTimeoutMs,
        int connectionsMaxIdleMs,
        SecurityProtocols securityProtocols,
        ILoggerFactory loggerFactory)
    {
        _maxInflightRequests = maxInflightRequests;
        _messageMaxBytes = messageMaxBytes;
        _closeConnectionTimeoutMs = closeConnectionTimeoutMs;
        _connectionsMaxIdleMs = connectionsMaxIdleMs;
        _securityProtocols = securityProtocols;
        _inFlightRequests = new ConcurrentDictionary<int, Task>(Environment.ProcessorCount, _maxInflightRequests);
        _logger = loggerFactory.CreateLogger<KafkaConnector>();

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

    //     private readonly ArrayPool<byte> _arrayPool;
//     private readonly RecyclableMemoryStreamManager _streamManager = new();
//     private readonly CancellationTokenSource _responseProcessingTokenSource = new();
//     private readonly Socket _socket;
//     private readonly ConcurrentDictionary<int, Broker.ResponseTaskCompletionSource> _tckPool;
//     
//     private readonly ConcurrentDictionary<int, Task> _inFlightRequests;
//     
//     public NetworkKafkaClient(
//         int maxInFlightRequests,
//         ILogger<NetworkKafkaClient> logger,
//         ActivitySource activitySource,
//         Meter meter)
//     {
//         _inFlightRequests = new ConcurrentDictionary<int, Task>(maxInFlightRequests, maxInFlightRequests);
//     }
//
//     
//

    Task<TResponseMessage> IKafkaConnector.SendAsync<TResponseMessage, TRequestMessage>(
        TRequestMessage message,
        bool isInternalRequest,
        CancellationToken token)
    {
        return Task.FromResult<TResponseMessage>(default);

        // if (_inFlightRequests.Count >= _maxInflightRequests)
        // {
        //     throw new ProtocolKafkaException(
        //         ErrorCodes.None,
        //         $"Количество ожидающих ответов запросов к брокеру превысило ограничение в '{_maxInflightRequests}' единиц");
        // }
        //
        // _globalTimeWaiting.Restart(); //Каждый новый запрос перезапускает таймер
        //
        // var requestId = Interlocked.Increment(ref _requestId);
        //
        // using var activity = KafkaDiagnosticsSource.InternalSendMessage(content.ApiKey, content.Version, requestId, Id, EndPoint);
        //
        // var request = new SendMessage(
        //     new RequestHeader
        //     {
        //         ClientId = _config.ClientId,
        //         RequestApiVersion = (short)content.Version,
        //         CorrelationId = requestId,
        //         RequestApiKey = (short)content.ApiKey
        //     },
        //     content);
        //
        // ThrowExceptionIfRequestNotValid(request, activity);
        //
        // await ThrowIfBrokerDontSupportApiVersion(request);
        //
        // await ReEstablishConnectionAsync(token);
        //
        // if (token.IsCancellationRequested)
        // {
        //     return await Task.FromCanceled<TResponseMessage>(token);
        // }
        //
        // using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        //
        // var taskCompletionSource = new ResponseTaskCompletionSource(
        //     (ApiKeys)request.Header.RequestApiKey,
        //     (ApiVersions)request.Header.RequestApiVersion);
        //
        // token.Register(
        //     state =>
        //     {
        //         var awaiter = state as ResponseTaskCompletionSource;
        //         awaiter?.TrySetCanceled();
        //     },
        //     taskCompletionSource,
        //     false);
        //
        // try
        // {
        //     _tckPool.TryAdd(requestId, taskCompletionSource);
        //     WakeupProcessingResponses(); //"пробуждаем" обработку ответов на запрос
        //
        //     if (_stream.CanWrite)
        //     {
        //         request.Write(_stream);
        //     }
        // }
        // catch (Exception exc)
        // {
        //     activity?.SetStatus(ActivityStatusCode.Error, exc.Message);
        //
        //     _tckPool.TryRemove(requestId, out _); //Если не удалось отправить запрос, то удаляем сообщение
        //
        //     if (!taskCompletionSource.Task.IsCanceled || !taskCompletionSource.Task.IsCompleted)
        //     {
        //         taskCompletionSource.SetException(new ProtocolKafkaException(ErrorCodes.UnknownServerError, "Не удалось отправить запрос", exc));
        //     }
        // }
        //
        // return (TResponseMessage)await taskCompletionSource.Task;
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

    internal enum State
    {
        Open,
        Closing,
        Closed
    }
}