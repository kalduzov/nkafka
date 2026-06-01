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
using System.Net;

using Microsoft.Extensions.Logging;

using NKafka.Config;
using NKafka.Exceptions;

namespace NKafka.Connection;

internal partial class KafkaConnectorPool: IKafkaConnectorPool
{
    private readonly bool _apiVersionRequest;
    private readonly ConcurrentDictionary<int, Node> _brokerNodesById = new();
    private readonly ConcurrentDictionary<int, List<IKafkaConnector>> _sharedConnectorsByNodeId = new();
    private readonly ConcurrentDictionary<int, ConcurrentBag<IKafkaConnector>> _dedicatedConnectorsByNodeId = new();
    private readonly string _clientId;
    private readonly int _closeConnectionTimeoutMs;
    private readonly int _connectionsMaxIdleMs;
    private readonly ILogger<KafkaConnectorPool> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly int _maxInflightRequests;
    private readonly int _messageMaxBytes;
    private readonly int _receiveBufferBytes;
    private readonly int _requestTimeoutMs;
    private readonly SaslSettings _saslSettings;
    private readonly SecurityProtocols _securityProtocol;

    // Bootstrap connectors only exist to discover the first usable topology and can later
    // be promoted into the shared broker registry when metadata confirms the endpoint.
    private readonly ConcurrentDictionary<EndPoint, IKafkaConnector> _seedConnectors;
    private readonly SocketFactory _socketFactory;
    private readonly SslSettings _sslSettings;

    private readonly RoundRobinNumberCounter _seedConnectorsNumberCounter;
    private readonly RandomNumberCounter _brokersNumberCounter;

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
        _seedConnectorsNumberCounter = new RoundRobinNumberCounter(seedBrokers.Count);
        _brokersNumberCounter = new RandomNumberCounter();

        InitSeedConnectors(seedBrokers);
    }

    // only for tests
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
        _seedConnectors = new ConcurrentDictionary<EndPoint, IKafkaConnector>();
        _logger = loggerFactory.CreateLogger<KafkaConnectorPool>();
        _socketFactory = new SocketFactory();
        _brokersNumberCounter = new RandomNumberCounter();
        _seedConnectorsNumberCounter = new RoundRobinNumberCounter(seedConnectors.Count);

        foreach (var connector in seedConnectors)
        {
            if (!_seedConnectors.TryAdd(connector.Endpoint, connector))
            {
                throw new ArgumentException(
                    "Коллекция не может содержать элементы, которые относятся к одному и тому же адресу",
                    nameof(seedConnectors));
            }
        }

        var connectorsByNodeId = brokerConnectors.GroupBy(c => c.NodeId);

        foreach (var groupByNodeId in connectorsByNodeId)
        {
            var first = groupByNodeId.First();
            var (host, port) = Utils.GetHostAndPort(first.Endpoint.ToString()!);
            _brokerNodesById.TryAdd(groupByNodeId.Key, new Node(groupByNodeId.Key, host, port));
            _sharedConnectorsByNodeId.TryAdd(groupByNodeId.Key, groupByNodeId.ToList());
        }
    }

    public IEnumerable<IKafkaConnector> GetOpenedSharedConnectors()
    {
        foreach (var connectors in _sharedConnectorsByNodeId.Values)
        {
            foreach (var connector in connectors)
            {
                if (connector.IsDedicated || connector.ConnectorState != KafkaConnector.State.Open)
                {
                    continue;
                }

                yield return connector;
            }
        }
    }

    public bool TryGetSharedConnector(int nodeId, out IKafkaConnector connector)
    {
        if (_sharedConnectorsByNodeId.TryGetValue(nodeId, out var connectors)
            && TryTakeLeastLoadedShared(connectors, out connector))
        {
            return true;
        }

        connector = null!;

        return false;
    }

    public bool TryCreateDedicatedConnector(int nodeId, out IKafkaConnector connector)
    {
        return TryDedicateConnector(nodeId, out connector);
    }

    public bool TryGetAnySharedBrokerConnector(out IKafkaConnector connector)
    {
        if (_sharedConnectorsByNodeId.IsEmpty)
        {
            connector = null!;

            return false;
        }

        var node = GetBrokerAsRandom();
        var listConnectors = _sharedConnectorsByNodeId[node.Id];

        return TryTakeLeastLoadedShared(listConnectors, out connector);
    }

    public bool TryGetBootstrapConnector(out IKafkaConnector connector)
    {
        return TryGetSeedConnectorAsRoundRobin(out connector);
    }

    public async ValueTask AddOrUpdateConnectorsAsync(IEnumerable<Node> nodes, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var metadataNodes = nodes.ToArray();
        var metadataBrokerIds = metadataNodes.Select(static node => node.Id).ToHashSet();

        foreach (var node in metadataNodes)
        {
            RegisterBrokerNode(node);
            EnsureSharedConnectorsForNode(node);
            RemoveStaleSharedConnectors(node);
            await OpenSharedConnectorsAsync(node.Id, token);
            RemoveDeadSharedConnectors(node.Id);
        }

        RemoveSharedConnectorsMissingFromMetadata(metadataBrokerIds);
    }

    public void Dispose()
    {
        DisposeConnectors(_seedConnectors.Values);
        DisposeConnectors(_sharedConnectorsByNodeId.Values.SelectMany(static connectors => connectors));
        DisposeConnectors(_dedicatedConnectorsByNodeId.Values.SelectMany(static connectors => connectors));

        _seedConnectors.Clear();
        _sharedConnectorsByNodeId.Clear();
        _dedicatedConnectorsByNodeId.Clear();
        _brokerNodesById.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeConnectorsAsync(_seedConnectors.Values);
        await DisposeConnectorsAsync(_sharedConnectorsByNodeId.Values.SelectMany(static connectors => connectors));
        await DisposeConnectorsAsync(_dedicatedConnectorsByNodeId.Values.SelectMany(static connectors => connectors));

        _seedConnectors.Clear();
        _sharedConnectorsByNodeId.Clear();
        _dedicatedConnectorsByNodeId.Clear();
        _brokerNodesById.Clear();
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
                continue;
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

    private static bool TryTakeLeastLoadedShared(IReadOnlyList<IKafkaConnector> connectors, out IKafkaConnector connector)
    {
        var sharedConnectors = connectors.Where(c => !c.IsDedicated).ToArray();

        if (sharedConnectors.Length == 0)
        {
            connector = null!;

            return false;
        }

        connector = sharedConnectors.Length == 1 ? sharedConnectors[0] : TakeLeastLoaded(sharedConnectors);

        return true;
    }

    private bool TryDedicateConnector(int nodeId, out IKafkaConnector connector)
    {
        if (_brokerNodesById.TryGetValue(nodeId, out var node))
        {
            var dedicatedConnector = CreateConnector(node, true);
            connector = dedicatedConnector;

            // Dedicated connectors are created for a specific caller-owned workflow and are
            // tracked only so the pool can dispose them when the whole cluster is torn down.
            _dedicatedConnectorsByNodeId.AddOrUpdate(
                nodeId,
                _ => new ConcurrentBag<IKafkaConnector>([dedicatedConnector]),
                (_, dedicatedConnectors) =>
                {
                    dedicatedConnectors.Add(dedicatedConnector);

                    return dedicatedConnectors;
                });

            return true;
        }

        connector = null!;

        return false;
    }

    private Node GetBrokerAsRandom()
    {
        var brokerIds = _sharedConnectorsByNodeId.Keys.ToArray();
        var index = _brokersNumberCounter.GetNextNumber(brokerIds.Length);

        return _brokerNodesById[brokerIds[index]];
    }

    private bool TryGetSeedConnectorAsRoundRobin(out IKafkaConnector connector)
    {
        if (_seedConnectors.IsEmpty)
        {
            connector = null!;

            return false;
        }

        var seedConnectors = _seedConnectors
            .OrderBy(static pair => pair.Key.ToString(), StringComparer.Ordinal)
            .ToArray();
        var index = _seedConnectorsNumberCounter.GetNextNumber() % seedConnectors.Length;
        connector = seedConnectors[index].Value;

        return true;
    }

    private void InitSeedConnectors(IEnumerable<Node> seedBrokers)
    {
        foreach (var broker in seedBrokers)
        {
            if (_seedConnectors.Keys.Any(endpoint => Utils.EndPointsEqual(endpoint, broker.EndPoint)))
            {
                _logger.IgnoreBootstrapEndpointWarning(broker.EndPoint);

                continue;
            }

            var connector = CreateConnector(broker);
            _seedConnectors.TryAdd(broker.EndPoint, connector);
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
            _loggerFactory)
        {
            NodeId = node.Id,
            IsDedicated = isDedicated
        };
    }

    private void RegisterBrokerNode(Node node)
    {
        _brokerNodesById.AddOrUpdate(node.Id, node, static (_, updatedNode) => updatedNode);
    }

    private void EnsureSharedConnectorsForNode(Node node)
    {
        _sharedConnectorsByNodeId.AddOrUpdate(
            node.Id,
            _ => CreateInitialSharedConnectors(node),
            (_, existingConnectors) => EnsureSharedConnectorsForNode(node, existingConnectors));
    }

    private List<IKafkaConnector> EnsureSharedConnectorsForNode(Node node, List<IKafkaConnector> connectors)
    {
        if (connectors.Any(c => Utils.EndPointsEqual(c.Endpoint, node.EndPoint)))
        {
            return connectors;
        }

        // Shared connectors are the pool-owned transport for metadata-driven traffic,
        // so a newly discovered broker endpoint must have exactly one shared registry entry.
        connectors.Add(CreateSharedConnector(node));

        return connectors;
    }

    private void RemoveStaleSharedConnectors(Node node)
    {
        if (!_sharedConnectorsByNodeId.TryGetValue(node.Id, out var connectors))
        {
            return;
        }

        var staleConnectors = connectors
            .Where(connector => !Utils.EndPointsEqual(connector.Endpoint, node.EndPoint))
            .ToArray();

        foreach (var staleConnector in staleConnectors)
        {
            staleConnector.Dispose();
            connectors.Remove(staleConnector);
        }
    }

    private List<IKafkaConnector> CreateInitialSharedConnectors(Node node)
    {
        return [CreateSharedConnector(node)];
    }

    private IKafkaConnector CreateSharedConnector(Node node)
    {
        var bootstrapEntry = _seedConnectors.FirstOrDefault(pair => Utils.EndPointsEqual(pair.Key, node.EndPoint));

        if (!bootstrapEntry.Equals(default(KeyValuePair<EndPoint, IKafkaConnector>))
            && _seedConnectors.TryRemove(bootstrapEntry.Key, out var bootstrapConnector))
        {
            // Promoting the bootstrap connector preserves the already-known transport target
            // while moving ownership into the shared broker registry.
            bootstrapConnector.NodeId = node.Id;

            return bootstrapConnector;
        }

        return CreateConnector(node);
    }

    private async ValueTask OpenSharedConnectorsAsync(int nodeId, CancellationToken token)
    {
        if (!_sharedConnectorsByNodeId.TryGetValue(nodeId, out var connectors))
        {
            return;
        }

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
                // The shared registry keeps the ownership of broker-facing traffic,
                // so an unusable connector must be removed from this registry after the open attempt fails.
            }
        }
    }

    private void RemoveDeadSharedConnectors(int nodeId)
    {
        if (!_sharedConnectorsByNodeId.TryGetValue(nodeId, out var connectors))
        {
            return;
        }

        var deadConnectors = connectors
            .Where(static connector => connector.ConnectorState != KafkaConnector.State.Open)
            .ToArray();

        foreach (var deadConnector in deadConnectors)
        {
            deadConnector.Dispose();
            connectors.Remove(deadConnector);
        }

        if (connectors.Count != 0)
        {
            return;
        }

        _sharedConnectorsByNodeId.TryRemove(nodeId, out _);
    }

    private void RemoveSharedConnectorsMissingFromMetadata(HashSet<int> metadataBrokerIds)
    {
        var removedBrokerIds = _sharedConnectorsByNodeId.Keys
            .Where(nodeId => !metadataBrokerIds.Contains(nodeId))
            .ToArray();

        foreach (var removedBrokerId in removedBrokerIds)
        {
            if (!_sharedConnectorsByNodeId.TryRemove(removedBrokerId, out var removedConnectors))
            {
                continue;
            }

            // Shared connectors are derived from the current metadata snapshot, so when a broker
            // disappears from that snapshot the pool must stop routing metadata-driven traffic to it.
            DisposeConnectors(removedConnectors);
            _brokerNodesById.TryRemove(removedBrokerId, out _);
        }
    }

    private static void DisposeConnectors(IEnumerable<IKafkaConnector> connectors)
    {
        foreach (var connector in connectors)
        {
            connector.Dispose();
        }
    }

    private static async ValueTask DisposeConnectorsAsync(IEnumerable<IKafkaConnector> connectors)
    {
        foreach (var connector in connectors)
        {
            await connector.DisposeAsync();
        }
    }
}
