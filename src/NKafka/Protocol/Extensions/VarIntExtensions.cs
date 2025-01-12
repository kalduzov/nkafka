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
        switch (value)
        {
            case >= 0 and <= VarIntCodes.MAX_SINGLE_VALUE:
                buffer.WriteUnmanaged((sbyte)value);

                break;
            case >= 0 and <= short.MaxValue:
                buffer.WriteUnmanaged(VarIntCodes.INT16, (short)value);

                break;
            case >= 0 and <= int.MaxValue:
                buffer.WriteUnmanaged(VarIntCodes.INT32, (int)value);

                break;
            case >= 0:
                buffer.WriteUnmanaged(VarIntCodes.INT64, value);

                break;
            case >= VarIntCodes.MIN_SINGLE_VALUE:
                buffer.WriteUnmanaged((sbyte)value);

                break;
            case >= sbyte.MinValue:
                buffer.WriteUnmanaged(VarIntCodes.SBYTE, (sbyte)value);

                break;
            case >= short.MinValue:
                buffer.WriteUnmanaged(VarIntCodes.INT16, (short)value);

                break;
            case >= int.MinValue:
                buffer.WriteUnmanaged(VarIntCodes.INT32, (int)value);

                break;
            default:
                buffer.WriteUnmanaged(VarIntCodes.INT64, value);

                break;
        }
    }

    /// <summary>
    /// Write int value as varints to buffer
    /// </summary>
    internal static void WriteVarInt32(this ref BufferWriter buffer, int value)
    {
        switch (value)
        {
            // same as sbyte.MaxValue
            case >= 0 and <= VarIntCodes.MAX_SINGLE_VALUE:
                buffer.WriteUnmanaged((sbyte)value);

                break;
            case >= 0 and <= short.MaxValue:
                buffer.WriteUnmanaged(VarIntCodes.INT16, (short)value);

                break;
            case >= 0:
                buffer.WriteUnmanaged(VarIntCodes.INT32, (int)value);

                break;
            case >= VarIntCodes.MIN_SINGLE_VALUE:
                buffer.WriteUnmanaged((sbyte)value);

                break;
            case >= sbyte.MinValue:
                buffer.WriteUnmanaged(VarIntCodes.SBYTE, (sbyte)value);

                break;
            case >= short.MinValue:
                buffer.WriteUnmanaged(VarIntCodes.INT16, (short)value);

                break;
            default:
                buffer.WriteUnmanaged(VarIntCodes.INT32, value);

                break;
        }
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