using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microlibs.Kafka.Protocol;

internal static class VarInt
{
    internal static string ReadCompactString(this BinaryReader reader)
    {
        var len = ReadUVariant(reader);
        var buf = reader.ReadBytes(len);

        return Encoding.UTF8.GetString(buf);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    private static int ReadUVariant(BinaryReader reader)
    {
        var i = 0;
        var value = 0;
        var shift = 0;

        while (true)
        {
            var b = reader.ReadByte();

            if (b < 0x80)
            {
                if (i >= 5 || i == 4 && b > 1)
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
}