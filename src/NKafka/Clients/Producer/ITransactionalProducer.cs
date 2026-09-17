// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace NKafka.Clients.Producer;

/// <summary>
/// Provides sequential transactions for one transactional producer.
/// </summary>
/// <remarks>
/// A transactional producer is owned by the caller and must not be registered as a shared singleton.
/// Only one transaction may be active for an instance at a time. Messages are sent through the
/// returned <see cref="IProducerTransaction"/> and are not sent directly by this interface.
/// </remarks>
public interface ITransactionalProducer : IAsyncDisposable
{
    /// <summary>
    /// Starts a new transaction owned by this producer.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel initialization of the transaction.</param>
    /// <returns>A transaction object that must be disposed by its owner.</returns>
    /// <exception cref="ObjectDisposedException">The producer has already been disposed.</exception>
    public ValueTask<IProducerTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default);
}
