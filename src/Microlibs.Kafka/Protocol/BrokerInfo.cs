namespace Microlibs.Kafka.Protocol;

public record struct BrokerInfo(int NodeId, string Host, int Port, string? Rack);