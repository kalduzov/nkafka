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

using System.Net.Security;
using System.Net.Sockets;

namespace NKafka.Connection;

/// <inheritdoc />
internal class SocketFactory: ISocketFactory
{
    /// <inheritdoc />
    public ISocketProxy CreateSocket(SocketType socketType, ProtocolType protocolType, int receiveBufferBytes)
    {
        var socketProxy = new SocketProxy(socketType, protocolType);

        if (receiveBufferBytes != -1)
        {
            socketProxy.Socket.ReceiveBufferSize = receiveBufferBytes;
        }

        return socketProxy;
    }

    /// <inheritdoc />
    public Stream CreateNetworkStream(Socket socket, bool ownsSocket)
    {
        return new NetworkStream(socket, ownsSocket);
    }

    /// <inheritdoc />
    public Stream CreateSslStream(Stream inputStream)
    {
        return new SslStream(inputStream, false, (_, _, _, _) => true); //skip server cert validation
    }
}