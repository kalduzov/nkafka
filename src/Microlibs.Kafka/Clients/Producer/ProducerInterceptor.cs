using System;

namespace Microlibs.Kafka.Clients.Producer;

/// <summary>
///     A plugin interface that allows you to intercept (and possibly mutate) the records received by the producer before
///     they are published to the Kafka cluster.
/// </summary>
public interface ProducerInterceptor<TKey, TValue>
{
    Message<TKey, TValue> OnProduce(Message<TKey, TValue> record);

    void OnAcknowledgement(RecordMetadata metadata, Exception exception);
}