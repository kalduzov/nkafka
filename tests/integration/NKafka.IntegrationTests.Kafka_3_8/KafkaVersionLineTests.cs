namespace NKafka.IntegrationTests.Kafka_3_8;

public class KafkaVersionLineTests
{
    public static bool IsKafka38LineEnabled => E2E.KafkaE2EClusterFactory.IsKafkaVersionLineSelected("3.8");

    [Trait("Category", "KafkaVersionLine")]
    [Trait("KafkaVersionLine", "3.8")]
    [Fact(
        SkipUnless = nameof(IsKafka38LineEnabled),
        Skip = "Kafka 3.8 line is not selected. Set NKAFKA_E2E_ENABLED=true and NKAFKA_E2E_KAFKA_VERSION to a 3.8.x value.")]
    public void SelectedKafkaVersion_ShouldMatch_3_8_Line()
    {
        E2E.KafkaE2EClusterFactory.CurrentKafkaVersion.Should().StartWith("3.8");
    }
}
