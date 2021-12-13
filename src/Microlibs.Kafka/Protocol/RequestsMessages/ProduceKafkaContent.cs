using System;

namespace Microlibs.Kafka.Protocol.RequestsMessages;

internal class ProduceKafkaContent : KafkaContent
{
    public override ReadOnlySpan<byte> AsReadOnlySpan()
    {
        return default;
    }
}