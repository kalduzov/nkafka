namespace Microlibs.Kafka.Protocol.Responses
{
    public record DescribeResponseMessage(int TrottleTimeMs, string ClusterId, int ControllerId, int ClusterAuthorizedOperations) : ResponseMessage;
}