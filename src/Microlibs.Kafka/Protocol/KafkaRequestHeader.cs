namespace Microlibs.Kafka.Protocol;

public readonly struct KafkaRequestHeader
{
    public KafkaRequestHeader(ApiKeys apiKey, ApiVersions apiVersion, int correlationId, string clientId)
    {
        ApiKey = apiKey;
        ApiVersion = apiVersion;
        CorrelationId = correlationId;
        ClientId = clientId;

        Length = 0x2 + 0x2 + 0x4 + 0x2 + ClientId.Length;
    }

    public readonly ApiKeys ApiKey;

    public readonly ApiVersions ApiVersion;

    public readonly int CorrelationId;

    public readonly string ClientId;

    public readonly int Length;
}