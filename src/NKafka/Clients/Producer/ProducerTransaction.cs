// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Clients.Consumer;

namespace NKafka.Clients.Producer;

/// <summary>
/// Adapts the existing producer transaction operations to the explicit transaction contract.
/// </summary>
internal sealed class ProducerTransaction(Producer producer) : IProducerTransaction
{
    public Task<MessageDeliveryResult> ProduceAsync(
        TopicPartition topicPartition,
        Message message,
        CancellationToken cancellationToken = default)
    {
        return producer.ProduceAsync(topicPartition, message, cancellationToken);
    }

    public Task SendOffsetsAsync(
        IReadOnlyCollection<TopicPartitionOffset> offsets,
        ConsumerGroupMetadata groupMetadata,
        CancellationToken cancellationToken = default)
    {
        return producer.SendOffsetsToTransactionAsync(offsets, groupMetadata, cancellationToken);
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return producer.CommitTransactionAsync(cancellationToken);
    }

    public Task AbortAsync(CancellationToken cancellationToken = default)
    {
        return producer.AbortTransactionAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
