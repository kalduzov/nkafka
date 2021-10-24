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
using System.Net.Security;
using System.Net.Sockets;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IO;

using NKafka.Config;
using NKafka.Connection;

namespace NKafka.Tests.Connection;

public class KafkaConnectorTests
{
    private readonly IPEndPoint _endPoint = new(IPAddress.Loopback, 9000);
    private bool _isConnected;
    private readonly Mock<ISocketFactory> _socketFactoryMock;

    public KafkaConnectorTests()
    {
        Mock<ISocketProxy> socketMock = new();

        socketMock.Setup(s => s.ConnectAsync(_endPoint, It.IsAny<CancellationToken>())).Callback(() => _isConnected = true);
        socketMock.Setup(s => s.Connected).Returns(_isConnected);

        Stream mockStream = new MockStream();

        var remoteCertificateValidationCallback = new RemoteCertificateValidationCallback((_, _, _, _) => true);
        var sslStreamMock = new Mock<SslStream>(mockStream, false, remoteCertificateValidationCallback);

        _socketFactoryMock = new Mock<ISocketFactory>();
        _socketFactoryMock.Setup(sf => sf.CreateSocket(SocketType.Stream, ProtocolType.Tcp, 0)).Returns(socketMock.Object);
        _socketFactoryMock.Setup(sf => sf.CreateNetworkStream(It.IsAny<Socket>(), It.IsAny<bool>())).Returns(mockStream);
        _socketFactoryMock.Setup(sf => sf.CreateSslStream(It.IsAny<Stream>())).Returns(sslStreamMock.Object);
    }

    [Fact]
    public void CreateConnector_Successful()
    {
        IKafkaConnector CreateConnector()
            => new KafkaConnector(
                _endPoint,
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
                _socketFactoryMock.Object,
                new RecyclableMemoryStreamManager(),
                NullLoggerFactory.Instance);

        FluentActions.Invoking(CreateConnector).Should().NotThrow();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ConnectorOpen_Successful(bool apiRequest)
    {
        var kafkaConnector = new KafkaConnector(
            _endPoint,
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
            apiRequest,
            _socketFactoryMock.Object,
            new RecyclableMemoryStreamManager(),
            NullLoggerFactory.Instance);

        await kafkaConnector.OpenAsync(CancellationToken.None);
        kafkaConnector.ConnectorState.Should().Be(KafkaConnector.State.Open);
    }
}