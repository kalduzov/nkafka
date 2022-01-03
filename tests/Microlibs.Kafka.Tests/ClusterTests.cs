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
}