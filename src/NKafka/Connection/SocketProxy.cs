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
using System.Net.Sockets;

namespace NKafka.Connection;

internal class SocketProxy: ISocketProxy
{
    public SocketProxy(SocketType socketType, ProtocolType protocolType)
    {
        Socket = new Socket(socketType, protocolType);
    }

    public bool Connected => Socket.Connected;

    public Socket Socket { get; }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Socket.Dispose();
    }

    public ValueTask ConnectAsync(EndPoint remoteEp, CancellationToken token)
    {
        return Socket.ConnectAsync(remoteEp, token);
    }

    public void Close(int timeout)
    {
        Socket.Close(timeout);
    }
}