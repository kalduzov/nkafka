# Consumer Rebalance Notes from Ozon Tech Article

## Source

Notes based on the article:

- [Kafka: ребалансировка изнутри](https://habr.com/ru/companies/ozontech/articles/910568/)

Article date:

- 2025-05-22

## Purpose

This document captures implementation-relevant ideas from the article and maps them to the current `NKafka` codebase and specifications.

It is not a protocol source of truth.

It is a design note for future consumer/group coordination work.

## Short summary

The article describes consumer-group rebalancing from the client side and highlights the following points:

1. Group formation is fundamentally built around:
   - `FindCoordinator`
   - `JoinGroup`
   - leader-side assignment
   - `SyncGroup`
2. Rebalancing is asynchronous relative to message processing.
3. Because of that, the client needs a synchronization barrier before partitions are reassigned.
4. In eager protocols this usually becomes a stop-the-world operation.
5. In cooperative protocols the client tries to avoid immediate revoke and moves partitions in phases.
6. Even cooperative rebalance does not eliminate synchronization completely when revoke is required.

## Ideas relevant to NKafka

### 1. Rebalance is not just a new assignment

The article reinforces that rebalance must be modeled as a phased process, not as a single event where the client simply receives a new assignment.

Relevant implication for `NKafka`:

- consumer coordinator logic should explicitly model rebalance phases
- assignment application should be separated from revoke and cleanup

Related code:

- [src/NKafka/Clients/Consumer/Internal/Coordinator.cs](K:\nkafka\src\NKafka\Clients\Consumer\Internal\Coordinator.cs)

### 2. Rebalance barrier matters

The article highlights the importance of `rebalance_timeout` as a synchronization barrier.

Why it matters:

- a partition may still be processed by the old consumer while the group is already trying to move it
- without a clear barrier, ownership transfer becomes unsafe

Relevant implication for `NKafka`:

- `rebalance_timeout` should be treated as part of coordination semantics, not just as a passive config value
- stop/revoke/apply phases should be explicit in consumer coordinator behavior

Related spec:

- [spec/protocol-interaction.md](K:\nkafka\spec\protocol-interaction.md)

### 3. Eager protocols behave like stop-the-world

The article describes classic eager rebalance as a mode where participants stop processing, release ownership, and only then receive new work.

Relevant implication for `NKafka`:

- current classic consumer path should not assume seamless reassignment
- shutdown and rebalance handling must be explicit and conservative

Related gap:

- [spec/code-gaps.md](K:\nkafka\spec\code-gaps.md)

### 4. Cooperative rebalance requires owned-partitions thinking

The article explains that cooperative sticky-style behavior depends on the client knowing what it currently owns and sharing that information into rebalance decisions.

Relevant implication for `NKafka`:

- `ownedPartitions`-style semantics should be treated as first-class protocol/runtime data
- cooperative rebalance should be modeled as a phased transition, not as a small variation of eager rebalance

This is especially important for future work around:

- incremental rebalance
- minimal revoke
- low-disruption reassignment

### 5. Cooperative rebalance still has a synchronization cost

The article makes an important practical point: cooperative rebalance reduces disruption, but it does not remove the need for synchronization when partitions must actually move.

Relevant implication for `NKafka`:

- cooperative support should not be oversimplified as “no pauses”
- specs should describe both:
  - reduced disruption
  - remaining synchronization requirements

### 6. Rollout behavior can be counterintuitive

The article notes that under rolling deployments cooperative strategies may keep partitions on old consumers longer than expected in order to minimize movement.

Relevant implication for `NKafka`:

- future specs should distinguish between:
  - minimizing pauses
  - maximizing immediate rebalance fairness
- cooperative semantics may intentionally delay redistribution for stability

## Mapping to current NKafka state

### Consumer coordinator

The article directly reinforces the importance of the current gaps in:

- [src/NKafka/Clients/Consumer/Internal/Coordinator.cs](K:\nkafka\src\NKafka\Clients\Consumer\Internal\Coordinator.cs:180)

Especially relevant:

- `StopSessionAsync()` is still effectively empty
- `SyncGroup` error handling is incomplete
- rebalance stop/revoke/apply lifecycle is not yet described as a full state model

### Existing specs

These notes align with and strengthen:

- [spec/protocol-interaction.md](K:\nkafka\spec\protocol-interaction.md)
- [spec/code-gaps.md](K:\nkafka\spec\code-gaps.md)
- [spec/code-map.md](K:\nkafka\spec\code-map.md)

### Modern consumer protocol direction

These notes are also relevant for future work around:

- `KIP-429`
- `KIP-848`
- `ConsumerGroupHeartbeat`
- `ConsumerGroupDescribe`

The main design lesson is that protocol support alone is not enough.

`NKafka` also needs an explicit runtime model for:

- rebalance phases
- revoke timing
- assignment timing
- processing barriers
- session shutdown behavior

## Suggested follow-up changes to specs

When consumer/group work resumes, consider the following spec updates:

1. Add a dedicated subsection to [spec/protocol-interaction.md](K:\nkafka\spec\protocol-interaction.md) for:
   - rebalance barrier
   - `session_timeout` vs `rebalance_timeout`
   - revoke/apply lifecycle
2. Refine consumer entries in [spec/code-gaps.md](K:\nkafka\spec\code-gaps.md) using more precise terms:
   - `rebalance barrier handling`
   - `safe revoke before reassignment`
   - `cooperative phase handling`
3. Extend [spec/code-map.md](K:\nkafka\spec\code-map.md) to explicitly note that the current coordinator path is closer to classic/eager semantics than to a complete cooperative runtime model.

## Main takeaway

The main practical takeaway for `NKafka` is:

- consumer group coordination should be designed around explicit rebalance phases
- partition transfer should be modeled as a safe handoff process
- cooperative support should be treated as a real runtime model, not just as another assignment algorithm
