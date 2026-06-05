using NKafka.IntegrationTests.E2E;
using NKafka.Protocol;

namespace NKafka.IntegrationTests.Protocol;

public abstract class ApiVersionsE2ETestsBase
{
    public static bool IsSupportedApiVersionsProfileEnabled => KafkaE2EClusterFactory.IsSupportedRuntimeProfileEnabled;

    [Trait("Category", "E2E")]
    [Trait("Scenario", "ApiVersions")]
    [Fact(
        SkipUnless = nameof(IsSupportedApiVersionsProfileEnabled),
        Skip = "A supported Kafka E2E profile is not selected. Set NKAFKA_E2E_ENABLED=true together with a plaintext or supported SASL plaintext profile.")]
    public async Task OpenCluster_ShouldPopulate_ApiVersionsMetadata()
    {
        await using var kafkaCluster = await KafkaE2EClusterFactory.CreateClusterAsync();

        var clusterMetadata = kafkaCluster.GetClusterMetadata();
        clusterMetadata.AggregationApiByVersion.Should().ContainKey(ApiKeys.ApiVersions);
        ((short)clusterMetadata.GetMaxCurrentApiVersion(ApiKeys.ApiVersions)).Should().BeGreaterThanOrEqualTo((short)ApiVersion.Version0);
    }
}
