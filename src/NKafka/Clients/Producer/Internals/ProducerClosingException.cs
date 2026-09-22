using NKafka.Exceptions;

namespace NKafka.Clients.Producer.Internals;

internal sealed class ProducerClosingException(PersistenceStatus status) : ProducerException("Producer is closing.")
{
    internal PersistenceStatus Status { get; } = status;
}
