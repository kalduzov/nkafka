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

using NKafka.Protocol;
using NKafka.Protocol.Buffers;

namespace NKafka.Tests.Messages;

internal static class RequestMessageTests
{
    public static void SerializeAndDeserializeMessageTest<T>(this T message, ApiVersion version)
        where T : IMessage, new()
    {

        var arrayBuffer = new ArrayBuffer(true, false, 10000);
        var writer = new BufferWriter(ref arrayBuffer);
        message.Write(ref writer, version);

        var serializeMessage = arrayBuffer.DangerousGetFirstBuffer(); //Первые 4 байта - это длинна сообщения

        var reader = new BufferReader(serializeMessage);
        var deserializeMessage = new T();
        deserializeMessage.Read(ref reader, version);

        message.Should().BeEquivalentTo(deserializeMessage);
    }
}