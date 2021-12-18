using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Protocol.Responses;
using Microsoft.Extensions.Logging.Abstractions;
using ConfluentKafka = Confluent.Kafka;

namespace Microlibs.Kafka.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net50)]
[SimpleJob(RuntimeMoniker.Net60)]
public class ProduceBenchmarks
{
    private ConfluentKafka.IAdminClient _adminClient;
    private IKafkaCluster _cluster;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var adminClientBuilder = new ConfluentKafka.AdminClientBuilder(
            new ConfluentKafka.AdminClientConfig
            {
                BootstrapServers = "localhost:9091",
            });

        _adminClient = adminClientBuilder.Build();

        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:9092"
            }
        };

        _cluster = clusterConfig
            .CreateNewClusterAsync(NullLoggerFactory.Instance)
            .GetAwaiter()
            .GetResult();
    }

    [Benchmark(Baseline = true)]
    public ConfluentKafka.Metadata ConfluentKafkaGetMetadata()
    {
        return _adminClient.GetMetadata("test", TimeSpan.FromSeconds(10));
    }

    [Benchmark]
    public Task<MetadataResponseMessage> NewKafkaGetMetadata()
    {
        return _cluster.RefreshMetadataAsync(default, "test");
    }
}