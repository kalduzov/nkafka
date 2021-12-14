using System.Buffers;
using System.IO;
using Microsoft.IO;

namespace Microlibs.Kafka.Protocol.Responses;

public class DescribeResponseMessage : KafkaResponseMessage
{
    public DescribeResponseMessage()
    {
    }

    public int TrottleTimeMs { get; init; }

    public string ClusterId { get; init; }

    public int ControllerId { get; init; }

    public int ClusterAuthorizedOperations { get; init; }

    public override void DeserializeFromStream(Stream stream)
    {

    }
}