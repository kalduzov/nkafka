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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using static System.Buffers.Binary.BinaryPrimitives;

namespace NKafka.Protocol.Extensions;

internal static class ProtocolWriterExtensions
{
    private static readonly Dictionary<string, byte[]> _clientIdCache = new(1);

    public static void WriteLength(this Stream writer, int value)
    {
        writer.Write(value.ToBigEndian());
    }

    public static byte[] AsNullableString(this string str)
    {
        var buf = new byte[0x2 + str.Length];

        var len = ((short)str.Length).ToBigEndian();

        buf[0] = len[0];
        buf[1] = len[1];

        Array.Copy(Encoding.UTF8.GetBytes(str), 0, buf, 2, str.Length);

        return buf;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte AsByte(this bool value)
    {
        return value ? (byte)1 : (byte)00;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Span<byte> AsShort(this ApiKeys apiKey)
    {
        return ((short)apiKey).ToBigEndian();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Span<byte> AsShort(this ApiVersion version)
    {
        return ((short)version).ToBigEndian();
    }

    public static Span<byte> ToBigEndian(this short value)
    {
        Span<byte> destination = stackalloc byte[sizeof(short)];

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        MemoryMarshal.Write(destination, ref value);

        return destination.ToArray();
    }

    public static Span<byte> ToBigEndian(this ushort value)
    {
        Span<byte> destination = stackalloc byte[sizeof(ushort)];

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        MemoryMarshal.Write(destination, ref value);

        return destination.ToArray();
    }

    internal static Span<byte> ToBigEndian(this uint value)
    {
        Span<byte> destination = stackalloc byte[sizeof(uint)];

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        MemoryMarshal.Write(destination, ref value);

        return destination.ToArray();
    }

    internal static ReadOnlySpan<byte> ToBigEndian(this int value)
    {
        Span<byte> destination = stackalloc byte[sizeof(int)];

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        MemoryMarshal.Write(destination, ref value);

        return destination.ToArray();
    }

    internal static Span<byte> ToBigEndian(this long value)
    {
        Span<byte> destination = stackalloc byte[sizeof(long)];

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        MemoryMarshal.Write(destination, ref value);

        return destination.ToArray();
    }

    internal static Span<byte> ToBigEndian(this ulong value)
    {
        Span<byte> destination = stackalloc byte[sizeof(ulong)];

        if (BitConverter.IsLittleEndian)
        {
            value = ReverseEndianness(value);
        }

        MemoryMarshal.Write(destination, ref value);

        return destination.ToArray();
    }
}