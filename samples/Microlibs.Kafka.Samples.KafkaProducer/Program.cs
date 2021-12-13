using System;
using System.Threading;
using Microlibs.Kafka;
using Microlibs.Kafka.Config;

await using var kafkaCluster = await KafkaCluster.CreateAsync(
    new ClusterConfig
    {
        BootstrapServers = new[]
        {
            "localhost:9091"
        }
    });

using var producer = kafkaCluster.BuildProducer<Null, int>();
{
    var test = new Message<Null, int>
    {
        Value = 1
    };

    await producer.ProduceAsync("test_topic", test, CancellationToken.None);
    Console.WriteLine("Produce OK");
}

Console.ReadKey();