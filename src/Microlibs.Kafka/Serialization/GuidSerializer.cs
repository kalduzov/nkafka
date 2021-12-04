using System;

namespace Microlibs.Kafka.Serialization;

public sealed class GuidSerializer: ISerializer<Guid>
{
    public byte[] Serialize(Guid data)
    {
        return data.ToByteArray();
    }
}