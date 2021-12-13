namespace Microlibs.Kafka.Protocol;

internal interface IRequestBuilder
{
    // KafkaRequestMessage Create<TMessage>(ApiKeys apiKey, int correlationId, string topicName, TMessage message);
    //
    // KafkaRequestMessage Create(ApiKeys apiKey, int correlationId);
    //
    // KafkaRequestMessage Create<TMessage>(ApiKeys apiKey, TMessage message);
}