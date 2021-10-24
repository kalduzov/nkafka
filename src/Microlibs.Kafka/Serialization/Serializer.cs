using System;

namespace Microlibs.Kafka
{
    public interface Serializer<in T> : IDisposable
    {
        Span<byte> Serialize(T data);
    }
}