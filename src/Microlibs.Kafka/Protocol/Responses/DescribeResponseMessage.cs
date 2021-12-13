namespace Microlibs.Kafka.Protocol.Responses;

public class DescribeResponseMessage : KafkaResponseMessage
{
    public DescribeResponseMessage(int trottleTimeMs, string clusterId, int controllerId, int clusterAuthorizedOperations)
    {
        TrottleTimeMs = trottleTimeMs;
        ClusterId = clusterId;
        ControllerId = controllerId;
        ClusterAuthorizedOperations = clusterAuthorizedOperations;
    }

    public int TrottleTimeMs { get; init; }

    public string ClusterId { get; init; }

    public int ControllerId { get; init; }

    public int ClusterAuthorizedOperations { get; init; }
}