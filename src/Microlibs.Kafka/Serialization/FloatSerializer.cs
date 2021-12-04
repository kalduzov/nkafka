using System;

namespace Microlibs.Kafka.Serialization;

public sealed class FloatSerializer : ISerializer<float>
{
    public byte[] Serialize(float data)
    {
        var bits = BitConverter.SingleToInt32Bits(data);

        return new[]
        {
            (byte)(bits >> 24),
            (byte)(bits >> 16),
            (byte)(bits >> 8),
            (byte)bits
        };
    }
}