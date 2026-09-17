// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

namespace NKafka.Clients.Producer;

/// <summary>
/// Adapts the internal producer transaction operations to the explicit transactional producer contract.
/// </summary>
internal sealed class TransactionalProducer(Producer producer, Action releaseTransactionalId) : ITransactionalProducer
{
    private int _disposed;

    public ValueTask<IProducerTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        producer.BeginTransaction();

        return ValueTask.FromResult<IProducerTransaction>(new ProducerTransaction(producer));
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        try
        {
            await producer.DisposeAsync();
        }
        finally
        {
            releaseTransactionalId();
        }
    }
}
