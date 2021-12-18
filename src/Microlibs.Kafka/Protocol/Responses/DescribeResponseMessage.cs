using System;
using System.IO;
using System.IO.Pipelines;

namespace Microlibs.Kafka.Protocol.Responses;

public record DescribeResponseMessage : KafkaResponseMessage
{
    public DescribeResponseMessage()
    {
    }

    public int TrottleTimeMs { get; init; }

    public string ClusterId { get; init; }

    public int ControllerId { get; init; }

    public int ClusterAuthorizedOperations { get; init; }
}