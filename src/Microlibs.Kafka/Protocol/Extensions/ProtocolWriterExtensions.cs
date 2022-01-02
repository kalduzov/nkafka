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
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microlibs.Kafka.Protocol.Extensions;

internal static class ProtocolWriterExtensions
{
    public static void WriteLength(this Stream writer, int value)
    {
        writer.Write(value.ToBigEndian());
    }

    public static void WriteHeader(this Stream writer, KafkaRequestHeader header)
    {
        writer.Write(header.ApiKey.AsShort());
        writer.Write(((short)header.ApiVersion).ToBigEndian());
        writer.Write(header.CorrelationId.ToBigEndian());

        writer.Write(header.ClientId.AsNullableString()); //todo возможно стоит это кешировать?
    }

    public static void WriteMessage(this Stream writer, RequestBody content)
    {
        content.SerializeToStream(writer);
    }

    public static ReadOnlySpan<byte> AsNullableString(this string str)
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

    internal static string ReadCompactString(this BinaryReader reader)
    {
        var len = reader.ReadByte();
        var buf = reader.ReadBytes(len);

        return Encoding.UTF8.GetString(buf);
    }

    internal static string ReadNormalString(this BinaryReader reader)
    {
        var len = reader.ReadInt16().Swap();
        var buf = reader.ReadBytes(len);

        return Encoding.UTF8.GetString(buf);
    }

    internal static string ReadCompactNullableString(this BinaryReader reader)
    {
        var len = reader.ReadInt16().Swap();

        if (len <= 0)
        {
            return null!;
        }

        var buf = reader.ReadBytes(len);

        return Encoding.UTF8.GetString(buf);
    }

    internal static Span<byte> ToBigEndian(this int value)
    {
        Span<byte> bytes = stackalloc byte[4];

        if (BitConverter.IsLittleEndian)
        {
            bytes[0] = (byte)(value >> 24);
            bytes[1] = (byte)(value >> 16);
            bytes[2] = (byte)(value >> 8);
            bytes[3] = (byte)value;
        }
        else
        {
            bytes[3] = (byte)(value >> 24);
            bytes[2] = (byte)(value >> 16);
            bytes[1] = (byte)(value >> 8);
            bytes[0] = (byte)value;
        }

        return bytes.ToArray();
    }

    public static Span<byte> ToBigEndian(this short value)
    {
        Span<byte> bytes = stackalloc byte[2];

        if (BitConverter.IsLittleEndian)
        {
            bytes[0] = (byte)(value >> 8);
            bytes[1] = (byte)value;
        }
        else
        {
            bytes[1] = (byte)(value >> 8);
            bytes[0] = (byte)value;
        }

        return bytes.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Swap(this uint value)
    {
        var x = (value >> 24) & 0x000000FF;
        var y = (value >> 8) & 0x0000FF00;
        var z = (value << 8) & 0x00FF0000;
        var w = (value << 24) & 0xFF000000;

        return x | y | z | w;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static short Swap(this short value)
    {
        return (short)Swap((ushort)value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort Swap(this ushort value)
    {
        var x = (ushort)((value >> 8) & 0x00FF);
        var y = (ushort)((value << 8) & 0xFF00);

        return (ushort)(x | y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Swap(this int value)
    {
        return (int)Swap((uint)value);
    }
}