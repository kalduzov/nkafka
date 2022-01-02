// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 * 
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.IO;
using Microlibs.Kafka.Protocol.Extensions;

namespace Microlibs.Kafka.Protocol;

public class KafkaRequestMessage
{
    public readonly int RequestLength;

    public KafkaRequestMessage(KafkaRequestHeader header, RequestBody content)
    {
        Header = header;
        Content = content;
        RequestLength = header.Length + content.Length;
    }

    /// <summary>
    /// </summary>
    public short ApiVersion { get; set; }

    public KafkaRequestHeader Header { get; set; }

    public RequestBody Content { get; set; }

    public void ToByteStream(Stream stream)
    {
        stream.WriteLength(RequestLength);
        stream.WriteHeader(Header);
        stream.WriteMessage(Content);
    }
}