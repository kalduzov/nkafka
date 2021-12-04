using Microlibs.Kafka.Protocol;
using Microsoft.Extensions.Logging;

namespace Microlibs.Kafka.Clients.Producer.Internals;

public class TransactionManager
{
    public TransactionManager(
        ILoggerFactory loggerFactory,
        string? configTransactionalId,
        int configTransactionTimeoutMs,
        long configRetryBackoffMs,
        ApiVersions apiVersions)
    {
    }

    public bool IsTransactional { get; set; }

    public TransactionalRequestResult InitTransaction()
    {
        return null;
    }
}