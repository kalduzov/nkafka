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
- [tests/integration](K:\nkafka\tests\integration) for real-broker integration tests

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

The matrix must distinguish topology mode explicitly instead of encoding it only inside combined profile names.

At minimum the environment matrix should support:

- `zk`
  ZooKeeper-based broker installation.
- `kraft`
  KRaft-based broker installation.

This distinction matters independently from authentication because the client must be validated against both:

- classic ZooKeeper-backed metadata/bootstrap environments
- modern KRaft-based metadata/bootstrap environments

The topology axis must therefore remain a first-class test dimension even when the same security profile is reused across both modes.

The topology matrix must describe real broker installations even when the client does not yet support every auth path as a positive runtime scenario.
`OAUTHBEARER` therefore belongs to the infrastructure matrix, but not yet to the positive `NKafka` runtime-success matrix until its provider path is implemented honestly.

### Version axis

Kafka version support must be expressed explicitly instead of relying on floating images like `latest`.

The harness must support running against any Kafka version that `NKafka` claims to support, not only against a short hardcoded list of example releases.

The version axis should therefore be dynamic:

- the selected Kafka version comes from the `E2E` environment contract
- compose assets must accept a pinned version override instead of baking one fixed version per profile
- the supported version set must follow the versions the client declares as supported in specs and compatibility code

Pinned examples such as `3.7.x`, `3.8.x`, `3.9.x` or `4.x` are useful as validation baselines, but they must not become the only runnable options.

If Confluent images are used for some profiles and Apache images for others, the matrix must record both:

- Kafka broker version
- container distribution
- compose/profile compatibility notes

### Security axis

The purpose of this plan is to define the broker environment matrix that the repository must be able to bootstrap, not only the subset that the client already supports successfully.

The target environment model is therefore a cartesian combination of:

- topology mode
- Kafka version
- security profile
- effective API capability floor

Combined names like `kraft-sasl-scram256` may still exist as convenient shortcuts in scripts or CI labels, but the underlying plan should treat them as composed values, not as the primary model.

At the infrastructure level, the matrix should be able to bootstrap all of these security profiles:

- `PLAINTEXT`
- `SSL`
- `SASL/PLAIN`
- `SASL/SCRAM-SHA-256`
- `SASL/SCRAM-SHA-512`
- `SASL/OAUTHBEARER`
- `SASL_SSL/PLAIN`
- `SASL_SSL/SCRAM-SHA-256`
- `SASL_SSL/SCRAM-SHA-512`
- `SASL_SSL/OAUTHBEARER`

This infrastructure target is intentionally broader than the current positive-runtime matrix of `NKafka`.

The current honest positive runtime matrix for `NKafka` is:

- `PLAINTEXT`
- `SASL/PLAIN`
- `SASL/SCRAM-SHA-256`
- `SASL/SCRAM-SHA-512`

Not yet supported:

- `SSL`
- `SASL_SSL`
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
- mixed-version compatibility where the effective API surface is lower than the physical broker version

### Current implementation status

The repository now has the first dedicated broker-backed `ApiVersions` scenario in the versioned `E2E` test assemblies.
That scenario intentionally checks only the negotiation baseline:

- a real cluster can be opened
- the aggregated cluster metadata contains `ApiVersions`
- the capability snapshot is populated before broader admin or producer/consumer smoke flows are required

The repository also now contains the first normalized infrastructure profile at:

- [infra/e2e/kraft/plaintext/docker-compose.yml](K:\nkafka\infra\e2e\kraft\plaintext\docker-compose.yml)

This profile is the starting point for the broader topology/security matrix and is meant to anchor the first real `ApiVersions` run.

## Implementation phases

### Phase 1. Normalize environment profiles

Create a formal profile model for real Kafka environments.

Deliverables:

- explicit topology mode selection
- explicit security profile selection
- dynamic Kafka version selection
- ability to bootstrap every topology/security combination in the target infrastructure matrix
- no `latest` tags in supported `E2E` profiles
- common environment variables for version, profile, bootstrap and auth inputs

Expected code/work:

- central `E2E` profile helper in test code
- documentation of required environment variables
- removal of accidental ambiguity between old `IntegrationTests` bootstrap and new `E2E` bootstrap

### Phase 1a. Split E2E test assemblies by Kafka version

The `E2E` layer should be split into separate test assemblies per Kafka version line instead of keeping every version scenario in one project forever.

The reason is practical:

- version-targeted runs become easier to select in CI
- per-version skips and exclusions stay local to the relevant broker line
- version-specific setup differences do not leak into one giant conditional test assembly

Recommended structure:

- `tests/integration/NKafka.IntegrationTests.Shared/`
- `tests/integration/NKafka.IntegrationTests.Kafka_3_7/`
- `tests/integration/NKafka.IntegrationTests.Kafka_3_8/`
- `tests/integration/NKafka.IntegrationTests.Kafka_3_9/`
- additional version-specific projects for every other Kafka version the client claims to support

The versioned assemblies should stay thin.
They should contain:

- version selection
- version-specific includes/excludes
- version-specific bootstrap details when required

Reusable code should be shared when reasonable, for example through:

- a shared test helper project
- linked/common fixtures
- shared scenario base classes
- shared environment/bootstrap contracts

The repository should avoid duplicating the same smoke flow N times just because it runs against N Kafka versions.

### Phase 1b. Add API-floor scenario assemblies

Physical broker version and effective client-visible API surface are not the same thing.

In real Kafka environments, especially during rolling upgrades, a cluster may contain newer brokers while the effective API compatibility seen by the client still has to respect lower broker capabilities.

The `E2E` strategy must therefore cover both:

- physical Kafka version lines
- forced lower API floors on top of those physical versions

Recommended model:

- version-specific assemblies describe the physical broker line
- additional scenario assemblies or test groups describe the forced API floor

Examples:

- `Kafka_3_9` physical broker line with default capabilities
- `Kafka_3_9.ApiFloor_3_7`
- `Kafka_3_9.ApiFloor_3_8`

These scenarios should verify that the client chooses the lowest commonly supported API version instead of assuming that the highest broker version in the cluster defines the usable request version.

The upgrade focus here must stay on minor-version transitions inside one major Kafka line.
Scenarios like `2.x -> 3.x` or `3.x -> 4.x` should not be treated as ordinary rolling-upgrade E2E coverage because those transitions usually involve broader infrastructure migration steps rather than simple broker-by-broker updates.

### Phase 2. Normalize compose assets

Rework `infra/` examples into a consistent runnable test matrix.

Deliverables:

- profile-oriented compose layout
- explicit version pinning
- documented listener/bootstrap ports
- clear security inputs per profile

Recommended target structure:

- `infra/e2e/zk/plaintext/`
- `infra/e2e/zk/ssl/`
- `infra/e2e/zk/sasl-plain/`
- `infra/e2e/zk/sasl-scram256/`
- `infra/e2e/zk/sasl-scram512/`
- `infra/e2e/zk/sasl-oauthbearer/`
- `infra/e2e/zk/sasl-ssl-plain/`
- `infra/e2e/zk/sasl-ssl-scram256/`
- `infra/e2e/zk/sasl-ssl-scram512/`
- `infra/e2e/zk/sasl-ssl-oauthbearer/`
- `infra/e2e/kraft/plaintext/`
- `infra/e2e/kraft/ssl/`
- `infra/e2e/kraft/sasl-plain/`
- `infra/e2e/kraft/sasl-scram256/`
- `infra/e2e/kraft/sasl-scram512/`
- `infra/e2e/kraft/sasl-oauthbearer/`
- `infra/e2e/kraft/sasl-ssl-plain/`
- `infra/e2e/kraft/sasl-ssl-scram256/`
- `infra/e2e/kraft/sasl-ssl-scram512/`
- `infra/e2e/kraft/sasl-ssl-oauthbearer/`

Each profile should include:

- compose file
- `.env` or version override file
- setup notes
- trust material generation or mounted certificate assets
- if needed, JAAS / SCRAM / token bootstrap scripts

### Phase 3. Add broker-backed E2E smoke tests

Introduce a dedicated `E2E` layer in tests for real-cluster connectivity.

Initial smoke tests should verify:

- cluster describe
- metadata refresh
- create/list/delete topic
- produce one batch
- consume one batch

These smoke scenarios should live primarily in shared code and be invoked from the version-specific test assemblies, unless a scenario is truly version-bound.

Security-enabled profiles should at minimum verify:

- cluster open succeeds
- metadata path succeeds
- one admin call succeeds

Profiles that are present in the broker matrix but not yet supported by the client runtime should still have explicit negative-path coverage:

- the installation can be selected intentionally
- the test harness recognizes the profile explicitly
- the failure mode is honest and deterministic instead of being treated as an unknown environment

SSL-enabled profiles should additionally verify that:

- certificate material is provisioned reproducibly
- bootstrap endpoints expose the intended secure listener
- the test harness can opt into trust behavior explicitly instead of relying on hidden defaults

### Phase 4. Add version-aware execution

The same `E2E` test body should be runnable against multiple broker versions.

Version support should be driven by:

- environment variables
- topology selection
- security profile selection
- pinned image tags
- one selected Kafka version per run
- version-specific test assembly selection
- optional forced API-floor selection

At this stage the harness should support commands like:

- run `zk + plaintext` on any supported legacy-compatible version
- run `zk + ssl` on any supported version where the secure profile exists
- run `kraft + plaintext` on any supported modern version
- run `kraft + sasl-plain` on any supported version where the profile is available
- run `kraft + sasl-scram256` on any supported version where the profile is available
- run `kraft + sasl-ssl-scram256` on any supported version where the secure profile is available
- run `kraft + sasl-oauthbearer` on any supported version as a tracked unsupported-runtime profile
- run a newer physical broker line with a lower forced API floor, for example `Kafka 3.9 physical + API floor 3.7`

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

| Topology | Security | Kafka versions | Purpose |
|---|---|---|---|
| `zk` | `PLAINTEXT` | dynamically selected from supported client versions | keep classic metadata/bootstrap path honest |
| `zk` | `SSL` | dynamically selected from supported client versions | keep secure classic deployment bootstrap reproducible |
| `kraft` | `PLAINTEXT` | dynamically selected from supported client versions | validate modern broker topology path |
| `kraft` | `SASL/PLAIN` | dynamically selected from supported client versions | validate supported SASL baseline |
| `kraft` | `SASL/SCRAM-SHA-256` | dynamically selected from supported client versions | validate SCRAM runtime path |
| `kraft` | `SASL/SCRAM-SHA-512` | dynamically selected from supported client versions | validate second SCRAM branch |
| `kraft` | `SASL/OAUTHBEARER` | dynamically selected from supported client versions | keep the broker installation in the matrix and verify honest unsupported-runtime behavior until client support exists |
| `kraft` | `SASL_SSL/SCRAM-SHA-256` | dynamically selected from supported client versions | keep secure SCRAM installation reproducible and ready for client validation |

### API-floor matrix

In addition to the physical broker matrix, the repository should keep explicit scenarios where the effective API capability floor is lower than the physical broker version.

At minimum, this matrix should include:

| Physical broker line | Forced API floor | Purpose |
|---|---|---|
| latest supported modern line | previous supported line | verify lowest-common-version selection during rolling upgrades |
| latest supported modern line | oldest still-supported line | verify compatibility against the lowest supported API surface |

These scenarios do not replace mixed-broker integration later on, but they do give a deterministic first layer of verification for the client's API version selection rules.

The intended rolling-upgrade coverage is specifically:

- `3.7 -> 3.8`
- `3.8 -> 3.9`
- similar minor-line transitions inside the same supported major line

The plan should not require ordinary rolling-upgrade scenarios for:

- `2.x -> 3.x`
- `3.x -> 4.x`

Those major-line transitions may still need separate validation later, but as infrastructure migration scenarios rather than standard broker upgrade tests.

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
- `NKAFKA_E2E_TOPOLOGY_MODE=<zk|kraft>`
- `NKAFKA_E2E_SECURITY_PROFILE=<plaintext|ssl|sasl-plain|sasl-scram256|sasl-scram512|sasl-oauthbearer|sasl-ssl-plain|sasl-ssl-scram256|sasl-ssl-scram512|sasl-ssl-oauthbearer>`
- `NKAFKA_E2E_KAFKA_VERSION=<version>`
- `NKAFKA_E2E_API_FLOOR_VERSION=<version|default>`
- `NKAFKA_E2E_BOOTSTRAP_SERVERS=<host:port,...>`
- `NKAFKA_E2E_SECURITY_PROTOCOL=<PlainText|SaslPlaintext|SaslSsl|Ssl>`
- `NKAFKA_E2E_SASL_MECHANISM=<Plain|ScramSha256|ScramSha512|OAuthBearer>`
- `NKAFKA_E2E_SASL_USERNAME=<user>`
- `NKAFKA_E2E_SASL_PASSWORD=<password>`
- `NKAFKA_E2E_TRUST_SERVER_CERTIFICATE=<true|false>`

The test harness should fail fast when a selected profile is underconfigured.

If the repository keeps shortcut names for convenience, they should be derived from the explicit topology/security pair rather than replacing it.

If `NKAFKA_E2E_API_FLOOR_VERSION` is set, the selected environment must expose a deterministic lower API capability surface for the client, even when the physical broker image is newer.

## CI strategy

The project should not run the full `E2E` matrix on every ordinary test invocation.

Recommended CI layers:

1. `unit + focused integration`
   Runs on every PR.
2. `E2E smoke per version`
   Runs selected version-specific assemblies against selected profiles, at least plaintext + one security profile.
3. `extended matrix`
   Runs nightly or on-demand for multiple Kafka versions, security combinations, and forced API-floor scenarios.

## Non-goals

This plan does not require:

- immediate implementation of `OAUTHBEARER` runtime support
- immediate implementation of `SSL` positive-runtime support
- immediate implementation of `SASL_SSL` positive-runtime support
- immediate implementation of `Kerberos/GSSAPI`
- replacing all current `IntegrationTests` at once
- full transaction/consumer workflow coverage in the first `E2E` iteration

## Success criteria

This plan can be considered successfully implemented when:

- real Kafka profiles are version-pinned and documented
- real Kafka infrastructure can be bootstrapped for every topology/security combination in the target matrix
- `E2E` tests can connect to multiple broker setups through one shared harness with version-specific assemblies on top
- supported runtime security mechanisms have real broker-backed smoke coverage
- unsupported broker-side profiles such as `OAUTHBEARER` remain visible in the environment matrix and have explicit deterministic handling
- the repository can verify that the client chooses the lowest commonly supported API version when the effective API floor is lower than the physical broker version
- the repository can express which Kafka versions and profiles are actually verified, not just theoretically supported
