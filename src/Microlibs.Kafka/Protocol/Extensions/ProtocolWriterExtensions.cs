using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microlibs.Kafka.Protocol.Extensions;

internal static class ProtocolWriterExtensions
{
    public static void WriteLength(this BinaryWriter writer, int value)
    {
        writer.Write(value.ToBigEndian());
    }

    public static void WriteHeader(this BinaryWriter writer, KafkaRequestHeader header)
    {
        writer.Write(header.ApiKey.AsShort());
        writer.Write(header.ApiVersion.ToBigEndian());
        writer.Write(header.CorrelationId.ToBigEndian());

        writer.Write(header.ClientId.AsNullableString()); //todo возможно стоит это кешировать?
    }

    public static void WriteMessage(this BinaryWriter writer, KafkaContent content)
    {
        writer.Write(content.AsReadOnlySpan());
    }

    private static ReadOnlySpan<byte> AsNullableString(this string str)
    {
        var buf = new byte[0x2 + str.Length];

        var len = ((short)str.Length).ToBigEndian();

        buf[0] = len[0];
        buf[1] = len[1];

        Array.Copy(Encoding.UTF8.GetBytes(str), 0, buf, 2, str.Length);

        return buf;
    }

    private static Span<byte> AsShort(this ApiKeys apiKey)
    {
        return ((short)apiKey).ToBigEndian();
    }

    internal static string ReadCompactString(this BinaryReader reader)
    {
        var len = reader.ReadByte();
        var buf = reader.ReadBytes(len - 1);
        return Encoding.UTF8.GetString(buf);
    }
    
    internal static string ReadCompactNullableString(this BinaryReader reader)
    {
        var len = reader.ReadInt16().Swap();

        if (len == 0)
        {
            return null!;
        }
        var buf = reader.ReadBytes(len - 1);
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

    private static Span<byte> ToBigEndian(this short value)
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
            bytes[0] = (byte)(value);
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