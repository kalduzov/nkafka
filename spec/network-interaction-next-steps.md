# Plan: Further Changes to NKafka Network Interaction Layer

Based on audit of current code vs. spec targets in `network-interaction.md` and `network-layer-decoupling-plan.md`.

## Current State Summary

### Done (Steps 2-4 of decoupling plan)
- Explicit pool methods added: `TryGetSharedConnector`, `TryCreateDedicatedConnector`, `TryGetAnySharedBrokerConnector`, `TryGetBootstrapConnector`, `GetOpenedSharedConnectors`
- `KafkaCluster` migrated to explicit routing: `SendAsync(nodeId)` uses `TryGetSharedConnector`, service routing uses `controller → any broker → bootstrap` path, `MergeAllVersions` uses `GetOpenedSharedConnectors`
- Renamed `ProvideDedicateConnector` → `ProvideDedicatedConnector`

### Not Done — requires action

---

## Phase 1. Complete explicit contract migration

### 1.1 Update tests to new methods (Step 5)

| File | What |
|---|---|
| `tests/NKafka.Tests/Connection/KafkaConnectorPoolTests.cs` | Replace `GetConnector()` → `TryGetAnySharedBrokerConnector` / `TryGetBootstrapConnector`; replace `TryGetConnector(id, false, ...)` → `TryGetSharedConnector(id, ...)`; replace `TryGetConnector(id, true, ...)` → `TryCreateDedicatedConnector(id, ...)` |
| `tests/NKafka.Tests/ClusterTests.cs` | Update mocked `IKafkaConnectorPool` setup from `TryGetConnector(-1, false, ...)` to new methods |

### 1.2 Remove old ambiguous methods (Step 6)

- Remove `IKafkaConnectorPool.GetConnector()`
- Remove `IKafkaConnectorPool.TryGetConnector(int nodeId, bool isDedicated, ...)`
- Remove `IKafkaConnectorPool.GetAllOpenedConnectors()`
- Remove implementations in `KafkaConnectorPool`
- Verify no call sites remain in `KafkaCluster`, `Consumer`, `Producer`, `Admin`

### 1.3 Cluster helper cleanup (Step 7)

Add private helpers to `KafkaCluster`:

- `HasUsableBrokerTopology` — `_nodes.Count > 0 && Brokers.Count > 0`
- `HasKnownController` — `_controllerId != Node.NoNode.Id`
- `GetConnectorForBootstrapOrAnyBroker()` — explicit switch between bootstrap phase and any-shared-broker phase
- `GetConnectorForKnownBroker(int nodeId)` — centralize `TryGetSharedConnector` + throw

---

## Phase 2. Connection state machine (Wave 2)

### 2.1 Formalize connector state model

Current enum: `Open`, `Closed`, `Closing` (3 states)
Target: `Closed → Connecting → Negotiating → Authenticating → Open → Faulted → Closing` (7 states)

- Update `KafkaConnector.State` enum
- Define allowed transitions with explicit validation
- Define side effects per transition (invalidate inflight, reset `SupportVersions`, dispose stream)
- Unit tests for all transitions

### 2.2 Centralize invalidation behavior

- On `Closed`/`Faulted` → fail all pending in `_inFlightRequests`
- On reconnect → clear `SupportVersions` before new negotiation
- On protocol-level stream corruption → `Faulted`, not silent reconnect

### 2.3 Distinguish failure categories

| Failure type | State transition | Recovery |
|---|---|---|
| Socket connect error | → Closed/Faulted | Higher-layer retry |
| SSL handshake failure | → Faulted | No retry in connector |
| ApiVersions failure | → Closed | Reconnect allowed |
| SASL auth failure | → Faulted | No retry in connector |
| Correlation mismatch | → Faulted | Stream alignment lost |
| Malformed response | → Faulted | Stream alignment lost |
| Remote disconnect | → Closed | Reconnect allowed |

---

## Phase 3. Session establishment extraction (Wave 2 continued)

### 3.1 Separate setup from steady-state

Currently `ReEstablishConnectionAsync()` in `KafkaConnector` handles: socket connect → SSL → ApiVersions → SASL → mark Open. Meanwhile `SendAsync()` also calls `ReEstablishConnectionAsync()` on each request.

Target:

```
KafkaConnector
├── SessionSetup (logical or physical component)
│   ├── TcpConnect
│   ├── SslHandshake
│   ├── ApiVersionsNegotiation
│   ├── SaslAuthentication
│   └── PublishSupportVersions
├── SendPath (steady-state)
│   ├── CorrelationId assignment
│   ├── Inflight registration
│   ├── Frame write
│   └── Timeout handling
└── ResponsePath (steady-state)
    ├── Single-reader loop
    ├── Frame read + correlation
    ├── Response deserialization
    └── Task completion
```

### 3.2 SCRAM integration point

- Wire existing SCRAM provider into `Auth.` flow
- Currently only `PLAIN` and `OAUTHBEARER` are reachable in runtime auth switch
- SCRAM classes exist in `Connection/Sasl/` but not connected

---

## Phase 4. Steady-state request/response cleanup (Wave 4)

### 4.1 Inflight invariants

Current `_inFlightRequests` is `ConcurrentDictionary<int, ResponseTaskCompletionSource>`.
Formalize:

- One `CorrelationId` = one pending request (already true)
- Request timeout completes pending task deterministically (verify cancellation path)
- Failed write removes from inflight (verify exception handling in `SendAsync`)
- Connection shutdown completes/fails all pending predictably (verify `ResetConnection` → inflight cleanup)

### 4.2 Single-reader response loop

Current `ResponseReaderTask` + `ParseResponseAsync` creates concurrent parse tasks.
Verify:

- Stream reads remain single-threaded (already — only `ResponseReaderTask` reads)
- Buffer ownership is unambiguous (check buffer pool return timing)
- `_responsesTasks` tracking is needed or can be simplified
- Out-of-order completion across correlation IDs is safe (should be by design)

### 4.3 Reframe response-processing TODO

Current TODO suggests moving processing to pool — change to: "consider extraction to internal coordination component, not pool, preserving stream ownership in connector".

---

## Phase 5. Pool topology cleanup (Wave 3)

### 5.1 Seed vs broker lifecycle

- `_seedConnectors` is cleaned during operation but has thread-safety concern (existing TODO)
- Separate seed connector lifecycle from broker connector lifecycle explicitly
- `UpdateConnectors()` should not mutate seed connectors

### 5.2 Topology update vs connector creation

- `AddOrUpdateConnectorsAsync()` does: topology sync + connector open + dead cleanup
- Split logically: `SyncTopology(nodes)` → `OpenConnectors(nodes)` → `CleanupDeadConnectors()`

### 5.3 Move idle-close timer to pool

- `_closeConnectionAfterTimeout` and `ResetConnection()` in `KafkaConnector` embed policy in connector
- Move to `KafkaConnectorPool` as pool-level idle management policy

---

## Phase 6. Security hardening (Wave 6)

### 6.1 SCRAM production auth path

- Connect SCRAM provider in `KafkaConnector.Auth..cs` switch
- Add integration tests for SCRAM-SHA-256 and SCRAM-SHA-512

### 6.2 Config validation

- `SaslSettings` surface is broader than supported runtime
- Validate selected mechanism against actually wired providers at config time

---

## Phase 7. Testing (cross-cutting)

### 7.1 Unit tests

- Connector state transitions
- Inflight registration/removal and timeout
- Reconnect invalidation of `SupportVersions`
- Pool selection policy: seed/shared/dedicated

### 7.2 Integration tests

- Bootstrap + metadata + request flow
- SSL/SASL combinations
- Reconnect behavior
- Response correlation on broken stream

---

## Priority ordering

```
High (immediate)
├── Phase 1.1 (update tests to new methods)
├── Phase 1.2 (remove old ambiguous methods)
├── Phase 1.3 (cluster helper cleanup)
│
Medium (next)
├── Phase 2.1 (connection state machine)
├── Phase 2.2 (invalidation behavior)
├── Phase 5.3 (move idle timer to pool)
├── Phase 6.1 (SCRAM auth path)
│
Lower
├── Phase 3.1 (session setup extraction)
├── Phase 4 (steady-state cleanup)
├── Phase 5.1-5.2 (pool topology)
├── Phase 6.2 (config validation)
├── Phase 7 (tests)
```

Each phase must leave the code compilable and passing existing tests.
