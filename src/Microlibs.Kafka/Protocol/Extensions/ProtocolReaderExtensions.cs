using System;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace Microlibs.Kafka.Protocol.Extensions;

internal static class ProtocolReaderExtensions
{
    internal static ReadOnlySpan<byte> ReadNullableString(this ReadOnlySpan<byte> span, out string value)
    {
        var nextSpan = span.ReadInt16(out var stringLength);

        if (stringLength == -1)
        {
            value = null!;

            return nextSpan;
        }

        var byteSting = nextSpan[..stringLength];
        value = Encoding.UTF8.GetString(byteSting);

        return nextSpan[stringLength..];
    }

    internal static ReadOnlySpan<byte> ReadString(this ReadOnlySpan<byte> span, out string value)
    {
        var nextSpan = span.ReadInt16(out var stringLength);
        var byteSting = nextSpan[..stringLength];
        value = Encoding.UTF8.GetString(byteSting);

        return nextSpan[stringLength..];
    }

    internal static ReadOnlySpan<byte> ReadInt16(this ReadOnlySpan<byte> span, out short value)
    {
        value = ReadInt16BigEndian(span[..2]);

        return span[2..];
    }

    internal static ReadOnlySpan<byte> ReadInt32(this ReadOnlySpan<byte> span, out int value)
    {
        value = ReadInt32BigEndian(span[..4]);

        return span[4..];
    }

    internal static ReadOnlySpan<byte> ReadBoolean(this ReadOnlySpan<byte> span, out bool value)
    {
        value = span[..1][0] == 1;

        return span[1..];
    }
}