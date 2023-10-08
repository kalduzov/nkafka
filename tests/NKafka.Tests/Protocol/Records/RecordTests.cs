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
 *     https://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using NKafka.Protocol.Buffers;

using Record = NKafka.Protocol.Records.Record;

namespace NKafka.Tests.Protocol.Records;

public class RecordTests
{
    [Fact]
    public void CtorRecord_FromBytes_Successful()
    {
        var buffer = new byte[]
        {
            0x14,
            0x00,
            0x00,
            0x00,
            0x00,
            0x08,
            0x00,
            0x00,
            0x00,
            0x01,
            0x00
        };
        var bufferReader = new BufferReader(buffer);

        var record = new Record(ref bufferReader);

        record.Attributes.Should().Be(0);
        record.Headers.Count().Should().Be(0);
        record.Length.Should().Be(10);
        record.Value!.Length.Should().Be(4);
        record.Key.Should().BeNull();
        record.OffsetDelta.Should().Be(0);
        record.TimestampDelta.Should().Be(0);
    }
}