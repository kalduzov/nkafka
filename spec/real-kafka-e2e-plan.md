# Real Kafka E2E Plan

## Purpose

This document defines how `NKafka` should be validated against real Kafka clusters with:

- different Kafka versions
- different topology modes
- different security settings
- real end-to-end broker interaction instead of mock-stream-only verification

The goal is to make `E2E` coverage explicit, reproducible and extendable.

## Current state

The repository already contains useful infrastructure fragments:

- [infra/docker-compose.yml](K:\nkafka\infra\docker-compose.yml) for ZooKeeper-based plaintext clusters
- [infra/docker-compose.big-cluster.yml](K:\nkafka\infra\docker-compose.big-cluster.yml) for large ZooKeeper-based plaintext clusters
- [infra/docker-compose.simple.yml](K:\nkafka\infra\docker-compose.simple.yml) for KRaft plaintext
- [infra/docker-compose.kraft.sasl.yml](K:\nkafka\infra\docker-compose.kraft.sasl.yml) for KRaft with `SASL/PLAIN`
- [tests/NKafka.IntegrationTests](K:\nkafka\tests\NKafka.IntegrationTests) for real-broker integration tests

But the current setup is still incomplete for a full `E2E` strategy:

- compose files are examples, not a formal matrix
- version coverage is not expressed as a supported test matrix
- security scenarios are partially represented, but not organized into reusable profiles
- integration tests are still mostly admin-oriented smoke tests
- `E2E` orchestration is environment-driven, but not yet modeled as a first-class test matrix

## Target outcomes

After this plan is implemented, the project should have:

1. A formal Kafka environment matrix.
2. Reproducible compose profiles for real broker runs.
3. `E2E` test entrypoints that can connect to those profiles.
4. Version-aware verification for the Kafka versions that `NKafka` claims to support.
5. Clear separation between:
   - unit tests
   - focused integration tests
   - real Kafka `E2E` tests

## Environment matrix

### Topology axis

At minimum the `E2E` matrix should distinguish:

- `zk-plaintext`
  ZooKeeper-based brokers, plaintext listeners.
- `kraft-plaintext`
  KRaft-based brokers, plaintext listeners.
- `kraft-sasl-plain`
  KRaft-based brokers with `SASL/PLAIN`.
- `kraft-sasl-scram256`
  KRaft-based brokers with `SASL/SCRAM-SHA-256`.
- `kraft-sasl-scram512`
  KRaft-based brokers with `SASL/SCRAM-SHA-512`.
- `ssl`
  Broker profile with encrypted client transport once SSL client path is ready for broker-backed verification.

`OAUTHBEARER` must stay outside the supported `E2E` matrix until the runtime provider path is implemented honestly.

### Version axis

Kafka version support must be expressed explicitly instead of relying on floating images like `latest`.

The matrix should be built around pinned broker versions, for example:

- `3.7.x`
- `3.8.x`
- `3.9.x`
- `4.x` when the project is ready to claim it

If Confluent images are used for some profiles and Apache images for others, the matrix must record both:

- Kafka broker version
- container distribution
- compose/profile compatibility notes

### Security axis

The current honest runtime matrix for `NKafka` is:

- `PLAINTEXT`
- `SASL/PLAIN`
- `SASL/SCRAM-SHA-256`
- `SASL/SCRAM-SHA-512`

Planned later:

- `SSL`
- `SASL_SSL`

Not yet supported:

- `OAUTHBEARER`
- `Kerberos/GSSAPI`

## Test responsibilities

### What focused integration tests continue to cover

Focused integration tests may keep covering:

- connector lifecycle
- negotiation logic
- request/response races
- mock-driven auth branches

### What real Kafka E2E tests must cover

Real Kafka `E2E` tests should focus on broker-backed confidence:

- cluster open and metadata bootstrap
- `ApiVersions` negotiation against real brokers
- admin smoke operations
- producer send to real partitions
- consumer fetch from real brokers
- group coordination basics
- security handshake success paths for supported mechanisms
- version-dependent behavior where broker capabilities differ

## Implementation phases

### Phase 1. Normalize environment profiles

Create a formal profile model for real Kafka environments.

Deliverables:

- pinned environment profile names
- one profile = one topology + one security contract
- no `latest` tags in supported `E2E` profiles
- common environment variables for version, profile, bootstrap and auth inputs

Expected code/work:

- central `E2E` profile helper in test code
- documentation of required environment variables
- removal of accidental ambiguity between old `IntegrationTests` bootstrap and new `E2E` bootstrap

### Phase 2. Normalize compose assets

Rework `infra/` examples into a consistent runnable test matrix.

Deliverables:

- profile-oriented compose layout
- explicit version pinning
- documented listener/bootstrap ports
- clear security inputs per profile

Recommended target structure:

- `infra/e2e/zk-plaintext/`
- `infra/e2e/kraft-plaintext/`
- `infra/e2e/kraft-sasl-plain/`
- `infra/e2e/kraft-sasl-scram256/`
- `infra/e2e/kraft-sasl-scram512/`

Each profile should include:

- compose file
- `.env` or version override file
- setup notes
- if needed, JAAS / SCRAM init scripts

### Phase 3. Add broker-backed E2E smoke tests

Introduce a dedicated `E2E` layer in tests for real-cluster connectivity.

Initial smoke tests should verify:

- cluster describe
- metadata refresh
- create/list/delete topic
- produce one batch
- consume one batch

Security-enabled profiles should at minimum verify:

- cluster open succeeds
- metadata path succeeds
- one admin call succeeds

### Phase 4. Add version-aware execution

The same `E2E` test body should be runnable against multiple broker versions.

Version support should be driven by:

- environment variables
- profile selection
- pinned image tags

At this stage the harness should support commands like:

- run `kraft-plaintext` on `3.7.x`
- run `kraft-sasl-plain` on `3.8.x`
- run `kraft-sasl-scram256` on `3.9.x`

### Phase 5. Expand from smoke tests to client workflows

After the bootstrap matrix is stable, add broader `E2E` flows:

- producer ordering/basic delivery
- consumer subscribe/poll/commit basics
- coordinator lookup and heartbeat behavior
- reconnect/restart scenarios
- unsupported-security regression checks

## Recommended test matrix

### Minimum required matrix

This is the smallest matrix that gives meaningful confidence:

| Profile | Kafka versions | Purpose |
|---|---|---|
| `zk-plaintext` | one pinned legacy-compatible version | keep classic metadata/bootstrap path honest |
| `kraft-plaintext` | two pinned modern versions | validate modern broker topology path |
| `kraft-sasl-plain` | one pinned modern version | validate supported SASL baseline |
| `kraft-sasl-scram256` | one pinned modern version | validate SCRAM runtime path |
| `kraft-sasl-scram512` | one pinned modern version | validate second SCRAM branch |

### Extended matrix

As the harness matures, the matrix can grow with:

- `SASL_SSL`
- SSL-only client transport
- broker restart / rolling bounce scenarios
- version-upgrade compatibility checks

## Environment contract for tests

The `E2E` layer should use dedicated environment variables instead of reusing the generic integration ones forever.

Recommended contract:

- `NKAFKA_E2E_ENABLED=true`
- `NKAFKA_E2E_PROFILE=<profile-name>`
- `NKAFKA_E2E_KAFKA_VERSION=<version>`
- `NKAFKA_E2E_BOOTSTRAP_SERVERS=<host:port,...>`
- `NKAFKA_E2E_SECURITY_PROTOCOL=<PlainText|SaslPlaintext|SaslSsl|Ssl>`
- `NKAFKA_E2E_SASL_MECHANISM=<Plain|ScramSha256|ScramSha512>`
- `NKAFKA_E2E_SASL_USERNAME=<user>`
- `NKAFKA_E2E_SASL_PASSWORD=<password>`
- `NKAFKA_E2E_TRUST_SERVER_CERTIFICATE=<true|false>`

The test harness should fail fast when a selected profile is underconfigured.

## CI strategy

The project should not run the full `E2E` matrix on every ordinary test invocation.

Recommended CI layers:

1. `unit + focused integration`
   Runs on every PR.
2. `E2E smoke`
   Runs on selected profiles, at least plaintext + one security profile.
3. `extended matrix`
   Runs nightly or on-demand for multiple Kafka versions and security combinations.

## Non-goals

This plan does not require:

- immediate implementation of `OAUTHBEARER`
- immediate implementation of `Kerberos/GSSAPI`
- replacing all current `IntegrationTests` at once
- full transaction/consumer workflow coverage in the first `E2E` iteration

## Success criteria

This plan can be considered successfully implemented when:

- real Kafka profiles are version-pinned and documented
- `E2E` tests can connect to multiple broker setups through one test harness
- supported runtime security mechanisms have real broker-backed smoke coverage
- the repository can express which Kafka versions and profiles are actually verified, not just theoretically supported
