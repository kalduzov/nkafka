namespace Microlibs.Kafka.Serialization;

public sealed class ShortSerializer : ISerializer<short>
{
    public byte[] Serialize(short data)
    {
        return new[]
        {
            (byte)(data >> 8),
            (byte)data
        };
    }
}