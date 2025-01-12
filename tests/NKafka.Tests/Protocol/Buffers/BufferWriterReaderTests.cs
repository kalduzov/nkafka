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

using NKafka.Protocol.Buffers;
using NKafka.Protocol.Extensions;

namespace NKafka.Tests.Protocol.Buffers;

public class BufferWriterReaderTests
{
    [Fact]
    public void WriteStringTests()
    {
        var arrayBuffer = new ArrayBuffer(true, false, 7);
        var bw = new BufferWriter(ref arrayBuffer);
        bw.WriteString("range");
        bw.Flush();

        arrayBuffer.DangerousGetFirstBuffer()
            .SequenceEqual(new byte[]
            {
                0x00,
                0x05,
                0x72,
                0x61,
                0x6e,
                0x67,
                0x65
            })
            .Should()
            .BeTrue();
    }

    [Fact]
    public void WriteZeroByteTest()
    {
        var arrayBuffer = new ArrayBuffer(true, false, 1);
        var bw = new BufferWriter(ref arrayBuffer);
        bw.WriteZeroBytes<byte>();
        bw.Flush();
        arrayBuffer.DangerousGetFirstBuffer()[0].Should().Be(0);
    }

    [Fact]
    public void WriteByteTest()
    {
        var arrayBuffer = new ArrayBuffer(true, false, sizeof(byte));
        var bw = new BufferWriter(ref arrayBuffer);
        bw.WriteByte(1);
        bw.Flush();
        arrayBuffer.DangerousGetFirstBuffer()[0].Should().Be(1);
    }

    [Fact]
    public void WriteSByteTest()
    {
        var arrayBuffer = new ArrayBuffer(true, false, sizeof(sbyte));
        var bw = new BufferWriter(ref arrayBuffer);
        bw.WriteSByte(-1);
        bw.Flush();
        ((sbyte)arrayBuffer.DangerousGetFirstBuffer()[0]).Should().Be(-1);
    }

    [Fact]
    public void WriteShortTest()
    {
        var arrayBuffer = new ArrayBuffer(true, false, sizeof(short));
        var bw = new BufferWriter(ref arrayBuffer);
        bw.WriteShort(short.MaxValue);
        bw.Flush();
        arrayBuffer.DangerousGetFirstBuffer()
            .SequenceEqual(new byte[]
            {
                0x7f,
                0xff
            })
            .Should()
            .BeTrue();
    }

    [Fact]
    public void WriteUShortTest()
    {
        var arrayBuffer = new ArrayBuffer(true, false, sizeof(ushort));
        var bw = new BufferWriter(ref arrayBuffer);
        bw.WriteUShort(45000);
        bw.Flush();
        arrayBuffer.DangerousGetFirstBuffer()
            .SequenceEqual(new byte[]
            {
                0xaf,
                0xc8
            })
            .Should()
            .BeTrue();
    }

    [Fact]
    public void WriteIntTest()
    {
        var arrayBuffer = new ArrayBuffer(true, false, sizeof(int));
        var bw = new BufferWriter(ref arrayBuffer);
        bw.WriteInt(300000);
        bw.Flush();
        arrayBuffer.DangerousGetFirstBuffer()
            .SequenceEqual(new byte[]
            {
                0x00,
                0x04,
                0x93,
                0xe0
            })
            .Should()
            .BeTrue();
    }

    [Theory(DisplayName = "Write signed long as varlong")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(-2)]
    [InlineData(4)]
    [InlineData(23)]
    [InlineData(253)]
    [InlineData(-500)]
    [InlineData(0x7fffffff)]
    [InlineData(-0x80000000)]
    public void ReadVarLong_FromSimpleBuffer_Successful(long original)
    {
        var buffer = new ArrayBuffer(true, false, sizeof(long));
        var bw = new BufferWriter(ref buffer);

        bw.WriteVarInt64(original);

        var br = new BufferReader(buffer.DangerousGetFirstBuffer());

        var actual = br.ReadVarInt64();

        actual.Should().Be(original);
    }
}