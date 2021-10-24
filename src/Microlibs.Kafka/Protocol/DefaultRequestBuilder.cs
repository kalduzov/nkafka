namespace Microlibs.Kafka.Protocol
{
    internal class DefaultRequestBuilder : IRequestBuilder
    {
        private readonly string _clientName;

        public DefaultRequestBuilder(string clientName)
        {
            _clientName = clientName;
        }

        public KafkaRequest Create<TMessage>(ApiKeys apiKey, int correlationId, string topicName, TMessage message)
        {
            return new KafkaRequest();
        }

        public KafkaRequest Create(ApiKeys apiKey, int correlationId)
        {
            return Create<RequestMessage>(apiKey, correlationId, null!);
        }

        public KafkaRequest Create<T>(ApiKeys apiKey, int correlationId, T message)
            where T : class
        {
            return apiKey switch
            {
                ApiKeys.ApiVersions => CreteApiVersionsRequest(correlationId),
            };
        }

        private KafkaRequest CreteApiVersionsRequest(int correlationId)
        {
            var header = new RequestHeader(ApiKeys.ApiVersions, 0, correlationId, _clientName);
            var message = RequestMessage.Empty;

            return new KafkaRequest(header, message);
        }
    }
}