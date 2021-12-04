using System;

namespace Microlibs.Kafka.Serialization;

public sealed class DoubleSerializer : ISerializer<double>
{
    public byte[] Serialize(double data)
    {
        var bits = BitConverter.DoubleToInt64Bits(data);

        return new[]
        {
            (byte)(bits >> 56),
            (byte)(bits >> 48),
            (byte)(bits >> 40),
            (byte)(bits >> 32),
            (byte)(bits >> 24),
            (byte)(bits >> 16),
            (byte)(bits >> 8),
            (byte)bits
        };
    }
}