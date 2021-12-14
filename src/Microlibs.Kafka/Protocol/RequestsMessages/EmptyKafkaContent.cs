using System;
using System.IO;

namespace Microlibs.Kafka.Protocol.RequestsMessages;

internal class EmptyKafkaContent : KafkaContent
{
    public EmptyKafkaContent()
    {
        Length = 0x00;
    }

    public override void SerializeToStream(Stream stream)
    {
        stream.Write(Array.Empty<byte>());
    }
}