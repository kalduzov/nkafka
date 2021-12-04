namespace Microlibs.Kafka.Serialization;

public sealed class ByteArraySerializer : ISerializer<byte[]>
{
    public byte[] Serialize(byte[] data)
    {
        return data;
    }
}