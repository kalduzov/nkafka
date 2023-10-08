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

namespace NKafka.Tests.Protocol;

public sealed class BufferWriterTests
{
    private readonly BufferWriter _bw;
    private readonly MemoryStream _ms;

    public BufferWriterTests()
    {
        var buf = new byte[]
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10
        };
        _ms = new MemoryStream(buf);
        _bw = new BufferWriter(_ms, 0);
    }

    [Fact]
    public void ChangePosition()
    {
        _bw.Position = 5;
        var byteValue = _ms.ReadByte();
        byteValue.Should().Be(6);
        _bw.Position.Should().Be(6);
    }

    [Fact]
    public void LengthTest()
    {
        _bw.Length.Should().Be(10);
    }

    [Fact]
    public void RemainingTest()
    {
        _bw.Remaining.Should().Be(10);
        _bw.WriteByte(1);
        _bw.Remaining.Should().Be(9);
    }

    [Fact]
    public void WrittenCountTest()
    {
        _bw.WrittenCount.Should().Be(0);
        _bw.WriteByte(1);
        _bw.WrittenCount.Should().Be(1);
        _bw.WriteBytes(new byte[]
        {
            1,
            2,
            3
        });
        _bw.WrittenCount.Should().Be(4);
    }
}