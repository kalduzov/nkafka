namespace Microlibs.Kafka.Serialization;

public sealed class NullSerializer : ISerializer<Null>
{
    public byte[] Serialize(Null data)
    {
        return null!;
    }
}