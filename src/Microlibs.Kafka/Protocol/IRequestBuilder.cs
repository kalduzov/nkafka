namespace Microlibs.Kafka.Protocol
{
    internal interface IRequestBuilder
    {
        KafkaRequest Create<TMessage>(ApiKeys apiKey, int correlationId, string topicName, TMessage message);

        KafkaRequest Create(ApiKeys apiKey, int correlationId);
    }
}