//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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

using NKafka.Connection;

namespace NKafka.Tests.Connection.Fixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class ConnectorFixture
{
    private readonly ISocketFactory _socketFactoryMock;
    private readonly IPEndPoint _endPoint = new(IPAddress.Loopback, 9000);
    private bool _isConnected;

    internal ISocketFactory SocketFactory => _socketFactoryMock;

    internal IReadOnlyCollection<IKafkaConnector> SeedBrokers { get; }

    internal IReadOnlyCollection<IKafkaConnector> Borkers { get; }

    internal IPEndPoint EndPoint => _endPoint;

    public ConnectorFixture()
    {
        var socketMock = Substitute.For<ISocketProxy>();

        socketMock.ConnectAsync(_endPoint, Arg.Any<CancellationToken>())
            .Returns(ValueTask.CompletedTask)
            .AndDoes(_ => _isConnected = true);

        socketMock.Connected.Returns(_isConnected);

        var mockStream = new MockStream();

        var remoteCertificateValidationCallback = new RemoteCertificateValidationCallback((_, _, _, _) => true);
        var sslStreamMock = Substitute.For<SslStream>(mockStream, false, remoteCertificateValidationCallback);

        _socketFactoryMock = Substitute.For<ISocketFactory>();
        _socketFactoryMock.CreateSocket(SocketType.Stream, ProtocolType.Tcp, 0).Returns(socketMock);
        _socketFactoryMock.CreateNetworkStream(Arg.Any<Socket>(), Arg.Any<bool>()).Returns(mockStream);
        _socketFactoryMock.CreateSslStream(Arg.Any<Stream>()).Returns(sslStreamMock);

        SeedBrokers = CreateSeedBrokers();
        Borkers = CreateBrokers();
    }

    private static IReadOnlyCollection<IKafkaConnector> CreateBrokers()
    {
        var list = new List<IKafkaConnector>();

        var connector1 = Substitute.For<IKafkaConnector>();
        connector1.IsDedicated.Returns(false);
        EndPoint address = new IPEndPoint(IPAddress.Loopback, 9001);
        connector1.Endpoint.Returns(address);
        connector1.NodeId.Returns(1);
        connector1.CurrentNumberInflightRequests.Returns(10);

        var connector2 = Substitute.For<IKafkaConnector>();
        connector2.IsDedicated.Returns(false);
        address = new IPEndPoint(IPAddress.Loopback, 9002);
        connector2.Endpoint.Returns(address);
        connector2.NodeId.Returns(1);
        connector2.CurrentNumberInflightRequests.Returns(1);

        var connector3 = Substitute.For<IKafkaConnector>();
        connector3.IsDedicated.Returns(false);
        address = new IPEndPoint(IPAddress.Loopback, 9003);
        connector3.Endpoint.Returns(address);
        connector3.NodeId.Returns(3);
        connector3.CurrentNumberInflightRequests.Returns(1);

        var connector4 = Substitute.For<IKafkaConnector>();
        connector4.IsDedicated.Returns(false);
        address = new IPEndPoint(IPAddress.Loopback, 9004);
        connector4.Endpoint.Returns(address);
        connector4.NodeId.Returns(4);
        connector4.CurrentNumberInflightRequests.Returns(10);

        var connector5 = Substitute.For<IKafkaConnector>();
        connector5.IsDedicated.Returns(false);
        address = new IPEndPoint(IPAddress.Loopback, 9005);
        connector5.Endpoint.Returns(address);
        connector5.NodeId.Returns(4);
        connector5.CurrentNumberInflightRequests.Returns(1);

        list.Add(connector1);
        list.Add(connector2);
        list.Add(connector3);
        list.Add(connector4);
        list.Add(connector5);

        return list;
    }

    private static IReadOnlyCollection<IKafkaConnector> CreateSeedBrokers()
    {
        var list = new List<IKafkaConnector>();

        var connector1 = Substitute.For<IKafkaConnector>();
        connector1.IsDedicated.Returns(false);
        EndPoint address = new IPEndPoint(IPAddress.Loopback, 9001);
        connector1.Endpoint.Returns(address);
        connector1.NodeId.Returns(-1);

        var connector2 = Substitute.For<IKafkaConnector>();
        connector2.IsDedicated.Returns(false);
        address = new IPEndPoint(IPAddress.Loopback, 9002);
        connector2.Endpoint.Returns(address);
        connector2.NodeId.Returns(-1);

        var connector3 = Substitute.For<IKafkaConnector>();
        connector3.IsDedicated.Returns(false);
        address = new IPEndPoint(IPAddress.Loopback, 9003);
        connector3.Endpoint.Returns(address);
        connector3.NodeId.Returns(-1);

        var connector4 = Substitute.For<IKafkaConnector>();
        connector4.IsDedicated.Returns(false);
        address = new IPEndPoint(IPAddress.Loopback, 9004);
        connector4.Endpoint.Returns(address);
        connector4.NodeId.Returns(-1);

        list.Add(connector1);
        list.Add(connector2);
        list.Add(connector3);
        list.Add(connector4);

        return list;
    }
}