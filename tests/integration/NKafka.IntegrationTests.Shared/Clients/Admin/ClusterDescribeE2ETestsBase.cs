using NKafka.Clients.Admin;
using NKafka.Exceptions;
using NKafka.IntegrationTests.E2E;

namespace NKafka.IntegrationTests.Clients.Admin;

public abstract class ClusterDescribeE2ETestsBase
{
    public static bool IsZkPlaintextProfileEnabled => KafkaE2EClusterFactory.IsZkPlaintextProfileEnabled;

    public static bool IsKraftPlaintextProfileEnabled => KafkaE2EClusterFactory.IsKraftPlaintextProfileEnabled;

    public static bool IsKraftSaslPlainProfileEnabled => KafkaE2EClusterFactory.IsKraftSaslPlainProfileEnabled;

    public static bool IsKraftSaslScramSha256ProfileEnabled => KafkaE2EClusterFactory.IsKraftSaslScramSha256ProfileEnabled;

    public static bool IsKraftSaslScramSha512ProfileEnabled => KafkaE2EClusterFactory.IsKraftSaslScramSha512ProfileEnabled;

    public static bool IsKraftSaslOAuthBearerProfileEnabled => KafkaE2EClusterFactory.IsKraftSaslOAuthBearerProfileEnabled;

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "zk-plaintext")]
    [Trait("TopologyMode", "zk")]
    [Trait("SecurityProfile", "plaintext")]
    [Fact(
        SkipUnless = nameof(IsZkPlaintextProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true, NKAFKA_E2E_TOPOLOGY_MODE=zk and NKAFKA_E2E_SECURITY_PROFILE=plaintext.")]
    public async Task DescribeCluster_WithZkPlaintextProfile_ShouldBe_Successful()
    {
        await DescribeClusterSmokeAsync();
    }

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "kraft-plaintext")]
    [Trait("TopologyMode", "kraft")]
    [Trait("SecurityProfile", "plaintext")]
    [Fact(
        SkipUnless = nameof(IsKraftPlaintextProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true, NKAFKA_E2E_TOPOLOGY_MODE=kraft and NKAFKA_E2E_SECURITY_PROFILE=plaintext.")]
    public async Task DescribeCluster_WithKraftPlaintextProfile_ShouldBe_Successful()
    {
        await DescribeClusterSmokeAsync();
    }

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "kraft-sasl-plain")]
    [Trait("TopologyMode", "kraft")]
    [Trait("SecurityProfile", "sasl-plain")]
    [Fact(
        SkipUnless = nameof(IsKraftSaslPlainProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true, NKAFKA_E2E_TOPOLOGY_MODE=kraft and NKAFKA_E2E_SECURITY_PROFILE=sasl-plain.")]
    public async Task DescribeCluster_WithKraftSaslPlainProfile_ShouldBe_Successful()
    {
        await DescribeClusterSmokeAsync();
    }

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "kraft-sasl-scram256")]
    [Trait("TopologyMode", "kraft")]
    [Trait("SecurityProfile", "sasl-scram256")]
    [Fact(
        SkipUnless = nameof(IsKraftSaslScramSha256ProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true, NKAFKA_E2E_TOPOLOGY_MODE=kraft and NKAFKA_E2E_SECURITY_PROFILE=sasl-scram256.")]
    public async Task DescribeCluster_WithKraftSaslScramSha256Profile_ShouldBe_Successful()
    {
        await DescribeClusterSmokeAsync();
    }

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "kraft-sasl-scram512")]
    [Trait("TopologyMode", "kraft")]
    [Trait("SecurityProfile", "sasl-scram512")]
    [Fact(
        SkipUnless = nameof(IsKraftSaslScramSha512ProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true, NKAFKA_E2E_TOPOLOGY_MODE=kraft and NKAFKA_E2E_SECURITY_PROFILE=sasl-scram512.")]
    public async Task DescribeCluster_WithKraftSaslScramSha512Profile_ShouldBe_Successful()
    {
        await DescribeClusterSmokeAsync();
    }

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "kraft-sasl-oauthbearer")]
    [Trait("TopologyMode", "kraft")]
    [Trait("SecurityProfile", "sasl-oauthbearer")]
    [Fact(
        SkipUnless = nameof(IsKraftSaslOAuthBearerProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true, NKAFKA_E2E_TOPOLOGY_MODE=kraft and NKAFKA_E2E_SECURITY_PROFILE=sasl-oauthbearer.")]
    public async Task CreateCluster_WithKraftSaslOAuthBearerProfile_ShouldFailHonestly_AsUnsupportedRuntime()
    {
        var createCluster = async () => await KafkaE2EClusterFactory.CreateClusterAsync();

        await createCluster.Should()
            .ThrowAsync<KafkaConfigException>()
            .WithMessage("*OAUTHBEARER*");
    }

    protected static async Task DescribeClusterSmokeAsync()
    {
        await using var kafkaCluster = await KafkaE2EClusterFactory.CreateClusterAsync();

        var result = await kafkaCluster.AdminClient.DescribeClusterAsync(new DescribeClusterOptions(), CancellationToken.None);

        result.Controller.Should().NotBeNull();
        result.Nodes.Should().NotBeEmpty();
        result.ClusterId.Should().NotBeNull();
    }
}
