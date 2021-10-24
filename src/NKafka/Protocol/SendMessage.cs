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

using NKafka.Exceptions;
using NKafka.Messages;

namespace NKafka.Protocol;

public readonly struct SendMessage
{
    private readonly ApiVersion _messageVersion;
    private readonly ApiVersion _headerVersion;
    private readonly RecyclableMemoryStreamManager _streamManager;

    /// <summary>
    /// 
    /// </summary>
    public RequestHeader Header { get; }

    /// <summary>
    /// 
    /// </summary>
    public IRequestMessage RequestMessage { get; }

    public SendMessage(
        RequestHeader header,
        IRequestMessage requestMessage,
        ApiVersion messageVersion,
        ApiVersion headerVersion,
        RecyclableMemoryStreamManager streamManager)
    {
        _messageVersion = messageVersion;
        _headerVersion = headerVersion;
        _streamManager = streamManager;
        Header = header;
        RequestMessage = requestMessage;
    }

    public void Write(Stream writableStream, bool throwIfSizeLargeThen = false, int messageMaxBytes = 1000000)
    {
        using var stream = _streamManager.GetStream();
        var writer = new BufferWriter(stream);

        Header.Write(writer, _headerVersion);
        RequestMessage.Write(writer, _messageVersion);

        writer.End();

        if (stream.Length > messageMaxBytes && throwIfSizeLargeThen)
        {
            var logMessage = $"Размер запроса превышает допустимый предел указанный в конфигурации {messageMaxBytes}";

            throw new ProtocolKafkaException(ErrorCodes.MessageTooLarge, logMessage);
        }

        stream.WriteTo(writableStream);
    }
}