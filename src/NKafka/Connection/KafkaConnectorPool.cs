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

using EM = NKafka.Resources.ExceptionMessages;

namespace NKafka.Connection;

internal partial class KafkaConnectorPool: IKafkaConnectorPool
{
    private readonly bool _apiVersionRequest;
    private readonly ConcurrentDictionary<int, Node> _brokers = new(); // The dictionary of all broker nodes by their id
    private readonly ConcurrentDictionary<Node, List<IKafkaConnector>> _brokersConnectors = new(); // The dictionary of all connections with broker nodes
    private readonly string _clientId;
    private readonly int _closeConnectionTimeoutMs;
    private readonly int _connectionsMaxIdleMs;
    private readonly ILogger<KafkaConnectorPool> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly int _maxInflightRequests;
    private readonly RecyclableMemoryStreamManager _memoryStreamManager;
    private readonly int _messageMaxBytes;
    private readonly int _receiveBufferBytes;
    private readonly int _requestTimeoutMs;
    private readonly SaslSettings _saslSettings;
    private readonly SecurityProtocols _securityProtocol;

    // Seed connections are used only at the initial stage of the life of the pool. They are cleaned during operation.
    private readonly Dictionary<EndPoint, IKafkaConnector> _seedConnectors;
    private readonly SocketFactory _socketFactory;
    private readonly SslSettings _sslSettings;

    private readonly INumberCounter _seedConnectorsNumberCounter;
    private readonly INumberCounter _brokersNumberCounter;

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
        _seedConnectors = new Dictionary<EndPoint, IKafkaConnector>();
        _logger = loggerFactory.CreateLogger<KafkaConnectorPool>();
        _socketFactory = new SocketFactory();
        _memoryStreamManager = new RecyclableMemoryStreamManager(1024, messageMaxBytes / 16, messageMaxBytes);
        _seedConnectorsNumberCounter = new RoundRobinNumberCounter(seedBrokers.Count);
        _brokersNumberCounter = new RandomNumberCounter();

        InitSeedConnectors(seedBrokers);

    }

    //only for tests
    internal KafkaConnectorPool(
        IReadOnlyCollection<IKafkaConnector> brokerConnectors,
        IReadOnlyCollection<IKafkaConnector> seedConnectors,
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
        _seedConnectors = new Dictionary<EndPoint, IKafkaConnector>();
        _logger = loggerFactory.CreateLogger<KafkaConnectorPool>();
        _socketFactory = new SocketFactory();
        _memoryStreamManager = new RecyclableMemoryStreamManager(1024, messageMaxBytes / 16, messageMaxBytes);
        _brokersNumberCounter = new RandomNumberCounter();
        _seedConnectorsNumberCounter = new RoundRobinNumberCounter(seedConnectors.Count);

        foreach (var connector in seedConnectors)
        {
            if (_seedConnectors.TryAdd(connector.Endpoint, connector) is not true)
            {
                throw new ArgumentException("Коллекция не может содержать элементы, которые относятся к одному и тому же адресу", nameof(seedConnectors));
            }
        }

        // Для тестов сбрасываем внутреннее состояние пула до корректного.
        // Если есть готовые брокеры - посевные адреса брокеров больше не нужны 
        if (brokerConnectors.Count != 0)
        {
            _seedConnectors.Clear();
        }

        var connectorsByNodeId = brokerConnectors.GroupBy(c => c.NodeId);

        foreach (var groupByNodeId in connectorsByNodeId)
        {
            var first = groupByNodeId.First();
            var (host, port) = Utils.GetHostAndPort(first.Endpoint.ToString());
            var node = new Node(groupByNodeId.Key, host, port);
            _brokers.TryAdd(groupByNodeId.Key, node);

            foreach (var connector in groupByNodeId.Select(c => c))
            {
                _brokersConnectors.AddOrUpdate(node,
                    _ => new List<IKafkaConnector>
                    {
                        connector
                    },
                    (_, nodes) =>
                    {
                        nodes.Add(connector);

                        return nodes;
                    });
            }
        }

    }

    /// <summary>
    /// Возвращает все рабочие соединения
    /// </summary>
    public IEnumerable<IKafkaConnector> GetAllOpenedConnectors()
    {
        foreach (var connectors in _brokersConnectors.Values)
        {
            foreach (var connector in connectors)
            {
                if (connector.ConnectorState == KafkaConnector.State.Open)
                {
                    yield return connector;
                }
            }
        }
    }

    /// <inheritdoc />
    public bool TryGetConnector(int nodeId, bool isDedicated, out IKafkaConnector connector)
    {
        if (isDedicated)
        {
            return TryDedicateConnector(nodeId, out connector);
        }

        if (_brokers.TryGetValue(nodeId, out var node))
        {
            if (_brokersConnectors.TryGetValue(node, out var connectors))
            {
                connector = connectors.Count == 1 ? connectors[0] : TakeLeastLoaded(connectors);

                return true;
            }
        }

        connector = null!;

        return false;
    }

    private static IKafkaConnector TakeLeastLoaded(IReadOnlyList<IKafkaConnector> connectors)
    {
        if (connectors.Count == 0)
        {
            throw new ConnectionKafkaException("Отсутсвуют физические подключения");
        }

        var currentNumberInflightRequests = 0;
        var selectedIndex = 0;

        for (var i = 0; i < connectors.Count; i++)
        {
            var connector = connectors[i];

            if (connector.IsDedicated)
            {
                continue; //на выделенные соединения запросы делает только координатор
            }

            var inflightRequests = connector.CurrentNumberInflightRequests;

            if (inflightRequests == 0)
            {
                selectedIndex = i;

                break;
            }

            if (currentNumberInflightRequests > inflightRequests)
            {
                selectedIndex = i;
            }
            currentNumberInflightRequests = inflightRequests;
        }

        return connectors[selectedIndex];
    }

    private bool TryDedicateConnector(int nodeId, out IKafkaConnector connector)
    {
        if (_brokers.TryGetValue(nodeId, out var node))
        {
            connector = CreateConnector(node, true);

            return true;
        }
        connector = null!;

        return false;
    }

    public IKafkaConnector GetConnector()
    {
        if (_brokers.IsEmpty)
        {
            return GetSeedConnectorAsRoundRobin();
        }

        var node = GetBrokerAsRandom();
        var listConnectors = _brokersConnectors[node];

        return TakeLeastLoaded(listConnectors);
    }

    private Node GetBrokerAsRandom()
    {
        var index = _brokersNumberCounter.GetNextNumber(_brokers.Count);
        var broker = ((ReadOnlyCollection<int>)_brokers.Keys)[index];

        return _brokers[broker];
    }

    private IKafkaConnector GetSeedConnectorAsRoundRobin()
    {
        if (_seedConnectors.Count == 0)
        {
            throw new ConnectorNotFoundException(EM.ConnectorPool_NoAvailableConnections);
        }

        var index = _seedConnectorsNumberCounter.GetNextNumber();

        return _seedConnectors.ToArray()[index].Value;
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
                if (connector.ConnectorState == KafkaConnector.State.Open)
                {
                    continue;
                }

                try
                {
                    await connector.OpenAsync(token);
                }
                catch
                {
                    deadConnections.Add(connector);
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
            if (_seedConnectors.ContainsKey(broker.EndPoint))
            {
                _logger.IgnoreBootstrapEndpointWarning(broker.EndPoint);

                continue;
            }
            var connector = CreateConnector(broker);
            _seedConnectors.Add(broker.EndPoint, connector);
        }
    }

    private KafkaConnector CreateConnector(Node node, bool isDedicated = false)
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
            NodeId = node.Id,
            IsDedicated = isDedicated
        };
    }

    private List<IKafkaConnector> UpdateConnectors(Node node, List<IKafkaConnector> connectors)
    {
        if (connectors.Any(c => c.Endpoint == node.EndPoint))
        {
            return connectors;
        }

        //todo: Нужно на 100% исключить вероятность доступа к переменной _seedConnectors из разных потоков 
        if (_seedConnectors.Count == 0 || !_seedConnectors.Remove(node.EndPoint, out var connector))
        {
            connector = CreateConnector(node: node, isDedicated: false);
        }
        else
        {
            connector.NodeId = node.Id; // Теоретически id брокера на конкретном адресе может измениться
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