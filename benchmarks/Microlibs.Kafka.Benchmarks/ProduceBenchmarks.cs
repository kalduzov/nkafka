using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microlibs.Kafka.Clients.Producer;
using Microlibs.Kafka.Config;
using Microsoft.Extensions.Logging.Abstractions;
using ConfluentKafka = Confluent.Kafka;

namespace Microlibs.Kafka.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net50)]
public class ProduceBenchmarks
{
    private ConfluentKafka.IProducer<ConfluentKafka.Null, int> _confluentProducer;
    private IProducer<Null, int> _newProducer;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var confluentProducerBuilder = new ConfluentKafka.ProducerBuilder<ConfluentKafka.Null, int>(
            new ConfluentKafka.ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                EnableDeliveryReports = false
            });

        _confluentProducer = confluentProducerBuilder.Build();

        var cluster = KafkaCluster.Create(
            new ClusterConfig
            {
                BootstrapServers = new[]
                {
                    "localhost:9092"
                }
            },
            NullLoggerFactory.Instance);

        _newProducer = cluster.BuildProducer<Null, int>();
    }

    [Benchmark]
    public void ConfluentKafkaProduce()
    {
        // foreach (var val in Enumerable.Range(1, 5))
        // {
        var message = new ConfluentKafka.Message<ConfluentKafka.Null, int>
        {
            Value = 1
        };
        _confluentProducer.Produce("test1", message);

        //}
    }

    [Benchmark]
    public void NewProduce()
    {
        // foreach (var val in Enumerable.Range(1, 5))
        // {
        var message = new Message<Null, int>
        {
            Value = 1
        };
        _newProducer.Produce("test1", message);

        //}
    }
}