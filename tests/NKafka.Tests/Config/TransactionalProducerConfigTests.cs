// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Config;

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
}
