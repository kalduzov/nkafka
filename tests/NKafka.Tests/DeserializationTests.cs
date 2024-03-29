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

namespace NKafka.Tests;

public class DeserializationTests
{
    [Fact]
    public void ApiVersionsResponseMessageTest()
    {
        var data = new byte[]
        {
            0x00,
            0x00,
            0x01,
            0x5E,
            0x00,
            0x00,
            0x00,
            0x01,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x38,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x09,
            0x00,
            0x01,
            0x00,
            0x00,
            0x00,
            0x0C,
            0x00,
            0x02,
            0x00,
            0x00,
            0x00,
            0x06,
            0x00,
            0x03,
            0x00,
            0x00,
            0x00,
            0x0B,
            0x00,
            0x04,
            0x00,
            0x00,
            0x00,
            0x05,
            0x00,
            0x05,
            0x00,
            0x00,
            0x00,
            0x03,
            0x00,
            0x06,
            0x00,
            0x00,
            0x00,
            0x07,
            0x00,
            0x07,
            0x00,
            0x00,
            0x00,
            0x03,
            0x00,
            0x08,
            0x00,
            0x00,
            0x00,
            0x08,
            0x00,
            0x09,
            0x00,
            0x00,
            0x00,
            0x07,
            0x00,
            0x0A,
            0x00,
            0x00,
            0x00,
            0x03,
            0x00,
            0x0B,
            0x00,
            0x00,
            0x00,
            0x07,
            0x00,
            0x0C,
            0x00,
            0x00,
            0x00,
            0x04,
            0x00,
            0x0D,
            0x00,
            0x00,
            0x00,
            0x04,
            0x00,
            0x0E,
            0x00,
            0x00,
            0x00,
            0x05,
            0x00,
            0x0F,
            0x00,
            0x00,
            0x00,
            0x05,
            0x00,
            0x10,
            0x00,
            0x00,
            0x00,
            0x04,
            0x00,
            0x11,
            0x00,
            0x00,
            0x00,
            0x01,
            0x00,
            0x12,
            0x00,
            0x00,
            0x00,
            0x03,
            0x00,
            0x13,
            0x00,
            0x00,
            0x00,
            0x07,
            0x00,
            0x14,
            0x00,
            0x00,
            0x00,
            0x06,
            0x00,
            0x15,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x16,
            0x00,
            0x00,
            0x00,
            0x04,
            0x00,
            0x17,
            0x00,
            0x00,
            0x00,
            0x04,
            0x00,
            0x18,
            0x00,
            0x00,
            0x00,
            0x03,
            0x00,
            0x19,
            0x00,
            0x00,
            0x00,
            0x03,
            0x00,
            0x1A,
            0x00,
            0x00,
            0x00,
            0x03,
            0x00,
            0x1B,
            0x00,
            0x00,
            0x00,
            0x01,
            0x00,
            0x1C,
            0x00,
            0x00,
            0x00,
            0x03,
            0x00,
            0x1D,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x1E,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x1F,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x20,
            0x00,
            0x00,
            0x00,
            0x04,
            0x00,
            0x21,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x22,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x23,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x24,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x25,
            0x00,
            0x00,
            0x00,
            0x03,
            0x00,
            0x26,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x27,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x28,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x29,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x2A,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x2B,
            0x00,
            0x00,
            0x00,
            0x02,
            0x00,
            0x2C,
            0x00,
            0x00,
            0x00,
            0x01,
            0x00,
            0x2D,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x2E,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x2F,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x30,
            0x00,
            0x00,
            0x00,
            0x01,
            0x00,
            0x31,
            0x00,
            0x00,
            0x00,
            0x01,
            0x00,
            0x32,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x33,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x38,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x39,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x3C,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x3D,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00,
            0x00
        };

        var bufferReader = new BufferReader(data);

        var len = bufferReader.ReadInt(); // длинна сообщения
        len.Should().Be(350);

        var header = new ResponseHeader(ref bufferReader, ApiVersion.Version0);
        header.CorrelationId.Should().Be(1);

        var response = new ApiVersionsResponseMessage(ref bufferReader, ApiVersion.Version0);
        response.ApiKeys.Count.Should().Be(56);
    }
}