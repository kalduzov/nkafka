# Client lifetime

[Русский](../ru/client-lifetime.md)

## Status

This page describes a planned common disposal setting. The current implementation does not yet provide this contract. The setting is being specified alongside [KIP-98](../../spec/KIP/KIP-98/vision.md).

## Common disposal deadline

A separate option in `CommonConfig` will limit disposal of producers, transactional producers, consumers, and administrative clients. The proposed name is `ClientDisposeTimeoutMs`, with a proposed default of 5000 ms and a positive integer range. These details remain subject to approval.

Producers and consumers use their effective inherited configuration. The current administrative client has no separate `AdminConfig` and uses the cluster configuration. Configuration copying and merging must preserve this option. Common validation must reject invalid values before client initialization.

The deadline belongs to one client's disposal operation, not to the total shutdown of an entire cluster. New operations are rejected when disposal begins. Nested cleanup and repeated or concurrent disposal calls do not restart the deadline.

## Behavior by client

| Client | Work covered by the deadline |
|---|---|
| Producer | Completion of accepted sends and stopping its own background operations |
| Transactional producer | Permitted transaction completion and stopping its own operations; disposal never automatically commits |
| Consumer | Stopping reads and group-exit actions already required by its contract; this option does not introduce automatic offset commits |
| Administrative client | Completion or termination of waiting for its own requests; a broker-side operation may still complete |

The setting applies to supported disposal paths. Introducing it does not by itself remove existing consumer or admin disposal methods.

## Expiration and resources

At expiration, stop broker waiting, complete pending operations with an error or unknown outcome, and perform local cleanup without another broker-wait period. A disposed client cannot resume work. Shared connections must not be closed to dispose one client. Dedicated resources may be released according to ownership and the remaining deadline.

The implementation must not wait indefinitely for a blocked operation or user callback. Runtime scheduling is not a hard real-time guarantee. Unknown delivery, transaction, or administrative outcomes must not be reported as successful cancellation on the broker. The exact public error contract remains under design.

## Other timeouts

`CloseConnectionTimeoutMs` controls physical connection closure. `RequestTimeoutMs` limits a request, `DeliveryTimeoutMs` limits message delivery, and `TransactionTimeoutMs` governs the transaction lifetime. None replaces the common client disposal deadline. Nested operations use the remaining disposal time instead of adding their full individual timeouts.
