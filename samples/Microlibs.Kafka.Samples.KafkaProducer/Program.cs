using System;
using System.Threading;
using Microlibs.Kafka;
using Microlibs.Kafka.Config;

var producerBuilder = KafkaCluster.CreateProducerBuilder(
    new ProducerConfig
    {
        BootstrapServers = new[]
        {
            "localhost:9092"
        }
    });

using var producer = producerBuilder.Build();
{
    var test = new
    {
        Id = 1
    };

    await producer.ProduceAsync("test_topic", test, CancellationToken.None);
    Console.WriteLine("Produce OK");
    Console.ReadKey();
}