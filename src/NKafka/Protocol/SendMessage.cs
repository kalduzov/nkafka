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
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using Microsoft.IO;

using NKafka.Messages;
using NKafka.Protocol.Extensions;

namespace NKafka.Protocol;

public readonly struct SendMessage
{
    private static readonly RecyclableMemoryStreamManager _manager = new();

    private ApiVersion Version { get; }

    /// <summary>
    /// 
    /// </summary>
    public RequestHeader Header { get; }

    /// <summary>
    /// 
    /// </summary>
    public IRequestMessage Content { get; }

    public SendMessage(RequestHeader header, IRequestMessage content, ApiVersion version)
    {
        Version = version;
        Header = header;
        Content = content;
    }

    public void Write(Stream writableStream)
    {
        using var stream = _manager.GetStream();
        var writer = new BufferWriter(stream);

        Header.Write(writer, (ApiVersion)Header.RequestApiVersion);
        Content.Write(writer, Version);

        writableStream.WriteLength((int)stream.Length);
        stream.WriteTo(writableStream);
    }
}