using System;

namespace Microlibs.Kafka.Protocol.RequestsMessages;

internal class EmptyKafkaContent : KafkaContent
{
    public EmptyKafkaContent()
    {
        Length = 0x00;
    }

    public override ReadOnlySpan<byte> AsReadOnlySpan()
    {
        return Array.Empty<byte>();
    }
}