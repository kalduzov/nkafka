// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Clients.Producer;

namespace NKafka.Tests.Clients.Producer;

public sealed class ProducerContractTests
{
    [Fact]
    public void OrdinaryProducer_DoesNotExposeTransactionOperations()
    {
        var methodNames = typeof(IProducer)
            .GetMethods()
            .Select(method => method.Name)
            .ToHashSet(StringComparer.Ordinal);

        methodNames.Should().NotContain("InitTransactionsAsync");
        methodNames.Should().NotContain("BeginTransaction");
        methodNames.Should().NotContain("CommitTransactionAsync");
        methodNames.Should().NotContain("AbortTransactionAsync");
        methodNames.Should().NotContain("SendOffsetsToTransactionAsync");
    }

    [Fact]
    public void TransactionContracts_ExposeOnlyExplicitTransactionOperations()
    {
        var producerMethodNames = typeof(ITransactionalProducer)
            .GetMethods()
            .Select(method => method.Name)
            .ToHashSet(StringComparer.Ordinal);
        var transactionMethodNames = typeof(IProducerTransaction)
            .GetMethods()
            .Select(method => method.Name)
            .ToHashSet(StringComparer.Ordinal);

        producerMethodNames.Should().Contain("BeginTransactionAsync");
        producerMethodNames.Should().NotContain("ProduceAsync");
        transactionMethodNames.Should().Contain("ProduceAsync");
        transactionMethodNames.Should().Contain("SendOffsetsAsync");
        transactionMethodNames.Should().Contain("CommitAsync");
        transactionMethodNames.Should().Contain("AbortAsync");
    }
}
