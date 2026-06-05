using NKafka.Clients.Admin;

namespace NKafka.IntegrationTests.Clients.Admin;

public abstract class ClusterDescribeTestsBase
{
    [Fact]
    public async Task DescribeCluster_ShouldBe_Successful()
    {
        await using var kafkaCluster = await BuildKafkaCluster();
        var result = await kafkaCluster.AdminClient.DescribeClusterAsync(new DescribeClusterOptions(), CancellationToken.None);
        result.Controller.Should().NotBeNull();
        result.Nodes.Count.Should().Be(5);
        result.Nodes.Contains(result.Controller).Should().BeTrue();
        result.ClusterId.Should().NotBeNull();
    }

    protected static async Task<IKafkaCluster> BuildKafkaCluster()
        => await IntegrationClusterFactory.CreateClusterAsync();
}
