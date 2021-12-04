using System;

namespace Microlibs.Kafka.Serialization;

public sealed class IntegerSerializer : ISerializer<int>
{
    public byte[] Serialize(int data)
    {
        return new[]
        {
            (byte)(data >> 24),
            (byte)(data >> 16),
            (byte)(data >> 8),
            (byte)data
        };
    }
}