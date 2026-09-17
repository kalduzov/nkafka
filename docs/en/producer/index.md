# Producers

[Русский](../../ru/producer/index.md)

## Transactional interface status

The transactional interface is being designed as part of [KIP-98](../../../spec/KIP/KIP-98/README.md). This page describes the target contract and its usage rules. The example uses proposed methods and is not yet a runnable example for the current library version.

## Transactional configuration

The planned `TransactionalProducerConfig` inherits from `ProducerConfig`, preserving the configuration hierarchy used by the library. Common send settings are inherited; `TransactionalId` and `TransactionTimeoutMs` belong to the transactional configuration.

The transactional configuration sets valid initial values, including `EnableIdempotence = true` and `Acks = Acks.All`. Inherited properties remain writable, but incompatible user changes cause `KafkaConfigException` during validation, before network initialization. The exception identifies the invalid option; the library does not silently correct it.

The validator runs the base configuration checks and then the transactional checks on the final configuration. Direct creation and every extension overload follow the same validation path, including configurations supplied through a configuration callback. The copy API for deriving configurations from a shared base is still being specified in KIP-98.

When an overload receives an explicit ID and a configuration, an unset ID (`null` or an empty string) is filled from the argument, and an identical ID is accepted. A different non-empty ID causes `KafkaConfigException` before ID reservation or network initialization. Comparison is ordinal and case-sensitive, without trimming whitespace. The supplied configuration is not modified. The same check runs after a configuration callback; the explicit argument must itself be a valid non-blank ID.

## Duplicate transactional IDs

Within one `IKafkaCluster`, creating a second active transactional producer with the same `TransactionalId` is prohibited. The ID is reserved atomically after validation and before network initialization. A duplicate creation attempt, including a concurrent one, throws an exception without initializing a second producer or changing the first instance. It does not return an existing producer.

The reservation remains in place between transactions and during shutdown. Failed or cancelled creation releases the ID only after partial resources have been cleaned up; normal disposal releases it only after shutdown has fully completed. A failed producer still occupies its ID until it is disposed. Dispose the previous instance before creating its replacement.

This check is local to one cluster object. Another cluster object or process can use the same ID and fence the previous producer at the broker.

## Transactional producer lifetime

**Do not register a transactional producer as a shared `singleton` in a dependency injection container. Manage its creation, use, and asynchronous disposal explicitly in code.** This rule applies to any container that supplies one instance to multiple independent callers.

A transactional producer can have only one unfinished transaction. A `singleton` registration does not serialize calls or create separate transactional state for each caller. Even support for concurrent sends within one transaction does not imply support for multiple independent transactions on one producer.

Sharing the instance introduces these problems:

- **Competing handlers.** While one handler is running a transaction, another cannot start its own and receives a state error. Retrying without coordination does not resolve ownership.
- **Shared failure scope.** Fencing by another instance with the same `TransactionalId`, a fatal error, or an unknown completion outcome can make the instance unusable for all callers.
- **Conflicting disposal.** One caller may close an instance that others still use. If everyone relies solely on container disposal, active processing may not finish in coordination with producer and cluster shutdown.
- **Difficult recovery.** After replacing a failed instance, previously obtained references still point to the old one. A container does not automatically move handlers safely to a replacement.
- **No isolation across processes.** A `singleton` registration applies within its container. Another process using the same `TransactionalId` can fence this producer at the broker.

A separate `IProducerTransaction` makes message membership explicit: the library must not silently add an independent handler's sends to someone else's transaction. The main risks of a shared producer are conflicting control and shared failures, not supported independent transactions.

## Recommended ownership

Assign one owner to an instance, such as a sequential background handler. The owner creates the producer, executes transactions sequentially, and disposes it after processing ends. On failure, it stops using the instance and explicitly manages recovery.

A long-lived producer is valid; a new producer is not required for every message or transaction. The restriction concerns giving the same instance to independent callers through a container. The container may manage the background handler itself, while the handler's code owns its transactional producer.

Independent concurrent handlers need separate producers with different `TransactionalId` values. Changing a `singleton` registration to another lifetime does not solve the problem if the new instances receive the same ID.

```csharp
// cluster is already open and outlives the producer.
await using var producer = await cluster.CreateTransactionalProducerAsync(
    new TransactionalProducerConfig
    {
        TransactionalId = "orders-worker-0"
    },
    cancellationToken);

await foreach (var message in messages.WithCancellation(cancellationToken))
{
    await using var transaction =
        await producer.BeginTransactionAsync(cancellationToken);

    await transaction.ProduceAsync("orders", message, cancellationToken);
    await transaction.CommitAsync(cancellationToken);
}
```

Here, `messages` is a stream of messages prepared by the application; this example does not cover committing Kafka consumer offsets. The producer is created once for its owner's lifetime, while transaction objects are created sequentially within the loop.

During shutdown, stop accepting new work, complete the active transaction according to its state, dispose the producer, and only then close the cluster. A commit timeout does not guarantee an abort: do not automatically repeat the business operation or switch to abort when the commit outcome is unknown. Error handling and bounded disposal are being specified in the [transaction design overview](../../../spec/KIP/KIP-98/vision.md).

## Asynchronous disposal and timeout

The planned producer API uses only `DisposeAsync`, without synchronous `IDisposable` or a separate cancellable `CloseAsync`. A new common setting, provisionally named `ClientDisposeTimeoutMs`, limits client disposal. Its proposed default is 5000 ms; the name, default, and valid range still need approval. This is a planned contract, not an implemented setting.

Disposal stops accepting new operations and uses one deadline for completing work and stopping the client's own background operations. Requests, retries, and nested transaction disposal do not receive a fresh full timeout. Disposing an active transaction separately uses the same setting from its owner's configuration.

At the deadline, the client stops waiting for the broker, completes pending operations with an error or unknown outcome, and performs necessary local cleanup without another broker-wait period. It cannot resume normal operation. Runtime scheduling prevents a hard real-time timing guarantee, but blocked work must not turn disposal into an indefinite wait.

Local disposal does not guarantee that Kafka aborted a transaction or that messages were never written. Shared cluster connections remain available to other clients. `CloseConnectionTimeoutMs` continues to govern physical connection closure, and `TransactionTimeoutMs` has a separate purpose. See [client lifetime](../client-lifetime.md) for the common producer, consumer, and admin contract. The exact timeout exception and preservation of an existing application exception remain under design.
