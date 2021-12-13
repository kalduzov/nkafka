namespace Microlibs.Kafka.Protocol.Connection;

/// <summary>
/// </summary>
/// <param name="Host"></param>
/// <param name="Port"></param>
public record BrokerEndpoint(string Host, int Port);