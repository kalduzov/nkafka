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

using System.Net;

using Microsoft.Extensions.Logging.Abstractions;

using NKafka.Config;
using NKafka.Connection;
using NKafka.Exceptions;
using NKafka.Tests.Connection.Fixtures;

namespace NKafka.Tests.Connection;

public class KafkaConnectorPoolTests: IClassFixture<ConnectorFixture>
{
    private readonly ConnectorFixture _fixture;

    public KafkaConnectorPoolTests(ConnectorFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CreateConnectorPool_Successful()
    {
        var connectorPool = CreateConnectorPool();
        connectorPool.Should().NotBeNull();
    }

    [Fact]
    public void GetConnectorFromPool_WhenNoConnectors_Successful()
    {
        var connectorPool = CreateConnectorPool();

        IKafkaConnector GetConnector()
        {
            return connectorPool.GetConnector();
        }

        FluentActions.Invoking(GetConnector)
            .Should()
            .Throw<ConnectorNotFoundException>();
    }

    [Fact]
    public void TryGetConnector_WithBrokers_Successful()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            _fixture.Borkers,
            _fixture.SeedBrokers,
            config.Ssl,
            config.Sasl,
            config.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        var result = connectorPool.TryGetConnector(1, false, out var connector);
        result.Should().BeTrue();
        connector.Should().NotBeNull();
    }

    [Fact]
    public void GetConnector_WithSeeds_Successful()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            Array.Empty<IKafkaConnector>(),
            _fixture.SeedBrokers,
            config.Ssl,
            config.Sasl,
            config.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        var connector = connectorPool.GetConnector();
        connector.Should().NotBeNull();
        ((IPEndPoint)connector.Endpoint).Port.Should().Be(9001);

        connector = connectorPool.GetConnector();
        connector.Should().NotBeNull();
        ((IPEndPoint)connector.Endpoint).Port.Should().Be(9002);
    }

    [Fact]
    public void GetConnector_WithBrokers_Successful()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            _fixture.Borkers,
            _fixture.SeedBrokers,
            config.Ssl,
            config.Sasl,
            config.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        var connector = connectorPool.GetConnector();
        connector.Should().NotBeNull();
        connector.CurrentNumberInflightRequests.Should().Be(1);
    }

    private static KafkaConnectorPool CreateConnectorPool()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            Array.Empty<Node>(),
            config.Ssl,
            config.Sasl,
            config.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        return connectorPool;
    }
}