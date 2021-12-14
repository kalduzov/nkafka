using System.Buffers;
using System.IO;
using Microsoft.IO;

namespace Microlibs.Kafka.Protocol.Responses;

public class ProduceResponseMessage : KafkaResponseMessage
{
    public override void DeserializeFromStream(Stream sequence)
    {
    }
}