using System.Threading.Tasks;
using FluentAssertions;
using Microlibs.Kafka.Config;
using Microlibs.Kafka.Exceptions;
using Xunit;

namespace Microlibs.Kafka.Tests;

public class ClusterTests
{
    [Fact(DisplayName = "Create new cluster when timeout must be throw ClusterKafkaException")]
    public async Task CreateNewClusterAsync_WhenTimeout_MustBeException()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:29091"
            },
            ClusterInitTimeoutMs = 10
        };

        var awaiting = FluentActions.Awaiting(() => clusterConfig.CreateNewClusterAsync());
        await awaiting.Should().ThrowAsync<ClusterKafkaException>();
    }

    [Fact(DisplayName = "Create new cluster broker updated")]
    public async Task CreateNewClusterAsync_BrokerUpdated()
    {
        var clusterConfig = new ClusterConfig
        {
            BootstrapServers = new[]
            {
                "localhost:29091"
            },
            ClusterInitTimeoutMs = 10000
        };

        await using var kafkaCluster = await clusterConfig.CreateNewClusterAsync();
        kafkaCluster.Brokers.Should().HaveCount(5);
        kafkaCluster.Controller.Should().NotBeNull();
        kafkaCluster.Controller.Id.Should().BePositive();
        kafkaCluster.ClusterId.Should().NotBeNull();
        kafkaCluster.ClusterId.Should().HaveLength(22);
        var partitions = await kafkaCluster.GetPartitionsAsync("test");
        partitions.Should().HaveCount(5);
    }
}