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

using FluentAssertions;

using NKafka.Messages;
using NKafka.Protocol;

using Xunit;

namespace NKafka.Tests;

public class SerializationTests
{
    [Fact]
    public void ApiVersionsRequestMessageTest()
    {
        var request = new ApiVersionsRequestMessage
        {
            ClientSoftwareName = "test"
        };

        var header = new RequestHeader
        {
            RequestApiKey = (short)request.ApiKey,
            RequestApiVersion = (short)ApiVersions.Version1,
            ClientId = "test",
            CorrelationId = 1,
        };

        var hashCode = header.GetHashCode();
        
        var header1 = new RequestHeader
        {
            RequestApiKey = (short)request.ApiKey,
            RequestApiVersion = (short)ApiVersions.Version1,
            ClientId = "test",
            CorrelationId = 2,
        };

        var hashCode2 = header1.GetHashCode();
        
        var sendMessage = new SendMessage(header, request);

        using var stream = new MemoryStream();

        sendMessage.Write(stream);

        var str = Convert.ToHexString(stream.ToArray());
        stream.Length.Should().Be(18);
    }
}