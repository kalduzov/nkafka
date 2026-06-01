using NKafka.Clients.Admin;

namespace NKafka.IntegrationTests.Clients.Admin;

public class ClusterDescribeSecurityTests
{
    public static bool IsSecurityScenarioEnabled => IntegrationClusterFactory.IsSecurityScenarioEnabled;
    public static bool IsPlainSaslSecurityScenarioEnabled => IntegrationClusterFactory.IsPlainSaslSecurityScenarioEnabled;
    public static bool IsOAuthBearerSecurityScenarioEnabled => IntegrationClusterFactory.IsOAuthBearerSecurityScenarioEnabled;
    public static bool IsScramSecurityScenarioEnabled => IntegrationClusterFactory.IsScramSecurityScenarioEnabled;

    [Trait("Category", "SecurityIntegration")]
    [Trait("SecurityTransport", "Generic")]
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

    [Trait("Category", "SecurityIntegration")]
    [Trait("SecurityMechanism", "PLAIN")]
    [Fact(
        SkipUnless = nameof(IsPlainSaslSecurityScenarioEnabled),
        Skip = "PLAIN integration scenario is disabled. Set NKAFKA_IT_ENABLE_SECURITY_SCENARIOS=true and configure NKAFKA_IT_SECURITY_PROTOCOL plus NKAFKA_IT_SASL_MECHANISM=Plain.")]
    public async Task DescribeCluster_WithConfiguredPlainSaslTransport_ShouldBe_Successful()
    {
        await using var kafkaCluster = await IntegrationClusterFactory.CreateClusterAsync();

        var result = await kafkaCluster.AdminClient.DescribeClusterAsync(new DescribeClusterOptions(), CancellationToken.None);

        result.Controller.Should().NotBeNull();
        result.Nodes.Should().NotBeEmpty();
        result.ClusterId.Should().NotBeNull();
    }

    [Trait("Category", "SecurityIntegration")]
    [Trait("SecurityMechanism", "OAUTHBEARER")]
    [Fact(
        SkipUnless = nameof(IsOAuthBearerSecurityScenarioEnabled),
        Skip = "OAUTHBEARER integration scenario is disabled. Set NKAFKA_IT_ENABLE_SECURITY_SCENARIOS=true and configure NKAFKA_IT_SECURITY_PROTOCOL plus NKAFKA_IT_SASL_MECHANISM=OAuthBearer.")]
    public async Task DescribeCluster_WithConfiguredOAuthBearerTransport_ShouldBe_Successful()
    {
        await using var kafkaCluster = await IntegrationClusterFactory.CreateClusterAsync();

        var result = await kafkaCluster.AdminClient.DescribeClusterAsync(new DescribeClusterOptions(), CancellationToken.None);

        result.Controller.Should().NotBeNull();
        result.Nodes.Should().NotBeEmpty();
        result.ClusterId.Should().NotBeNull();
    }

    [Trait("Category", "SecurityIntegration")]
    [Trait("SecurityMechanism", "SCRAM")]
    [Fact(
        SkipUnless = nameof(IsScramSecurityScenarioEnabled),
        Skip = "SCRAM integration scenario is disabled. Set NKAFKA_IT_ENABLE_SECURITY_SCENARIOS=true and configure NKAFKA_IT_SECURITY_PROTOCOL plus NKAFKA_IT_SASL_MECHANISM=ScramSha256 or ScramSha512.")]
    public async Task DescribeCluster_WithConfiguredScramTransport_ShouldBe_Successful()
    {
        await using var kafkaCluster = await IntegrationClusterFactory.CreateClusterAsync();

        var result = await kafkaCluster.AdminClient.DescribeClusterAsync(new DescribeClusterOptions(), CancellationToken.None);

        result.Controller.Should().NotBeNull();
        result.Nodes.Should().NotBeEmpty();
        result.ClusterId.Should().NotBeNull();
    }
}
