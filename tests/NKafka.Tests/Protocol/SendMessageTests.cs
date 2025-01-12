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

using NKafka.Messages;
using NKafka.Protocol;
using NKafka.Protocol.Buffers;

namespace NKafka.Tests.Protocol;

public class SendMessageTests
{
    [Fact]
    public async Task SendMessageTest()
    {
        var request = new ApiVersionsRequestMessage
        {
            ClientSoftwareName = "test"
        };

        var header = new RequestHeader
        {
            RequestApiKey = (short)request.ApiKey,
            RequestApiVersion = (short)ApiVersion.Version1,
            ClientId = "test",
            CorrelationId = 1
        };

        var buffer = ArrayBufferPool.Rent(10000);

        try
        {
            var sendMessage = new SendMessage(header, request, ApiVersion.Version1, ApiVersion.Version1, buffer);

            using var stream = new MemoryStream();

            await sendMessage.WriteToStream(stream);

            stream.Length.Should().Be(18); //Всего байт данных в запросе
            stream.Seek(3, SeekOrigin.Begin);
            stream.ReadByte().Should().Be(14); //Длина собственно данных запроса, без учета первых 4х байтов

        }
        finally
        {
            ArrayBufferPool.Return(buffer);
        }
    }
}