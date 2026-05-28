# Minimal Client API Matrix for Fallback Kafka Versions

## Purpose

This document defines which Kafka APIs should be present in the manual fallback compatibility matrix in [src/NKafka/Protocol/SupportVersionsExtensions.cs](K:\nkafka\src\NKafka\Protocol\SupportVersionsExtensions.cs).

It is intended for cases where `NKafka` cannot rely on a live `ApiVersions` negotiation and must fall back to a predefined Kafka version baseline through `FallbackBrokerVersion`.

This document answers four questions for each API:

- is the API already present in the fallback matrix
- is it required for a minimal client library baseline
- what current runtime or public API depends on it
- what priority it should have for inclusion

## Scope

This matrix is focused on client-side APIs only:

- producer
- consumer
- group coordination
- security/authentication
- admin APIs that are already public in `NKafka` or required for a practical client baseline

Out of scope:

- broker-only APIs
- controller-only APIs
- KRaft internals
- telemetry-only APIs
- share-group and queue APIs that are not yet part of the minimal baseline

## Current baseline

The current fallback matrix in [src/NKafka/Protocol/SupportVersionsExtensions.cs](K:\nkafka\src\NKafka\Protocol\SupportVersionsExtensions.cs:53) already includes the following core APIs:

- `AddOffsetsToTxn`
- `AddPartitionsToTxn`
- `ApiVersions`
- `CreateTopics`
- `EndTxn`
- `Fetch`
- `FetchSnapshot`
- `FindCoordinator`
- `Heartbeat`
- `JoinGroup`
- `LeaveGroup`
- `ListOffsets`
- `Metadata`
- `OffsetCommit`
- `OffsetFetch`
- `Produce`
- `SaslAuthenticate`
- `SaslHandshake`
- `SyncGroup`

That set covers the basic request path, classic consumer groups, and part of the transactional producer flow.

## Decision rules

An API should be added to the fallback matrix when at least one of the following is true:

- it is required for the minimal producer or consumer runtime path
- it is required for transactional or idempotent producer behavior already exposed by the library
- it is part of a public `AdminClient` API already exposed by `NKafka`
- it is required to support the intended modern Kafka client baseline for `4.x`

An API may stay out of the minimal fallback matrix when:

- it is not used by the current runtime
- it belongs to optional admin expansion only
- it belongs to a future protocol family not yet declared as baseline

## Matrix

| API | Area | In current fallback matrix | Should be in minimal fallback matrix | Priority | Why it matters | Current status in repository |
|---|---|---|---|---|---|---|
| `InitProducerId` | Producer | No | Yes | P0 | Required for idempotent and transactional producer initialization | Runtime path exists in [Producer.Transaction.cs](K:\nkafka\src\NKafka\Clients\Producer\Producer.Transaction.cs:32) and [TransactionManager.cs](K:\nkafka\src\NKafka\Clients\Producer\Internals\TransactionManager.cs:72) |
| `TxnOffsetCommit` | Producer/Transactions | No | Yes | P0 | Required for `SendOffsetsToTransactionAsync()` and full transactional offsets flow | Generated messages exist; flow is called out as missing in [code-gaps.md](K:\nkafka\spec\code-gaps.md:48) |
| `OffsetForLeaderEpoch` | Consumer | No | Yes | P0 | Required for log truncation handling and modern fetch recovery semantics | Generated messages exist; needed for `KIP-320` class of behavior |
| `DeleteTopics` | Admin | No | Yes | P0 | Already exposed as public admin operation and implemented in runtime | Implemented in [AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs:111) |
| `DescribeGroups` | Consumer/Admin | No | Yes | P0 | Required for group inspection and practical admin/client diagnostics | Generated messages exist; not part of fallback matrix yet |
| `ListGroups` | Consumer/Admin | No | Yes | P0 | Required for group discovery and basic admin inspection | Generated messages exist; not part of fallback matrix yet |
| `DeleteGroups` | Admin | No | Yes | P0 | Required for modern group administration baseline | Public/admin-level relevance confirmed in [code-map.md](K:\nkafka\spec\code-map.md:320) |
| `DescribeCluster` | Admin | No | Yes | P1 | Already exposed by `AdminClient`, should participate in fallback behavior | Implemented in [AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs:185) |
| `DescribeConfigs` | Admin | No | Yes | P1 | Public admin API already exists and should be consistent with fallback compatibility behavior | Public API exists, runtime currently stubbed in [AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs:241) |
| `AlterConfigs` | Admin | No | Yes | P1 | Public admin API exists; needed for legacy config mutation support | Public API exists, runtime currently stubbed in [AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs:255) |
| `IncrementalAlterConfigs` | Admin | No | Yes | P1 | Public admin API exists; this is the modern config mutation path | Public API exists, runtime currently stubbed in [AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs:270) |
| `DescribeAcls` | Admin/Security | No | Yes | P1 | Public admin API exists for ACL inspection | Public API exists, runtime currently stubbed in [AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs:203) |
| `CreateAcls` | Admin/Security | No | Yes | P1 | Public admin API exists for ACL creation | Public API exists, runtime currently stubbed in [AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs:215) |
| `DeleteAcls` | Admin/Security | No | Yes | P1 | Public admin API exists for ACL deletion | Public API exists, runtime currently stubbed in [AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs:228) |
| `ConsumerGroupHeartbeat` | Consumer | No | Yes | P1 | Required for the modern consumer group protocol in Kafka `4.x` | Messages and KIP references exist; runtime integration is still incomplete per [code-gaps.md](K:\nkafka\spec\code-gaps.md:55) |
| `ConsumerGroupDescribe` | Consumer/Admin | No | Yes | P1 | Useful companion API for the modern consumer group protocol and group diagnostics | Messages exist; not yet part of minimal fallback matrix |
| `DescribeTopicPartitions` | Admin/Metadata | No | Maybe | P2 | Useful for modern topic introspection, but not required for the smallest baseline | Nice-to-have extension for admin completeness |
| `CreatePartitions` | Admin | No | Maybe | P2 | Useful admin operation, but not required for minimal producer/consumer viability | Messages exist; not currently part of runtime-critical path |
| `DeleteRecords` | Admin | No | Maybe | P2 | Useful for maintenance/admin scenarios, but not part of core client behavior | Messages exist; optional admin expansion |
| `ListConfigResources` | Admin | No | Maybe | P2 | Useful for modern config discovery, but not required for minimal baseline | Messages and `ApiKeys` alias exist, but no runtime path yet |
| `AlterUserScramCredentials` | Security/Admin | No | Maybe | P2 | Useful only if user SCRAM administration becomes part of supported admin scope | Generated messages exist; not part of current minimal client flow |
| `DescribeUserScramCredentials` | Security/Admin | No | Maybe | P2 | Useful only if user SCRAM administration becomes part of supported admin scope | Generated messages exist; not part of current minimal client flow |
| `ShareGroupHeartbeat` | Consumer | No | No | P3 | Part of share-group/queue protocol family, not part of declared minimal baseline | Future-facing API, not required for current client baseline |
| `ShareGroupDescribe` | Consumer/Admin | No | No | P3 | Same as above | Future-facing API, not required for current client baseline |

## Recommended minimal additions

The highest-value additions for the fallback matrix are:

1. `InitProducerId`
2. `TxnOffsetCommit`
3. `OffsetForLeaderEpoch`
4. `DeleteTopics`
5. `DescribeGroups`
6. `ListGroups`
7. `DeleteGroups`

These are the smallest set that closes the most visible mismatch between:

- current public/runtime client surface
- generated protocol support
- fallback compatibility behavior

## Recommended second wave

The next group should be added once `AdminClient` and modern Kafka `4.x` support are treated as baseline, not as roadmap-only:

1. `DescribeCluster`
2. `DescribeConfigs`
3. `AlterConfigs`
4. `IncrementalAlterConfigs`
5. `DescribeAcls`
6. `CreateAcls`
7. `DeleteAcls`
8. `ConsumerGroupHeartbeat`
9. `ConsumerGroupDescribe`

## Notes for implementation

- The fallback matrix should not include APIs only because message contracts exist.
- Inclusion should follow actual client scope and public surface area.
- When an API is added to the minimal matrix, unit tests should validate that every supported Kafka baseline has an entry for that API.
- If an API is exposed publicly but intentionally excluded from the fallback matrix, that decision should be documented explicitly.
