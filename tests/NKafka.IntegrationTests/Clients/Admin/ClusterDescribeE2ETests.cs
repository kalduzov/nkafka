using NKafka.Clients.Admin;
using NKafka.IntegrationTests.E2E;

namespace NKafka.IntegrationTests.Clients.Admin;

public class ClusterDescribeE2ETests
{
    public static bool IsZkPlaintextProfileEnabled => KafkaE2EClusterFactory.IsZkPlaintextProfileEnabled;

    public static bool IsKraftPlaintextProfileEnabled => KafkaE2EClusterFactory.IsKraftPlaintextProfileEnabled;

    public static bool IsKraftSaslPlainProfileEnabled => KafkaE2EClusterFactory.IsKraftSaslPlainProfileEnabled;

    public static bool IsKraftSaslScramSha256ProfileEnabled => KafkaE2EClusterFactory.IsKraftSaslScramSha256ProfileEnabled;

    public static bool IsKraftSaslScramSha512ProfileEnabled => KafkaE2EClusterFactory.IsKraftSaslScramSha512ProfileEnabled;

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "zk-plaintext")]
    [Fact(
        SkipUnless = nameof(IsZkPlaintextProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true and NKAFKA_E2E_PROFILE=zk-plaintext.")]
    public async Task DescribeCluster_WithZkPlaintextProfile_ShouldBe_Successful()
    {
        await DescribeClusterSmokeAsync();
    }

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "kraft-plaintext")]
    [Fact(
        SkipUnless = nameof(IsKraftPlaintextProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true and NKAFKA_E2E_PROFILE=kraft-plaintext.")]
    public async Task DescribeCluster_WithKraftPlaintextProfile_ShouldBe_Successful()
    {
        await DescribeClusterSmokeAsync();
    }

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "kraft-sasl-plain")]
    [Fact(
        SkipUnless = nameof(IsKraftSaslPlainProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true and NKAFKA_E2E_PROFILE=kraft-sasl-plain.")]
    public async Task DescribeCluster_WithKraftSaslPlainProfile_ShouldBe_Successful()
    {
        await DescribeClusterSmokeAsync();
    }

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "kraft-sasl-scram256")]
    [Fact(
        SkipUnless = nameof(IsKraftSaslScramSha256ProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true and NKAFKA_E2E_PROFILE=kraft-sasl-scram256.")]
    public async Task DescribeCluster_WithKraftSaslScramSha256Profile_ShouldBe_Successful()
    {
        await DescribeClusterSmokeAsync();
    }

    [Trait("Category", "E2E")]
    [Trait("KafkaProfile", "kraft-sasl-scram512")]
    [Fact(
        SkipUnless = nameof(IsKraftSaslScramSha512ProfileEnabled),
        Skip = "Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true and NKAFKA_E2E_PROFILE=kraft-sasl-scram512.")]
    public async Task DescribeCluster_WithKraftSaslScramSha512Profile_ShouldBe_Successful()
    {
        await DescribeClusterSmokeAsync();
    }

    private static async Task DescribeClusterSmokeAsync()
    {
        await using var kafkaCluster = await KafkaE2EClusterFactory.CreateClusterAsync();

        var result = await kafkaCluster.AdminClient.DescribeClusterAsync(new DescribeClusterOptions(), CancellationToken.None);

        result.Controller.Should().NotBeNull();
        result.Nodes.Should().NotBeEmpty();
        result.ClusterId.Should().NotBeNull();
    }
}
