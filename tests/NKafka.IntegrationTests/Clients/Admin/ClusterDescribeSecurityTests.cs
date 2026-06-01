using NKafka.Clients.Admin;

namespace NKafka.IntegrationTests.Clients.Admin;

public class ClusterDescribeSecurityTests
{
    public static bool IsSecurityScenarioEnabled => IntegrationClusterFactory.IsSecurityScenarioEnabled;

    [Fact(
        SkipUnless = nameof(IsSecurityScenarioEnabled),
        Skip = "Security integration scenario is disabled. Set NKAFKA_IT_ENABLE_SECURITY_SCENARIOS=true and configure a non-plaintext integration broker.")]
    public async Task DescribeCluster_WithConfiguredSecurityTransport_ShouldBe_Successful()
    {
        await using var kafkaCluster = await IntegrationClusterFactory.CreateClusterAsync();

        var result = await kafkaCluster.AdminClient.DescribeClusterAsync(new DescribeClusterOptions(), CancellationToken.None);

        result.Controller.Should().NotBeNull();
        result.Nodes.Should().NotBeEmpty();
        result.ClusterId.Should().NotBeNull();
    }
}
