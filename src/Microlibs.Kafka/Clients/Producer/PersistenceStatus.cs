namespace Microlibs.Kafka.Clients.Producer;

public enum PersistenceStatus
{
    /// <summary>
    ///     Message was never transmitted to the broker, or failed with
    ///     an error indicating it was not written to the log.
    ///     Application retry risks ordering, but not duplication.
    /// </summary>
    NotPersisted = 0,

    /// <summary>
    ///     Message was transmitted to broker, but no acknowledgement was
    ///     received. Application retry risks ordering and duplication.
    /// </summary>
    PossiblyPersisted = 1,

    /// <summary>
    ///     Message was written to the log and acknowledged by the broker.
    ///     Note: acks='all' should be used for this to be fully trusted
    ///     in case of a broker failover.
    /// </summary>
    Persisted = 2
}