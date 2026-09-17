// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Clients.Producer;
using NKafka.Config;
using NKafka.Exceptions;

namespace NKafka.Tests.Clients.Producer;

public sealed class TransactionalProducerExtensionsTests
{
    [Fact]
    public async Task ExplicitId_WithEmptyConfigId_FillsCopyWithoutChangingInput()
    {
        var cluster = Substitute.For<IKafkaCluster>();
        var producer = Substitute.For<ITransactionalProducer>();
        cluster.CreateTransactionalProducerAsync(
                Arg.Any<TransactionalProducerConfig>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(producer));
        var config = new TransactionalProducerConfig();

        var result = await cluster.CreateTransactionalProducerAsync(
            "orders-worker",
            config,
            TestContext.Current.CancellationToken);

        result.Should().BeSameAs(producer);
        config.TransactionalId.Should().BeEmpty();
        await cluster.Received(1).CreateTransactionalProducerAsync(
            Arg.Is<TransactionalProducerConfig>(value => value.TransactionalId == "orders-worker"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void ExplicitId_WithDifferentConfigId_ThrowsBeforeCallingCluster()
    {
        var cluster = Substitute.For<IKafkaCluster>();
        var config = new TransactionalProducerConfig { TransactionalId = "configured-id" };

        Action action = () => _ = cluster.CreateTransactionalProducerAsync(
            "explicit-id",
            config,
            TestContext.Current.CancellationToken);

        action.Should().Throw<KafkaConfigException>()
            .Which.OptionName.Should().Be(nameof(TransactionalProducerConfig.TransactionalId));
        cluster.DidNotReceive().CreateTransactionalProducerAsync(
            Arg.Any<TransactionalProducerConfig>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public void ConfigureCallback_WithDifferentConfigId_ThrowsBeforeCallingCluster()
    {
        var cluster = Substitute.For<IKafkaCluster>();

        Action action = () => _ = cluster.CreateTransactionalProducerAsync(
                "explicit-id",
                value => value.TransactionalId = "configured-id",
                TestContext.Current.CancellationToken);

        action.Should().Throw<KafkaConfigException>();
        cluster.DidNotReceive().CreateTransactionalProducerAsync(
            Arg.Any<TransactionalProducerConfig>(),
            Arg.Any<CancellationToken>());
    }
}
