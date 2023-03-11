//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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
using NKafka.Protocol.Records;

using Record = NKafka.Protocol.Records.Record;

namespace NKafka.Tests.Protocol.Records;

public class RecordExtensionsTests
{
    private readonly byte[] _validRecord =
    {
        0x14,
        0x00,
        0x00,
        0x00,
        0x01,
        0x08,
        0x74,
        0x65,
        0x73,
        0x74,
        0x00
    };

    [Fact]
    public void WriteToTest()
    {
        var record = new Record
        {
            Headers = Headers.Empty,
            Key = null,
            Value = "test"u8.ToArray()
        };

        var buffer = new byte[_validRecord.Length];
        using var ms = new MemoryStream(buffer);
        var bufferWriter = new BufferWriter(ms, 0);
        record.WriteTo(bufferWriter);

        buffer.Should().BeEquivalentTo(_validRecord);
    }
}