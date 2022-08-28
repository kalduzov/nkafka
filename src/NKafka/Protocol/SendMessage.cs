﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

using NKafka.Messages;
using NKafka.Protocol.Extensions;

namespace NKafka.Protocol;

public class SendMessage
{
    /// <summary>
    /// 
    /// </summary>
    public RequestHeader Header { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public RequestMessage Content { get; set; }

    public SendMessage(RequestHeader header, RequestMessage content)
    {
        Header = header;
        Content = content;

        //RequestLength = header.Length + content.MessageSize;
    }

    public void Write(Stream writableStream)
    {
        using var stream = new MemoryStream();
        var writer = new BufferWriter(stream);

        Header.Write(writer, (ApiVersions)Header.RequestApiVersion);
        Content.Write(writer, (ApiVersions)Header.RequestApiVersion);

        writableStream.WriteLength((int)stream.Length);
        stream.WriteTo(writableStream);
    }
}