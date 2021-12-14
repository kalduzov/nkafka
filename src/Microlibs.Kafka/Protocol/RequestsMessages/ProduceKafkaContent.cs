using System.IO;

namespace Microlibs.Kafka.Protocol.RequestsMessages;

internal class ProduceKafkaContent : KafkaContent
{
    public override void SerializeToStream(Stream stream)
    {
    }
}