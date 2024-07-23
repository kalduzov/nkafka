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

using System.Runtime.CompilerServices;

namespace NKafka.Protocol.Buffers;

using static System.Buffers.Binary.BinaryPrimitives;

public ref partial struct BufferReader
{
    /// <summary>
    /// Read int from buffer 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt()
    {
        var value = ReadUnmanaged<int>();

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        return value;
    }

    /// <summary>
    /// Read uint from buffer 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt()
    {
        var value = ReadUnmanaged<uint>();

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        return value;
    }

    /// <summary>
    /// Read byte from buffer 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        var value = ReadUnmanaged<byte>();

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        return value;
    }

    /// <summary>
    /// Read sbyte from buffer 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByte()
    {
        var value = ReadUnmanaged<sbyte>();

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        return value;
    }

    /// <summary>
    /// Read short from buffer 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadShort()
    {
        var value = ReadUnmanaged<short>();

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        return value;
    }

    /// <summary>
    /// Read ushort from buffer 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUShort()
    {
        var value = ReadUnmanaged<ushort>();

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        return value;
    }

    /// <summary>
    /// Read long from buffer 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadLong()
    {
        var value = ReadUnmanaged<long>();

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        return value;
    }

    /// <summary>
    /// Read ulong from buffer 
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadULong()
    {
        var value = ReadUnmanaged<ulong>();

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        return value;
    }
}