// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Config;
using NKafka.Exceptions;

namespace NKafka.Tests.Config;

public sealed class TransactionalProducerConfigTests
{
    [Fact]
    public void Constructor_EnablesTransactionSafeDefaults()
    {
        var config = new TransactionalProducerConfig();

        config.EnableIdempotence.Should().BeTrue();
        config.Acks.Should().Be(Acks.All);
    }

    [Fact]
    public void Configuration_InheritsProducerConfiguration()
    {
        var config = new TransactionalProducerConfig();

        config.Should().BeAssignableTo<ProducerConfig>();
        config.TransactionTimeoutMs.Should().Be(60000);
        config.TransactionalId.Should().BeEmpty();
    }

    [Fact]
    public void Validate_RequiresTransactionalId()
    {
        var action = () => new TransactionalProducerConfig
        {
            BootstrapServers = ["localhost:9092"]
        }.Validate();

        action.Should().Throw<KafkaConfigException>().Which.OptionName.Should().Be(nameof(TransactionalProducerConfig.TransactionalId));
    }

    [Fact]
    public void Validate_RejectsDisabledIdempotence()
    {
        var config = new TransactionalProducerConfig
        {
            BootstrapServers = ["localhost:9092"],
            TransactionalId = "orders-worker",
            EnableIdempotence = false
        };

        var action = () => config.Validate();

        action.Should().Throw<KafkaConfigException>().Which.OptionName.Should().Be(nameof(TransactionalProducerConfig.EnableIdempotence));
    }

    [Fact]
    public void Validate_RejectsAcksOtherThanAll()
    {
        var config = new TransactionalProducerConfig
        {
            BootstrapServers = ["localhost:9092"],
            TransactionalId = "orders-worker",
            Acks = Acks.Leader
        };

        var action = () => config.Validate();

        action.Should().Throw<KafkaConfigException>().Which.OptionName.Should().Be(nameof(TransactionalProducerConfig.Acks));
    }
}
