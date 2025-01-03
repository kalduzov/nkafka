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

using System.Buffers;

using Microsoft.IO;

using NKafka.Exceptions;
using NKafka.Messages;
using NKafka.Protocol.Buffers;

namespace NKafka.Protocol;

/// <summary>
/// 
/// </summary>
internal struct SendMessage(
    RequestHeader header,
    IRequestMessage requestMessage,
    ApiVersion messageVersion,
    ApiVersion headerVersion,
    ArrayBuffer buffer)
{
    private ArrayBuffer _buffer = buffer;

    /// <summary>
    /// 
    /// </summary>
    public RequestHeader Header { get; } = header;

    /// <summary>
    /// 
    /// </summary>
    public IRequestMessage RequestMessage { get; } = requestMessage;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writableStream"></param>
    /// <param name="throwIfSizeLargeThen"></param>
    /// <param name="messageMaxBytes"></param>
    /// <returns></returns>
    /// <exception cref="ProtocolKafkaException"></exception>
    public async Task<long> Write(Stream writableStream, bool throwIfSizeLargeThen = false, int messageMaxBytes = 1000000)
    {
        var writer = new BufferWriter(ref _buffer);

        Header.Write(ref writer, headerVersion);
        RequestMessage.Write(ref writer, messageVersion);

        writer.WriteSizeToStart();

        if (writer.BufferLength > messageMaxBytes && throwIfSizeLargeThen)
        {
            var logMessage = $"Размер запроса превышает допустимый предел указанный в конфигурации {messageMaxBytes}";

            throw new ProtocolKafkaException(ErrorCodes.MessageTooLarge, logMessage);
        }

        await _buffer.WriteToAndResetAsync(writableStream, CancellationToken.None);

        return _buffer.TotalWritten;
    }
}