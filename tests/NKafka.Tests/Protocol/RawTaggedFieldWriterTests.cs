// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2025 Aleksey Kalduzov. All rights reserved
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

namespace NKafka.Tests.Protocol;

public class RawTaggedFieldWriterTests
{
    [Fact]
    public void WritingZeroRawTaggedFieldsTest()
    {
        var writer = RawTaggedFieldWriter.ForFields(null);
        writer.FieldsCount.Should().Be(0);
        var arrayBuffer = new ArrayBuffer(true, false, 0);
        var bufferWriter = new BufferWriter(ref arrayBuffer);
        writer.WriteRawTags(ref bufferWriter, int.MaxValue);
        arrayBuffer.TotalWritten.Should().Be(0);
    }

    [Fact]
    public void WritingSeveralRawTaggedFieldsTest()
    {
        List<TaggedField> tags =
        [
            new(2, [0x1, 0x2, 0x3]),
            new(5, [0x4, 0x5])
        ];

        var writer = RawTaggedFieldWriter.ForFields(tags);
        writer.FieldsCount.Should().Be(2);

        var arrayBuffer = new ArrayBuffer(true, false, 9);
        var bufferWriter = new BufferWriter(ref arrayBuffer);
        writer.WriteRawTags(ref bufferWriter, 1);
        arrayBuffer.DangerousGetFirstBuffer().Should().BeEquivalentTo([0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0]);

        writer.WriteRawTags(ref bufferWriter, 3);
        arrayBuffer.DangerousGetFirstBuffer().Should().BeEquivalentTo([0x2, 0x3, 0x1, 0x2, 0x3, 0x0, 0x0, 0x0, 0x0]);

        writer.WriteRawTags(ref bufferWriter, 7);
        arrayBuffer.DangerousGetFirstBuffer().Should().BeEquivalentTo([0x2, 0x3, 0x1, 0x2, 0x3, 0x5, 0x2, 0x4, 0x5]);

        writer.WriteRawTags(ref bufferWriter, int.MaxValue);
        arrayBuffer.DangerousGetFirstBuffer().Should().BeEquivalentTo([0x2, 0x3, 0x1, 0x2, 0x3, 0x5, 0x2, 0x4, 0x5]);
    }

    [Fact]
    public void InvalidNextDefinedTagTest()
    {
        List<TaggedField> tags =
        [
            new(2, [0x1, 0x2, 0x3]),
            new(5, [0x4, 0x5, 0x6]),
            new(7, [0x0])
        ];

        var writer = RawTaggedFieldWriter.ForFields(tags);
        writer.FieldsCount.Should().Be(3);

        var arrayBuffer = new ArrayBuffer(true, false, 1024);
        var bufferWriter = new BufferWriter(ref arrayBuffer);

        try
        {
            writer.WriteRawTags(ref bufferWriter, 2);
        }
        catch (Exception exception)
        {
            exception.Message.Should().Be("Attempted to use tag 2 as an undefined tag.");
        }

    }

    [Fact]
    public void OutOfOrderTagsTest()
    {
        List<TaggedField> tags =
        [
            new(5, [0x4, 0x5, 0x6]),
            new(2, [0x1, 0x2, 0x3]),
            new(7, [0x0])
        ];

        var writer = RawTaggedFieldWriter.ForFields(tags);
        writer.FieldsCount.Should().Be(3);

        var arrayBuffer = new ArrayBuffer(true, false, 1024);
        var bufferWriter = new BufferWriter(ref arrayBuffer);

        try
        {
            writer.WriteRawTags(ref bufferWriter, 8);
        }
        catch (Exception exception)
        {
            exception.Message.Should().Be("Invalid raw tag field list: tag 2 comes after tag 5, but is not higher than it.");
        }
    }
}