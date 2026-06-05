namespace NKafka.IntegrationTests.Kafka_3_8.Protocol;

public class ApiVersionsE2ETests : NKafka.IntegrationTests.Shared.Versioned.Kafka_3_8.Protocol.ApiVersionsE2ETestsBase
{
    public new static bool IsSupportedApiVersionsProfileEnabled => E2E.KafkaE2EClusterFactory.IsSupportedRuntimeProfileEnabled;
}
