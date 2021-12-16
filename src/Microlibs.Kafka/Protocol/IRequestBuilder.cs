namespace Microlibs.Kafka.Protocol;

internal interface IRequestBuilder
{
    KafkaRequestMessage Create(ApiKeys apiKey, ApiVersions version);
}