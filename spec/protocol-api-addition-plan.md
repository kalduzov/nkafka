# Protocol API Addition Plan

## Purpose

This document fixes the implementation plan for the current stage of protocol work.

The goal of this stage is to add missing Kafka client-side APIs to the library as known protocol entities without changing business logic that uses them.

This is a protocol-awareness stage, not a runtime-behavior stage.

## Stage goal

At the end of this stage, the library must:

- know about the selected missing Kafka APIs
- have message specs and generated contracts for them
- include them in the fallback compatibility matrix where required
- reflect their presence in project specifications

At the same time, the library must not:

- start using the new APIs in producer, consumer, or admin workflows
- change existing state machines
- change retry logic
- change orchestration logic
- change observable business behavior of existing public methods

## Definition of “API added”

An API is considered added for this stage when all relevant items below are completed:

1. The API exists in [src/NKafka/Protocol/ApiKeys.cs](K:\nkafka\src\NKafka\Protocol\ApiKeys.cs), if applicable.
2. The API has message specs in `resources/message`.
3. The API has generated message contracts, builders, and generated tests.
4. The API is added to [src/NKafka/Protocol/SupportVersionsExtensions.cs](K:\nkafka\src\NKafka\Protocol\SupportVersionsExtensions.cs) if it must participate in `FallbackBrokerVersion`.
5. The API is reflected in the relevant spec documents with correct readiness status.

## Out of scope

The following work is explicitly out of scope for this stage:

1. Runtime wiring of new APIs into existing producer flows.
2. Runtime wiring of new APIs into existing consumer flows.
3. Runtime wiring of new APIs into existing admin flows.
4. Transaction state machine changes.
5. Consumer rebalance behavior changes.
6. Retry and recovery policy changes.
7. New integration scenarios that require behavior changes to pass.

## Source documents

This plan must be used together with:

- [Base spec](K:\nkafka\spec\index.md)
- [Technical requirements](K:\nkafka\spec\technical-requirements.md)
- [Protocol interaction spec](K:\nkafka\spec\protocol-interaction.md)
- [Minimal client API matrix](K:\nkafka\spec\protocol-minimal-client-api-matrix.md)
- [Code map](K:\nkafka\spec\code-map.md)
- [Code gaps](K:\nkafka\spec\code-gaps.md)

## Work waves

### Wave A. Canonical API inventory

Goal:

- confirm the exact list of missing APIs for this stage

Tasks:

1. Use [spec/protocol-minimal-client-api-matrix.md](K:\nkafka\spec\protocol-minimal-client-api-matrix.md) as the canonical API list.
2. Split selected APIs into:
   - `protocol awareness + fallback matrix required`
   - `protocol awareness only`
3. Freeze the list for the current stage before editing code.

Output:

- agreed list of APIs for the stage

### Wave B. Protocol declarations

Goal:

- ensure the protocol layer can represent the selected APIs

Tasks:

1. Review [src/NKafka/Protocol/ApiKeys.cs](K:\nkafka\src\NKafka\Protocol\ApiKeys.cs).
2. Add missing API keys if the protocol enum does not yet contain them.
3. Keep aliases or renames consistent with the current Kafka baseline where needed.
4. Ensure protocol builders can reference these APIs through generated code.

Output:

- protocol layer knows the APIs by identifier

### Wave C. Message contracts

Goal:

- ensure the library has generated protocol artifacts for the selected APIs

Tasks:

1. Verify that request and response JSON specs exist in `resources/message`.
2. Sync missing specs from the reference Kafka baseline when required.
3. Run message generation.
4. Run generated-test generation.
5. Do not manually edit generated `.g.cs` files.

Output:

- message contracts, builders, and generated tests exist for the selected APIs

### Wave D. Fallback compatibility knowledge

Goal:

- make `FallbackBrokerVersion` aware of the selected APIs when appropriate

Tasks:

1. For APIs that belong to the minimal fallback baseline, update [src/NKafka/Protocol/SupportVersionsExtensions.cs](K:\nkafka\src\NKafka\Protocol\SupportVersionsExtensions.cs).
2. Use `validVersions` from the corresponding Kafka branch request JSON specs as the source of truth.
3. Do not add APIs to the fallback matrix only because generated messages exist.
4. Update [tests/NKafka.Tests/Protocol/SupportVersionsExtensionsTests.cs](K:\nkafka\tests\NKafka.Tests\Protocol\SupportVersionsExtensionsTests.cs) and related assertions if required.

Output:

- fallback compatibility matrix is aware of the selected APIs

### Wave E. Public surface awareness

Goal:

- align protocol knowledge and public surface without changing behavior

Tasks:

1. Review public/admin/client surfaces that already expose related capabilities.
2. Avoid changing method behavior or orchestration.
3. If an API is publicly exposed but runtime support is incomplete, preserve current behavior and document the readiness gap in specs.

Output:

- public surface remains stable while protocol knowledge expands

### Wave F. Spec updates

Goal:

- keep documentation aligned with protocol readiness

Tasks:

1. Update [spec/protocol-minimal-client-api-matrix.md](K:\nkafka\spec\protocol-minimal-client-api-matrix.md) with readiness status for newly added APIs.
2. Update [spec/code-gaps.md](K:\nkafka\spec\code-gaps.md) when a gap changes from `API absent` to `API present but not runtime wired`.
3. Update [spec/code-map.md](K:\nkafka\spec\code-map.md) if protocol awareness materially changes subsystem maturity.

Output:

- specifications correctly distinguish protocol presence from runtime completeness

### Wave G. Verification

Goal:

- verify that protocol-awareness changes are internally consistent

Tasks:

1. Run `dotnet build`.
2. Run generated/message-level tests as needed.
3. Run targeted tests for `SupportVersionsExtensions`.
4. Do not require new runtime behavior tests in this stage when behavior has not been changed.

Output:

- build and protocol-level verification pass

## Recommended API order

The recommended order for this stage is:

1. `InitProducerId`
2. `TxnOffsetCommit`
3. `OffsetForLeaderEpoch`
4. `DeleteTopics`
5. `DescribeGroups`
6. `ListGroups`
7. `DeleteGroups`
8. `DescribeCluster`
9. `DescribeConfigs`
10. `AlterConfigs`
11. `IncrementalAlterConfigs`
12. `DescribeAcls`
13. `CreateAcls`
14. `DeleteAcls`
15. `ConsumerGroupHeartbeat`
16. `ConsumerGroupDescribe`

This order prioritizes:

- protocol-critical producer and consumer APIs first
- then already exposed admin APIs
- then modern Kafka `4.x` group APIs

## Completion criteria

The stage is complete when:

1. All selected APIs are present in the protocol layer of the library.
2. All selected APIs have message specs and generated contracts.
3. All selected APIs that belong to the agreed fallback baseline are present in `SupportVersionsExtensions`.
4. Relevant tests are updated.
5. Specifications are updated.
6. No existing business logic has been intentionally changed.

## Readiness model

For this stage, each API should be classified using the following readiness states:

- `Absent`: the library does not know the API yet
- `Protocol-known`: the library knows the API and has protocol contracts
- `Fallback-known`: the API is also represented in the fallback compatibility matrix
- `Runtime-integrated`: the API is actively used by business logic

The target state for this stage is:

- `Protocol-known` for all selected APIs
- `Fallback-known` for selected APIs that belong to the minimal fallback baseline

The target state is not:

- `Runtime-integrated`
