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
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Net;

using Microsoft.Extensions.Logging;
using Microsoft.IO;

using NKafka.Config;
using NKafka.Exceptions;

namespace NKafka.Connection;

internal partial class KafkaConnectorPool: IKafkaConnectorPool
{
    private readonly bool _apiVersionRequest;
    private readonly ConcurrentDictionary<int, Node> _brokers = new(); //список всех брокеров по их id

    private readonly ConcurrentDictionary<Node, List<IKafkaConnector>> _brokersConnectors = new(); //Список всех соединений с брокерами
    private readonly string _clientId;
    private readonly int _closeConnectionTimeoutMs;
    private readonly int _connectionsMaxIdleMs;
    private readonly ILogger<KafkaConnectorPool> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly int _maxInflightRequests;
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;
    private readonly int _messageMaxBytes;
    private readonly Random _random = new();
    private readonly int _receiveBufferBytes;
    private readonly int _requestTimeoutMs;
    private readonly SaslSettings _saslSettings;
    private readonly SecurityProtocols _securityProtocol;

    //Посевные соединения, используются только на начальном этапе жизни пула. Во время работы они очищаются
    private readonly ConcurrentDictionary<EndPoint, IKafkaConnector> _seedConnectors;
    private readonly SocketFactory _socketFactory;
    private readonly SslSettings _sslSettings;

    public KafkaConnectorPool(
        IReadOnlyCollection<Node> seedBrokers,
        SslSettings sslSettings,
        SaslSettings saslSettings,
        int maxInflightRequests,
        int messageMaxBytes,
        int closeConnectionTimeoutMs,
        int connectionsMaxIdleMs,
        int requestTimeoutMs,
        int receiveBufferBytes,
        SecurityProtocols securityProtocol,
        string clientId,
        bool apiVersionRequest,
        ILoggerFactory loggerFactory)
    {
        _sslSettings = sslSettings;
        _saslSettings = saslSettings;
        _maxInflightRequests = maxInflightRequests;
        _messageMaxBytes = messageMaxBytes;
        _closeConnectionTimeoutMs = closeConnectionTimeoutMs;
        _connectionsMaxIdleMs = connectionsMaxIdleMs;
        _requestTimeoutMs = requestTimeoutMs;
        _receiveBufferBytes = receiveBufferBytes;
        _securityProtocol = securityProtocol;
        _clientId = clientId;
        _apiVersionRequest = apiVersionRequest;
        _loggerFactory = loggerFactory;
        _seedConnectors = new ConcurrentDictionary<EndPoint, IKafkaConnector>();
        _logger = loggerFactory.CreateLogger<KafkaConnectorPool>();
        _socketFactory = new SocketFactory();
        _memoryStreamManager = new RecyclableMemoryStreamManager(1024, messageMaxBytes / 16, messageMaxBytes);

        InitSeedConnectors(seedBrokers);
    }

    public bool TryGetConnector(int nodeId, out IKafkaConnector connector)
    {
        if (_brokers.TryGetValue(nodeId, out var node))
        {
            if (_brokersConnectors.TryGetValue(node, out var connectors))
            {
                connector = connectors[0]; //todo пока берем только первый, потом надо разобраться какой точно нужно взять

                return true;
            }
        }

        connector = null!;

        return false;
    }

    public IKafkaConnector GetRandomConnector()
    {
        if (_brokers.IsEmpty)
        {
            if (_seedConnectors.IsEmpty)
            {
                throw new ConnectorNotFoundException("Пул не содержит ни одного доступного соединения");
            }

            //todo allocation но всего лишь один раз за работу
            return _seedConnectors.Values.AsEnumerable().Shuffle().First();
        }

        var index = _random.Next(_brokers.Count);
        var broker = ((ReadOnlyCollection<int>)_brokers.Keys)[index];

        return _brokersConnectors[_brokers[broker]].First();
    }

    public async ValueTask AddOrUpdateConnectorsAsync(IEnumerable<Node> nodes, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        foreach (var node in nodes)
        {
            if (!_brokers.TryAdd(node.Id, node))
            {
                continue;
            }

            var connectors = _brokersConnectors.AddOrUpdate(node, AddNewConnector, UpdateConnectors);

            var deadConnections = new List<IKafkaConnector>();

            foreach (var connector in connectors)
            {
                if (connector.ConnectorState != KafkaConnector.State.Open)
                {
                    try
                    {
                        await connector.OpenAsync(token).ConfigureAwait(false);
                    }
                    catch
                    {
                        deadConnections.Add(connector);
                    }
                }
            }

            foreach (var deadConnection in deadConnections)
            {
                deadConnection.Dispose();
                var index = _brokersConnectors[node].IndexOf(deadConnection);
                _brokersConnectors[node].RemoveAt(index);

                if (_brokersConnectors[node].Count == 0)
                {
                    _brokersConnectors.TryRemove(node, out _);
                }
            }
        }
    }

    public void Dispose()
    {
        foreach (var broker in _brokersConnectors)
        {
            foreach (var connector in broker.Value)
            {
                connector.Dispose();
            }
        }

        _brokersConnectors.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var broker in _brokersConnectors)
        {
            foreach (var connector in broker.Value)
            {
                await connector.DisposeAsync();
            }
        }

        _brokersConnectors.Clear();
    }

    private void InitSeedConnectors(IEnumerable<Node> seedBrokers)
    {
        foreach (var broker in seedBrokers)
        {
            var connector = CreateConnector(broker);
            _seedConnectors.TryAdd(broker.EndPoint, connector);
        }
    }

    private IKafkaConnector CreateConnector(Node node)
    {
        _logger.CreateConnectorTrace(node.EndPoint);

        return new KafkaConnector(
            node.EndPoint,
            _maxInflightRequests,
            _messageMaxBytes,
            _closeConnectionTimeoutMs,
            _connectionsMaxIdleMs,
            _requestTimeoutMs,
            _receiveBufferBytes,
            _securityProtocol,
            _saslSettings,
            _sslSettings,
            _clientId,
            _apiVersionRequest,
            _socketFactory,
            _memoryStreamManager,
            _loggerFactory)
        {
            NodeId = node.Id
        };
    }

    private List<IKafkaConnector> UpdateConnectors(Node node, List<IKafkaConnector> connectors)
    {
        if (connectors.Any(c => c.Endpoint == node.EndPoint))
        {
            return connectors;
        }

        if (!_seedConnectors.TryRemove(node.EndPoint, out var connector))
        {
            connector = CreateConnector(node);
        }
        else
        {
            connector.NodeId = node.Id;
        }

        connectors.Add(connector);

        return connectors;
    }

    private List<IKafkaConnector> AddNewConnector(Node node)
    {
        return new List<IKafkaConnector>
        {
            CreateConnector(node)
        };
    }
}