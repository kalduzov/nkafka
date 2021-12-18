using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microlibs.Kafka;
using Microlibs.Kafka.Config;

var stopwatch = Stopwatch.StartNew();

var clusterConfig = new ClusterConfig
{
    BootstrapServers = new[]
    {
        "localhost:9091"
    },
    ClusterInitTimeoutMs = 160000 //160сек для отладки
};

await using var kafkaCluster = await clusterConfig.CreateNewClusterAsync();

// var tasks = Enumerable.Range(0, 10000)
//     .Select(
//         i => kafkaCluster.RefreshMetadataAsync(default, "test"));
//
// await Task.WhenAll(tasks);

// await using var producer = kafkaCluster.BuildProducer<Null, int>();
//
// var test = new Message<Null, int>
// {
//     Value = 1,
//     Partition = new Partition(1)
// };
//
// await producer.ProduceAsync("test_topic", test, CancellationToken.None);
Console.WriteLine($"All OK. {stopwatch.Elapsed}");


Console.ReadKey();