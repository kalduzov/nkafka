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

using System.Numerics;
using System.Text;

using NKafka.Protocol.Buffers;

namespace NKafka.Protocol.Extensions;

internal static class VarIntExtensions
{
    #region Read

    internal static long ReadVarInt64(this ref BufferReader buffer)
    {
        var value = ReadVarUInt64(ref buffer);

        return value >>> 1 ^ -(value & 1);
    }

    internal static int ReadVarInt32(this ref BufferReader buffer)
    {
        var value = ReadVarUInt32(ref buffer);

        return value >>> 1 ^ -(value & 1);
    }

    internal static long ReadVarUInt64(this ref BufferReader buffer)
    {
        var value = 0L;
        var i = 0;
        long b;

        while (((b = buffer.ReadSByte()) & 0x80) != 0)
        {
            value |= (b & 0x7F) << i;
            i += 7;

            if (i > 63)
            {
                throw new ArgumentException("VarUInt64 is too long");
            }
        }

        value |= b << i;

        return value;
    }

    internal static int ReadVarUInt32(this ref BufferReader buffer)
    {
        var tmp = buffer.ReadSByte();

        if (tmp >= 0)
        {
            return tmp;
        }

        var result = tmp & 127;

        if ((tmp = buffer.ReadSByte()) >= 0)
        {
            result |= tmp << 7;
        }
        else
        {
            result |= (tmp & 127) << 7;

            if ((tmp = buffer.ReadSByte()) >= 0)
            {
                result |= tmp << 14;
            }
            else
            {
                result |= (tmp & 127) << 14;

                if ((tmp = buffer.ReadSByte()) >= 0)
                {
                    result |= tmp << 21;
                }
                else
                {
                    result |= (tmp & 127) << 21;
                    result |= (tmp = buffer.ReadSByte()) << 28;

                    if (tmp < 0)
                    {
                        throw new ArgumentException("VarUInt32 is too long");
                    }
                }
            }
        }

        return result;
    }

    #endregion

    #region Write

    /// <summary>
    /// Write long value as varints to buffer 
    /// </summary>
    internal static void WriteVarInt64(this ref BufferWriter buffer, long value)
    {
        var v = value << 1 ^ value >> 63;

        while ((v & unchecked((long)0xffffffffffffff80L)) != 0L)
        {
            buffer.WriteByte((byte)((int)v & 0x7f | 0x80));
            v >>= 7;
        }
        buffer.WriteByte((byte)v);
    }

    /// <summary>
    /// Write int value as varints to buffer
    /// </summary>
    internal static void WriteVarInt32(this ref BufferWriter buffer, int value)
    {
        WriteVarUInt32(ref buffer, value << 1 ^ value >> 31);
    }

    internal static void WriteVarUInt32(this ref BufferWriter buffer, int value)
    {
        if ((value & 0xFFFFFFFF << 7) == 0)
        {
            buffer.WriteByte((byte)value);
        }
        else
        {
            buffer.WriteByte((byte)(value & 0xFF | 0x80));

            if ((value & 0xFFFFFFFF << 14) == 0)
            {
                buffer.WriteByte((byte)(value >> 7));
            }
            else
            {
                buffer.WriteByte((byte)((value >> 7) & 0xFF | 0x80));

                if ((value & 0xFFFFFFFF << 21) == 0)
                {
                    buffer.WriteByte((byte)(value >> 14));
                }
                else
                {
                    buffer.WriteByte((byte)((value >> 14) & 0xFF | 0x80));

                    if ((value & 0xFFFFFFFF << 28) == 0)
                    {
                        buffer.WriteByte((byte)(value >> 21));
                    }
                    else
                    {
                        buffer.WriteByte((byte)((value >> 21) & 0xFF | 0x80));
                        buffer.WriteByte((byte)(value >> 28));
                    }
                }
            }
        }
    }

    public static void WriteNullVarInt(this ref BufferWriter buffer)
    {
        buffer.WriteVarInt32(-1);
    }

    public static void WriteBytesWithLength(this ref BufferWriter buffer, byte[] value)
    {
        buffer.WriteVarInt32(value.Length);
        buffer.WriteBytes(value);
    }

    internal static int SizeOfVarInt(this int value)
    {
        return SizeOfVarUInt(value << 1 ^ value >> 31);
    }

    internal static int SizeOfVarUInt(this int value)
    {
        var leadingZeros = BitOperations.LeadingZeroCount((uint)value);
        var leadingZerosBelow38DividedBy7 = (38 - leadingZeros) * 0b10010010010010011 >> 19;

        return leadingZerosBelow38DividedBy7 + (leadingZeros >> 5);
    }

    internal static int SizeOfVarLong(this long value)
    {
        var v = value << 1 ^ value >> 63;
        var leadingZeros = BitOperations.LeadingZeroCount((ulong)v);
        var leadingZerosBelow70DividedBy7 = (70 - leadingZeros) * 0b10010010010010011 >> 19;

        return leadingZerosBelow70DividedBy7 + (leadingZeros >> 6);
    }

    /// <summary>
    /// Returns the size of a Kafka compact string encoded as VarUInt(length + 1) followed by UTF-8 bytes.
    /// This matches the payload written by <see cref="BufferWriter.WriteCompactString"/>.
    /// </summary>
    internal static int SizeOfCompactString(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var byteCount = Encoding.UTF8.GetByteCount(value);

        return (byteCount + 1).SizeOfVarUInt() + byteCount;
    }

    /// <summary>
    /// Returns the size of a nullable Kafka compact string encoded as VarUInt(length + 1) followed by UTF-8 bytes.
    /// A null value is represented by the Kafka compact-string sentinel length <c>0</c>.
    /// This matches the payload written by <see cref="BufferWriter.WriteNullableCompactString"/>.
    /// </summary>
    internal static int SizeOfNullableCompactString(this string? value)
    {
        return value is null ? 1 : value.SizeOfCompactString();
    }

    #endregion
}
