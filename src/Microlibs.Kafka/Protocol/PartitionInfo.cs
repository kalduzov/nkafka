namespace Microlibs.Kafka.Protocol;

public record PartitionInfo(
    StatusCodes ErrorCode,
    int PartitionIndex,
    int LeaderId,
    int LeaderEpoch,
    int[] ReplicaNodes,
    int[] IsrNodes,
    int[]? OfflineReplicas)
{
}