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

namespace NKafka.Tests.Protocol.Buffers;

public class BufferReaderTests
{
    [Theory(DisplayName = "Read varlong from simple buffer test ")]
    [InlineData(0, 0)]
    [InlineData(1, -1)]
    [InlineData(2, 1)]
    [InlineData(3, -2)]
    [InlineData(8, 4)]
    public void ReadVarLong_FromSimpleBuffer_Successful(long variant, long testValue)
    {
        var data = BitConverter.GetBytes(variant);
        var variantBuffer = GetVariantBuffer(data);
        var reader = new BufferReader(variantBuffer);

        var value = reader.ReadVarLong();

        value.Should().Be(testValue);
    }

    [Theory(DisplayName = "Read varlong from simple buffer test ")]
    [InlineData(0, 0)]
    [InlineData(1, -1)]
    [InlineData(2, 1)]
    [InlineData(3, -2)]
    [InlineData(8, 4)]
    public void ReadVarInt_FromSimpleBuffer_Successful(int variant, int testValue)
    {
        var data = BitConverter.GetBytes(variant);
        var variantBuffer = GetVariantBuffer(data);
        var reader = new BufferReader(variantBuffer);

        var value = reader.ReadVarInt();

        value.Should().Be(testValue);
    }

    private static byte[] GetVariantBuffer(byte[] data)
    {
        Array.Reverse(data);

        for (var i = 0; i < data.Length; i++)
        {
            if (data[i] != 0)
            {
                return data[i..];
            }
        }

        return new byte[]
        {
            0
        };
    }
}