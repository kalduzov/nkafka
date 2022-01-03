namespace Microlibs.Kafka.Compressions;

public class GZIPCompression : ICompression
{
    public byte[] Decode(byte[] data)
    {
        return new byte[]
        {
        };
    }

    public byte[] Encode(byte[] data)
    {
        return new byte[]
        {
        };
    }
}