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

using NKafka.Protocol.Buffers;

namespace NKafka.Protocol.Extensions;

internal static class VarIntExtensions
{
    #region Read

    internal static long ReadVarInt64(this ref BufferReader buffer)
    {
        var typeCode = buffer.ReadSByte();

        return typeCode switch
        {
            VarIntCodes.BYTE => buffer.ReadUnmanaged<byte>(),
            VarIntCodes.SBYTE => buffer.ReadUnmanaged<sbyte>(),
            VarIntCodes.UINT16 => buffer.ReadUnmanaged<ushort>(),
            VarIntCodes.INT16 => buffer.ReadUnmanaged<short>(),
            VarIntCodes.UINT32 => buffer.ReadUnmanaged<uint>(),
            VarIntCodes.INT32 => buffer.ReadUnmanaged<int>(),
            VarIntCodes.UINT64 => checked((long)buffer.ReadUnmanaged<ulong>()),
            VarIntCodes.INT64 => buffer.ReadUnmanaged<long>(),
            _ => typeCode
        };
    }

    internal static int ReadVarInt32(this ref BufferReader buffer)
    {
        var typeCode = buffer.ReadSByte();

        return typeCode switch
        {
            VarIntCodes.BYTE => buffer.ReadUnmanaged<byte>(),
            VarIntCodes.SBYTE => buffer.ReadUnmanaged<sbyte>(),
            VarIntCodes.UINT16 => buffer.ReadUnmanaged<ushort>(),
            VarIntCodes.INT16 => buffer.ReadUnmanaged<short>(),
            VarIntCodes.UINT32 => checked((int)buffer.ReadUnmanaged<uint>()),
            VarIntCodes.INT32 => buffer.ReadUnmanaged<int>(),
            VarIntCodes.UINT64 => checked((int)buffer.ReadUnmanaged<ulong>()),
            VarIntCodes.INT64 => checked((int)buffer.ReadUnmanaged<long>()),
            _ => typeCode
        };

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

    #endregion
}