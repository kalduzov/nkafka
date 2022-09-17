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

using System.IO;
using System.Numerics;
using System.Text;

namespace NKafka.Protocol.Extensions;

internal static class VarIntExtensions
{
    internal static string ReadCompactString(this BinaryReader reader)
    {
        var len = ReadVarInt(reader);
        var buf = reader.ReadBytes(len);

        return Encoding.UTF8.GetString(buf);
    }

    /// <summary>
    /// Write ulong value as varints to stream 
    /// </summary>
    internal static int WriteVarUInt64(this Stream stream, ulong value)
    {
        var count = 0;

        byte b;

        do
        {
            b = (byte)((value & 0x7F) | 0x80);
            stream.WriteByte(b);
            count++;
        } while ((value >>= 7) != 0);

        stream.Position--;
        b &= 0x7F;
        stream.WriteByte(b);

        return count;
    }

    /// <summary>
    /// Write long value as varints to stream 
    /// </summary>
    internal static int WriteVarInt64(this Stream stream, long value)
    {
        var ux = (ulong)value << 1;

        if (value < 0)
        {
            ux = ~ux;
        }

        return WriteVarUInt64(stream, ux);
    }

    /// <summary>
    /// Write int value as varints to stream
    /// </summary>
    internal static int WriteVarInt(this Stream stream, int value)
    {
        var ux = (ulong)value << 1;

        if (value < 0)
        {
            ux = ~ux;
        }

        return WriteVarUInt64(stream, ux);
    }

    /// <summary>
    /// Write uint value as varints to stream 
    /// </summary>
    internal static int WriteVarUInt(this Stream stream, uint value)
    {
        return WriteVarUInt64(stream, value);
    }

    private static int ReadVarInt(BinaryReader reader)
    {
        var i = 0;
        var value = 0;
        var shift = 0;

        while (true)
        {
            var b = reader.ReadByte();

            if (b < 0x80)
            {
                if (i >= 5 || (i == 4 && b > 1))
                {
                    return -(i + 1);
                }

                return i + 1;
            }

            value |= (b & 0x7F) << shift;
            shift += 7;
            i++;
        }
    }

    private static int ReadVarUInt64(BinaryReader reader)
    {
        var i = 0;
        ulong value = 0U;
        var shift = 0;

        while (true)
        {
            var b = reader.ReadByte();

            if (b < 0x80)
            {
                if (i >= 5 || (i == 4 && b > 1))
                {
                    return -(i + 1);
                }

                return i + 1;
            }

            value |= (ulong)(b & 0x7F) << shift;
            shift += 7;
            i++;
        }
    }

    internal static int SizeOfVarUInt(this int value)
    {
        var leadingZeros = BitOperations.LeadingZeroCount((uint)value);
        var leadingZerosBelow38DividedBy7 = ((38 - leadingZeros) * 0b10010010010010011) >> 19;

        return leadingZerosBelow38DividedBy7 + (leadingZeros >> 5);
    }
}