namespace NKafka.IntegrationTests.Kafka_3_7;

public class KafkaVersionLineTests
{
    public static bool IsKafka37LineEnabled => E2E.KafkaE2EClusterFactory.IsKafkaVersionLineSelected("3.7");

    [Trait("Category", "KafkaVersionLine")]
    [Trait("KafkaVersionLine", "3.7")]
    [Fact(
        SkipUnless = nameof(IsKafka37LineEnabled),
        Skip = "Kafka 3.7 line is not selected. Set NKAFKA_E2E_ENABLED=true and NKAFKA_E2E_KAFKA_VERSION to a 3.7.x value.")]
    public void SelectedKafkaVersion_ShouldMatch_3_7_Line()
    {
        E2E.KafkaE2EClusterFactory.CurrentKafkaVersion.Should().StartWith("3.7");
    }
}
