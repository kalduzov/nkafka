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
    public bool Connected => Socket.Connected;

    public Socket Socket { get; }

    private readonly TaskCompletionSource _connectCompletionSource = new();
    private readonly SocketAsyncEventArgs _socketEventArgs;

    public SocketProxy(SocketType socketType, ProtocolType protocolType)
    {
        Socket = new Socket(socketType, protocolType);
        _socketEventArgs = new SocketAsyncEventArgs();
        _socketEventArgs.Completed += SocketEventArgsOnCompleted;

    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _socketEventArgs.Completed -= SocketEventArgsOnCompleted;
        _socketEventArgs.Dispose();
        Socket.Dispose();
    }

    public async ValueTask ConnectAsync(EndPoint remoteEp, CancellationToken token)
    {
        if (Socket.Connected)
        {
            return;
        }

        _socketEventArgs.RemoteEndPoint = remoteEp;

        if (Socket.ConnectAsync(_socketEventArgs))
        {
            await _connectCompletionSource.Task;

            return;
        }

        ConnectCompleted(_socketEventArgs);
    }

    private void SocketEventArgsOnCompleted(object? sender, SocketAsyncEventArgs e)
    {
        switch (e.LastOperation)
        {
            case SocketAsyncOperation.Connect:
                ConnectCompleted(e);

                break;
            case SocketAsyncOperation.None:
            case SocketAsyncOperation.Accept:
            case SocketAsyncOperation.Disconnect:
            case SocketAsyncOperation.Receive:
            case SocketAsyncOperation.ReceiveFrom:
            case SocketAsyncOperation.ReceiveMessageFrom:
            case SocketAsyncOperation.Send:
            case SocketAsyncOperation.SendPackets:
            case SocketAsyncOperation.SendTo:
                break;
        }

    }

    private void ConnectCompleted(SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.Success)
        {
            _connectCompletionSource.SetResult();
        }
        else
        {
            _connectCompletionSource.SetException(new SocketException((int)e.SocketError));
        }
    }

    public void Close(int timeout)
    {
        Socket.Close(timeout);
    }
}