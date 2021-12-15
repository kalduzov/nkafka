using System;
using System.Buffers;
using System.IO;
using Microsoft.IO;

namespace Microlibs.Kafka.Protocol.Responses;

public class ProduceResponseMessage : KafkaResponseMessage
{
    public override void DeserializeFromStream(ReadOnlySpan<byte> span)
    {
    }
}