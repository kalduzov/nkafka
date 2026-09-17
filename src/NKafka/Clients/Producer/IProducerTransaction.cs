// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

using NKafka.Clients.Consumer;

namespace NKafka.Clients.Producer;

/// <summary>
/// Represents one explicit Kafka transaction.
/// </summary>
/// <remarks>
/// Successful message delivery does not confirm the transaction. The transaction is confirmed only
/// by <see cref="CommitAsync"/> or cancelled by <see cref="AbortAsync"/>. The object is owned by the
/// caller and must be disposed asynchronously.
/// </remarks>
public interface IProducerTransaction : IAsyncDisposable
{
    /// <summary>
    /// Sends a message as part of this transaction.
    /// </summary>
    /// <param name="topicPartition">The destination topic and partition.</param>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">Token used to cancel waiting for the operation.</param>
    /// <returns>A task with the broker delivery result for the message.</returns>
    public Task<MessageDeliveryResult> ProduceAsync(
        TopicPartition topicPartition,
        Message message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Includes the next offsets of processed records in this transaction.
    /// </summary>
    /// <param name="offsets">The next offsets after the records that were processed.</param>
    /// <param name="groupMetadata">The current metadata of the consumer group.</param>
    /// <param name="cancellationToken">Token used to cancel waiting for the operation.</param>
    /// <returns>A task that completes when the offsets are accepted by the transaction.</returns>
    public Task SendOffsetsAsync(
        IReadOnlyCollection<TopicPartitionOffset> offsets,
        ConsumerGroupMetadata groupMetadata,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms all records and offsets accepted by this transaction.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel waiting for confirmation.</param>
    /// <returns>A task that completes when the broker confirms the transaction.</returns>
    public Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels this transaction.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel waiting for cancellation.</param>
    /// <returns>A task that completes when the broker confirms cancellation.</returns>
    public Task AbortAsync(CancellationToken cancellationToken = default);
}
