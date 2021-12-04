namespace Microlibs.Kafka.Serialization;

public sealed class LongSerializer: ISerializer<long>
{
    public byte[] Serialize(long data)
    {
        return new[]
        {
            (byte)(data >> 56),
            (byte)(data >> 48),
            (byte)(data >> 40),
            (byte)(data >> 32),
            (byte)(data >> 24),
            (byte)(data >> 16),
            (byte)(data >> 8),
            (byte)data
        };
    }
}