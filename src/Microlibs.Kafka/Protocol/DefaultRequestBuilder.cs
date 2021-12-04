namespace Microlibs.Kafka.Protocol
{
    internal class DefaultRequestBuilder : IRequestBuilder
    {
        // private readonly string _clientName;
        //
        // public DefaultRequestBuilder(string clientName)
        // {
        //     _clientName = clientName;
        // }
        //
        // public KafkaRequestMessage Create<TMessage>(ApiKeys apiKey, int correlationId, string topicName, TMessage message)
        // {
        //     return new KafkaRequestMessage();
        // }
        //
        // public KafkaRequestMessage Create(ApiKeys apiKey, int correlationId)
        // {
        //     return Create<KafkaContent>(apiKey, correlationId, null!);
        // }
        //
        // public KafkaRequestMessage Create<T>(ApiKeys apiKey, int correlationId, T message)
        //     where T : class
        // {
        //     return apiKey switch
        //     {
        //         ApiKeys.ApiVersions => CreteApiVersionsRequest(correlationId),
        //     };
        // }
        //
        // private KafkaRequestMessage CreteApiVersionsRequest(int correlationId)
        // {
        //     var header = new KafkaRequestHeader(ApiKeys.ApiVersions, 0, correlationId, _clientName);
        //     var message = KafkaContent.Empty;
        //
        //     return new KafkaRequestMessage(header, message);
        // }
    }
}