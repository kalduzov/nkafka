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
using System.Net.Sockets;

using Microsoft.Extensions.Logging;

using NKafka.Config;

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
        return null;
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