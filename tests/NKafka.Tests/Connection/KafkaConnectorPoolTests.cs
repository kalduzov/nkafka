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
    public void TryGetBootstrapConnector_WhenNoConnectors_ReturnsFalse()
    {
        var connectorPool = CreateConnectorPool();

        var result = connectorPool.TryGetBootstrapConnector(out var connector);
        result.Should().BeFalse();
        connector.Should().BeNull();
    }

    [Fact]
    public void TryGetSharedConnector_WithBrokers_Successful()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            _fixture.Borkers,
            _fixture.SeedBrokers,
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        var result = connectorPool.TryGetSharedConnector(1, out var connector);
        result.Should().BeTrue();
        connector.Should().NotBeNull();
    }

    [Fact]
    public void TryGetBootstrapConnector_WithSeeds_Successful()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            Array.Empty<IKafkaConnector>(),
            _fixture.SeedBrokers,
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        var result = connectorPool.TryGetBootstrapConnector(out var connector);
        result.Should().BeTrue();
        connector.Should().NotBeNull();
        ((IPEndPoint)connector.Endpoint).Port.Should().Be(9001);

        result = connectorPool.TryGetBootstrapConnector(out connector);
        result.Should().BeTrue();
        connector.Should().NotBeNull();
        ((IPEndPoint)connector.Endpoint).Port.Should().Be(9002);
    }

    [Fact]
    public void TryGetAnySharedBrokerConnector_WithBrokers_Successful()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            _fixture.Borkers,
            _fixture.SeedBrokers,
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        var result = connectorPool.TryGetAnySharedBrokerConnector(out var connector);
        result.Should().BeTrue();
        connector.Should().NotBeNull();
        connector.CurrentNumberInflightRequests.Should().Be(1);
    }

    [Fact]
    public void TryCreateDedicatedConnector_WithKnownBroker_ReturnsDedicatedConnector()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            _fixture.Borkers,
            _fixture.SeedBrokers,
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        var result = connectorPool.TryCreateDedicatedConnector(1, out var connector);

        result.Should().BeTrue();
        connector.Should().NotBeNull();
        connector.IsDedicated.Should().BeTrue();
        connector.NodeId.Should().Be(1);
    }

    [Fact]
    public void Dispose_DisposesSharedConnectors()
    {
        var config = new ClusterConfig();
        var sharedConnector = Substitute.For<IKafkaConnector>();
        sharedConnector.Endpoint.Returns(new IPEndPoint(IPAddress.Loopback, 9011));
        sharedConnector.NodeId.Returns(1);

        var connectorPool = new KafkaConnectorPool(
            [sharedConnector],
            Array.Empty<IKafkaConnector>(),
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        connectorPool.TryCreateDedicatedConnector(1, out _).Should().BeTrue();

        connectorPool.Dispose();

        sharedConnector.Received(1).Dispose();
    }

    [Fact]
    public void Dispose_DisposesSeedConnectors()
    {
        var config = new ClusterConfig();
        var seedConnector = Substitute.For<IKafkaConnector>();
        seedConnector.Endpoint.Returns(new IPEndPoint(IPAddress.Loopback, 9010));

        var connectorPool = new KafkaConnectorPool(
            Array.Empty<IKafkaConnector>(),
            [seedConnector],
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        connectorPool.Dispose();

        seedConnector.Received(1).Dispose();
    }

    [Fact]
    public async Task AddOrUpdateConnectorsAsync_RemovesSharedConnectorsForBrokersMissingFromMetadata()
    {
        var config = new ClusterConfig();
        var brokerOneConnector = Substitute.For<IKafkaConnector>();
        brokerOneConnector.Endpoint.Returns(Utils.BuildEndPoint("127.0.0.1", 9011));
        brokerOneConnector.NodeId.Returns(1);
        brokerOneConnector.ConnectorState.Returns(KafkaConnector.State.Open);

        var brokerTwoConnector = Substitute.For<IKafkaConnector>();
        brokerTwoConnector.Endpoint.Returns(Utils.BuildEndPoint("127.0.0.1", 9012));
        brokerTwoConnector.NodeId.Returns(2);
        brokerTwoConnector.ConnectorState.Returns(KafkaConnector.State.Open);

        var connectorPool = new KafkaConnectorPool(
            [brokerOneConnector, brokerTwoConnector],
            Array.Empty<IKafkaConnector>(),
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        await connectorPool.AddOrUpdateConnectorsAsync(
            [new Node(1, "127.0.0.1", 9011)],
            CancellationToken.None);

        connectorPool.TryGetSharedConnector(2, out _).Should().BeFalse();
        connectorPool.TryCreateDedicatedConnector(2, out _).Should().BeFalse();
        brokerTwoConnector.Received(1).Dispose();
    }

    [Fact]
    public async Task AddOrUpdateConnectorsAsync_DoesNotAllowNewDedicatedConnector_WhenBrokerWasRemovedFromMetadata()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            _fixture.Borkers,
            _fixture.SeedBrokers,
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        connectorPool.TryCreateDedicatedConnector(3, out var dedicatedConnector).Should().BeTrue();
        dedicatedConnector.IsDedicated.Should().BeTrue();

        await connectorPool.AddOrUpdateConnectorsAsync(
            [new Node(1, "127.0.0.1", 9001)],
            CancellationToken.None);

        connectorPool.TryCreateDedicatedConnector(3, out _).Should().BeFalse();
    }

    [Fact]
    public async Task AddOrUpdateConnectorsAsync_PromotesBootstrapConnectorIntoSharedRegistry()
    {
        var config = new ClusterConfig();
        var seedConnector = Substitute.For<IKafkaConnector>();
        seedConnector.Endpoint.Returns(Utils.BuildEndPoint("127.0.0.1", 9010));
        seedConnector.NodeId.Returns(Node.UNKNOWN_ID);
        seedConnector.ConnectorState.Returns(KafkaConnector.State.Open);
        seedConnector.CurrentNumberInflightRequests.Returns(0);

        var connectorPool = new KafkaConnectorPool(
            Array.Empty<IKafkaConnector>(),
            [seedConnector],
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        await connectorPool.AddOrUpdateConnectorsAsync(
            [new Node(10, "127.0.0.1", 9010)],
            CancellationToken.None);

        connectorPool.TryGetBootstrapConnector(out _).Should().BeFalse();
        connectorPool.TryGetSharedConnector(10, out var sharedConnector).Should().BeTrue();
        sharedConnector.Should().BeSameAs(seedConnector);
    }

    [Fact]
    public async Task AddOrUpdateConnectorsAsync_ReplacesSharedConnector_WhenBrokerEndpointChanges()
    {
        var config = new ClusterConfig();
        var oldConnector = Substitute.For<IKafkaConnector>();
        var promotedConnector = Substitute.For<IKafkaConnector>();
        oldConnector.Endpoint.Returns(Utils.BuildEndPoint("127.0.0.1", 9011));
        oldConnector.NodeId.Returns(1);
        oldConnector.ConnectorState.Returns(KafkaConnector.State.Open);
        oldConnector.CurrentNumberInflightRequests.Returns(0);
        promotedConnector.Endpoint.Returns(Utils.BuildEndPoint("127.0.0.1", 9021));
        promotedConnector.NodeId.Returns(Node.UNKNOWN_ID);
        promotedConnector.ConnectorState.Returns(KafkaConnector.State.Open);
        promotedConnector.CurrentNumberInflightRequests.Returns(0);

        var connectorPool = new KafkaConnectorPool(
            [oldConnector],
            [promotedConnector],
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
            config.MessageMaxBytes,
            config.CloseConnectionTimeoutMs,
            config.ConnectionsMaxIdleMs,
            config.RequestTimeoutMs,
            config.ReceiveBufferBytes,
            config.SecurityProtocol,
            config.ClientId,
            config.ApiVersionRequest,
            NullLoggerFactory.Instance);

        await connectorPool.AddOrUpdateConnectorsAsync(
            [new Node(1, "127.0.0.1", 9021)],
            CancellationToken.None);

        connectorPool.TryGetSharedConnector(1, out var sharedConnector).Should().BeTrue();
        sharedConnector.Endpoint.Should().Be(Utils.BuildEndPoint("127.0.0.1", 9021));
        sharedConnector.Should().BeSameAs(promotedConnector);
        oldConnector.Received(1).Dispose();
    }

    private static KafkaConnectorPool CreateConnectorPool()
    {
        var config = new ClusterConfig();

        var connectorPool = new KafkaConnectorPool(
            Array.Empty<Node>(),
            config.Ssl,
            config.Sasl,
            CommonConfig.MaxInflightRequests,
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
