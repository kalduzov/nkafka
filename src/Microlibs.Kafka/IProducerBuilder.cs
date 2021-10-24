using Microlibs.Kafka.Clients;

namespace Microlibs.Kafka
{
    public interface IProducerBuilder
    {
        const string DEFAULT_PRODUCER_NAME = "__DefaultProducer";

        IProducer Build()
        {
            return Build(DEFAULT_PRODUCER_NAME);
        }

        IProducer Build(string name);
    }
}