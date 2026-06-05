namespace NKafka.IntegrationTests.Kafka_3_9;

public class KafkaVersionLineTests
{
    public static bool IsKafka39LineEnabled => E2E.KafkaE2EClusterFactory.IsKafkaVersionLineSelected("3.9");

    [Trait("Category", "KafkaVersionLine")]
    [Trait("KafkaVersionLine", "3.9")]
    [Fact(
        SkipUnless = nameof(IsKafka39LineEnabled),
        Skip = "Kafka 3.9 line is not selected. Set NKAFKA_E2E_ENABLED=true and NKAFKA_E2E_KAFKA_VERSION to a 3.9.x value.")]
    public void SelectedKafkaVersion_ShouldMatch_3_9_Line()
    {
        E2E.KafkaE2EClusterFactory.CurrentKafkaVersion.Should().StartWith("3.9");
    }
}
