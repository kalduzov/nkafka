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

using NKafka.Config;

namespace NKafka.Connection;

internal partial class KafkaConnectorPool: IKafkaConnectorPool
{
    private readonly SslSettings _sslSettings;
    private readonly SaslSettings _saslSettings;
    private readonly int _maxInflightRequests;
    private readonly int _messageMaxBytes;
    private readonly int _closeConnectionTimeoutMs;
    private readonly int _connectionsMaxIdleMs;
    private readonly int _requestTimeoutMs;
    private readonly SecurityProtocols _securityProtocol;
    private readonly string _clientId;
    private readonly ILoggerFactory _loggerFactory;

    private readonly Dictionary<EndPoint, IKafkaConnector> _seedConnectors; //Посевные соединения, используются только на начальном этапе жизни пула
    private ConcurrentDictionary<IBroker, List<IKafkaConnector>> _brokersConnectors = new(); //Список всех соединений с брокерами
    private readonly ConcurrentDictionary<int, IBroker> _brokers = new(); //список всех брокеров по их id

    private readonly ILogger<KafkaConnectorPool> _logger;
    private readonly Random _random = new();

    public KafkaConnectorPool(
        IReadOnlyCollection<IBroker> seedBrokers,
        SslSettings sslSettings,
        SaslSettings saslSettings,
        int maxInflightRequests,
        int messageMaxBytes,
        int closeConnectionTimeoutMs,
        int connectionsMaxIdleMs,
        int requestTimeoutMs,
        SecurityProtocols securityProtocol,
        string clientId,
        ILoggerFactory loggerFactory)
    {
        _sslSettings = sslSettings;
        _saslSettings = saslSettings;
        _maxInflightRequests = maxInflightRequests;
        _messageMaxBytes = messageMaxBytes;
        _closeConnectionTimeoutMs = closeConnectionTimeoutMs;
        _connectionsMaxIdleMs = connectionsMaxIdleMs;
        _requestTimeoutMs = requestTimeoutMs;
        _securityProtocol = securityProtocol;
        _clientId = clientId;
        _loggerFactory = loggerFactory;
        _seedConnectors = new Dictionary<EndPoint, IKafkaConnector>(seedBrokers.Count);
        _logger = loggerFactory.CreateLogger<KafkaConnectorPool>();

        InitSeedConnectors(seedBrokers);
    }

    private void InitSeedConnectors(IEnumerable<IBroker> seedBrokers)
    {
        foreach (var broker in seedBrokers)
        {
            var connector = CreateConnector(broker);
            _seedConnectors.TryAdd(broker.EndPoint, connector);
        }
    }

    private IKafkaConnector CreateConnector(IBroker broker)
    {
        _logger.CreateConnectorTrace(broker.EndPoint);

        return new KafkaConnector(
            broker.EndPoint,
            _maxInflightRequests,
            _messageMaxBytes,
            _closeConnectionTimeoutMs,
            _connectionsMaxIdleMs,
            _requestTimeoutMs,
            _securityProtocol,
            _saslSettings,
            _sslSettings,
            _clientId,
            _loggerFactory);
    }

    public bool TryGetConnector(int? controllerId, out IKafkaConnector connector)
    {
        connector = null;

        return false;
    }

    public IKafkaConnector GetRandomConnector()
    {
        if (_brokers.IsEmpty)
        {
            //todo allocation
            return _seedConnectors.Values.AsEnumerable().Shuffle().First();
        }

        var index = _random.Next(_brokers.Count);
        var broker = ((ReadOnlyCollection<int>)_brokers.Keys)[index];

        return _brokersConnectors[_brokers[broker]].First();
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
}