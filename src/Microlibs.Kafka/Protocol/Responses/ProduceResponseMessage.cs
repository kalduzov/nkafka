using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using Microsoft.IO;

namespace Microlibs.Kafka.Protocol.Responses;

public class ProduceResponseMessage : KafkaResponseMessage
{
    public override void DeserializeFromStream(PipeReader reader)
    {
    }
}