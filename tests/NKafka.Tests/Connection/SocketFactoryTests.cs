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

using NKafka.Connection;

namespace NKafka.Tests.Connection;

public class SocketFactoryTests
{
    private readonly DnsEndPoint _endPoint;

    public SocketFactoryTests()
    {
        _endPoint = new DnsEndPoint("google.com", 443); //todo: It is required to find a more technical address to test the ssl connection.
    }

    [Fact]
    public void CreateSocket_Successful()
    {
        var socket = new SocketFactory().CreateSocket(SocketType.Stream, ProtocolType.Tcp, 0);
        socket.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateStream_Successful()
    {
        var socketFactory = new SocketFactory();
        var socketProxy = socketFactory.CreateSocket(SocketType.Stream, ProtocolType.Tcp, 0);
        await socketProxy.ConnectAsync(_endPoint);
        var stream = socketFactory.CreateNetworkStream(socketProxy.Socket, true);
        stream.Should().NotBeNull();
        stream.CanWrite.Should().BeTrue();
    }

    [Fact]
    public async Task CreateSslStream_Successful()
    {
        var socketFactory = new SocketFactory();
        var socketProxy = socketFactory.CreateSocket(SocketType.Stream, ProtocolType.Tcp, 0);
        await socketProxy.ConnectAsync(_endPoint);
        var stream = socketFactory.CreateNetworkStream(socketProxy.Socket, true);
        var sslStream = socketFactory.CreateSslStream(stream);
        await ((SslStream)sslStream).AuthenticateAsClientAsync("google.com");
        sslStream.Should().NotBeNull();

        sslStream.CanWrite.Should().BeTrue();
    }
}