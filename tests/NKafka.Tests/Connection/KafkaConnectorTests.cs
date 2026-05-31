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

using Microsoft.Extensions.Logging.Abstractions;

using System.Net;
using System.Net.Security;
using System.Net.Sockets;

using NKafka.Config;
using NKafka.Connection;
using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Protocol;
using NSubstitute;

namespace NKafka.Tests.Connection;

public class KafkaConnectorTests
{
    [Fact]
    public void CreateConnector_Successful()
    {
        IKafkaConnector CreateConnector()
            => new KafkaConnector(
                CreateEndpoint(),
                100,
                1000,
                1000,
                1000,
                1000,
                0,
                SecurityProtocols.PlainText,
                SaslSettings.None,
                SslSettings.None,
                "test",
                true,
                CreateSocketFactoryContext(CreateEndpoint()).SocketFactory,
                NullLoggerFactory.Instance);

        FluentActions.Invoking(CreateConnector).Should().NotThrow();
    }

    [Fact]
    public async Task ConnectorOpen_Successful()
    {
        var kafkaConnector = CreateConnector(apiRequest: true);

        await kafkaConnector.OpenAsync(CancellationToken.None);
        kafkaConnector.ConnectorState.Should().Be(KafkaConnector.State.Open);
    }

    [Fact]
    public async Task OpenAsync_WithApiVersionNegotiation_PublishesSupportVersions()
    {
        var kafkaConnector = CreateConnector(apiRequest: true);

        await kafkaConnector.OpenAsync(CancellationToken.None);

        kafkaConnector.SupportVersions.Should().ContainKey(ApiKeys.ApiVersions);
        kafkaConnector.SupportVersions.Should().ContainKey(ApiKeys.Metadata);
    }

    [Fact]
    public async Task OpenAsync_WithApiVersionNegotiationDisabled_LeavesSupportVersionsEmpty()
    {
        var kafkaConnector = CreateConnector(apiRequest: false);

        await kafkaConnector.OpenAsync(CancellationToken.None);

        kafkaConnector.SupportVersions.Should().BeEmpty();
    }

    [Fact]
    public async Task OpenAsync_WithSslTransport_CreatesSslStream()
    {
        var socketContext = CreateSocketFactoryContext(CreateEndpoint());
        var kafkaConnector = CreateConnector(
            apiRequest: false,
            securityProtocol: SecurityProtocols.Ssl,
            socketContext: socketContext);

        await kafkaConnector.OpenAsync(CancellationToken.None);

        socketContext.SocketFactory.Received(1).CreateSslStream(Arg.Any<Stream>());
    }

    [Fact]
    public async Task SendAsync_DoesNotReestablishSession_WhenConnectorIsAlreadyOpen()
    {
        var socketContext = CreateSocketFactoryContext(CreateEndpoint());
        var kafkaConnector = CreateConnector(
            apiRequest: true,
            socketContext: socketContext);

        await kafkaConnector.OpenAsync(CancellationToken.None);

        await ((IKafkaConnector)kafkaConnector).SendAsync<MetadataRequestMessage, MetadataResponseMessage>(
            MetadataRequestMessage.Build(false, null),
            false,
            CancellationToken.None);

        await socketContext.SocketProxy.Received(1).ConnectAsync(Arg.Any<EndPoint>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DisposeAsync_ClearsSupportVersionsAndClosesConnector()
    {
        var kafkaConnector = CreateConnector(apiRequest: true);

        await kafkaConnector.OpenAsync(CancellationToken.None);

        kafkaConnector.SupportVersions.Should().NotBeEmpty();

        await kafkaConnector.DisposeAsync();

        kafkaConnector.ConnectorState.Should().Be(KafkaConnector.State.Closed);
        kafkaConnector.SupportVersions.Should().BeEmpty();
    }

    [Fact]
    public async Task DisposeAsync_FailsInflightRequestsAndClearsRegistry()
    {
        var kafkaConnector = CreateConnector(apiRequest: true, requestsWithoutResponse: [ApiKeys.Metadata]);

        await kafkaConnector.OpenAsync(CancellationToken.None);

        var responseTask = ((IKafkaConnector)kafkaConnector).SendAsync<MetadataRequestMessage, MetadataResponseMessage>(
            MetadataRequestMessage.Build(false, null),
            false,
            CancellationToken.None);

        await kafkaConnector.DisposeAsync();

        await FluentActions.Awaiting(async () => await responseTask)
            .Should()
            .ThrowAsync<ConnectionKafkaException>();

        kafkaConnector.CurrentNumberInflightRequests.Should().Be(0);
        kafkaConnector.ConnectorState.Should().Be(KafkaConnector.State.Closed);
    }

    private KafkaConnector CreateConnector(
        bool apiRequest,
        IEnumerable<ApiKeys>? requestsWithoutResponse = null,
        SecurityProtocols securityProtocol = SecurityProtocols.PlainText,
        SocketFactoryContext? socketContext = null)
        => new(
            CreateEndpoint(),
            100,
            1000,
            1000,
            1000,
            1000,
            0,
            securityProtocol,
            SaslSettings.None,
            SslSettings.None,
            "test",
            apiRequest,
            (socketContext ?? CreateSocketFactoryContext(CreateEndpoint(), requestsWithoutResponse)).SocketFactory,
            NullLoggerFactory.Instance);

    private static IPEndPoint CreateEndpoint()
        => new(IPAddress.Loopback, 9000);

    private static SocketFactoryContext CreateSocketFactoryContext(EndPoint endpoint, IEnumerable<ApiKeys>? requestsWithoutResponse = null)
    {
        var isConnected = false;
        var socketMock = Substitute.For<ISocketProxy>();
        socketMock.ConnectAsync(endpoint, Arg.Any<CancellationToken>())
            .Returns(ValueTask.CompletedTask)
            .AndDoes(_ => isConnected = true);
        socketMock.Connected.Returns(_ => isConnected);

        var mockStream = new MockStream(requestsWithoutResponse);
        var remoteCertificateValidationCallback = new RemoteCertificateValidationCallback((_, _, _, _) => true);
        var sslStreamMock = Substitute.For<SslStream>(mockStream, false, remoteCertificateValidationCallback);

        var socketFactory = Substitute.For<ISocketFactory>();
        socketFactory.CreateSocket(SocketType.Stream, ProtocolType.Tcp, 0).Returns(socketMock);
        socketFactory.CreateNetworkStream(Arg.Any<Socket>(), Arg.Any<bool>()).Returns(mockStream);
        socketFactory.CreateSslStream(Arg.Any<Stream>()).Returns(sslStreamMock);

        return new SocketFactoryContext(socketFactory, socketMock);
    }

    private sealed record SocketFactoryContext(ISocketFactory SocketFactory, ISocketProxy SocketProxy);
}
