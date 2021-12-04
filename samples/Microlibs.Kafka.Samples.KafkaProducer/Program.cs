using System;
using System.Threading;
using Microlibs.Kafka;
using Microlibs.Kafka.Config;
using Microsoft.Extensions.Logging.Abstractions;

var loggerFactory = new NullLoggerFactory();

using var kafkaCluster = KafkaCluster.Create(
    new ClusterConfig
    {
        BootstrapServers = new[]
        {
            "localhost:9092"
        }
    },
    loggerFactory);

using var producer = kafkaCluster.BuildProducer<int, int>();
{
    var test = new Message<int, int>(1, 1);

    await producer.ProduceAsync("test_topic", test, CancellationToken.None);
    Console.WriteLine("Produce OK");
    Console.ReadKey();
}