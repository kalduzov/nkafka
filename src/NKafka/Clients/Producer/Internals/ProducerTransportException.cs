using NKafka.Exceptions;
using NKafka.Protocol;

namespace NKafka.Clients.Producer.Internals;

internal sealed class ProducerTransportException(PersistenceStatus status)
    : ProducerException("The producer could not determine whether the record reached the broker.")
{
    internal PersistenceStatus Status { get; } = status;
    internal ProducerError Error { get; } = new(ErrorCodes.ClientError, ProducerLocalError.TransportFailure);
}
