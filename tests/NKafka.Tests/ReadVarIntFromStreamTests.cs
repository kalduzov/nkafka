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

using NKafka.Protocol.Extensions;

namespace NKafka.Tests;

public class ReadVarIntFromStreamTests
{
    [Theory]
    [InlineData(0x1, 0x1, 1)]
    [InlineData(0x0, 0x0, 1)]
    [InlineData(0x12C, 0x2AC, 2)]
    public void WriteVarUInt64Test(ulong value, ulong calculateValue, int countBytes)
    {
        using var stream = new MemoryStream(5);

        var count = stream.WriteVarUInt64(value);

        count.Should().Be(countBytes);

        var array = stream.ToArray();
        Array.Resize(ref array, 8);
        var checkValue = BitConverter.ToUInt64(array);
        checkValue.Should().Be(calculateValue);
    }

    [Theory]
    [InlineData(0x0, 0x0, 1)]
    [InlineData(-0x1, 0x1, 1)]
    [InlineData(0x1, 0x2, 1)]
    [InlineData(-0x2, 0x3, 1)]
    public void WriteVarInt64Test(long value, ulong calculateValue, int countBytes)
    {
        using var stream = new MemoryStream(5);

        var count = stream.WriteVarInt64(value);

        count.Should().Be(countBytes);

        var array = stream.ToArray();
        Array.Resize(ref array, 8);
        var checkValue = BitConverter.ToUInt64(array);
        checkValue.Should().Be(calculateValue);
    }
}