# ZooKeeper plaintext E2E profile

This profile bootstraps a single-broker Kafka cluster backed by a single ZooKeeper node.

It is intentionally minimal and is meant to validate the real-broker `ApiVersions` path for the classic `zk + plaintext` topology.

## Manual startup

Copy `.env.example` to `.env` when you want to pin a local version override:

```powershell
Copy-Item .env.example .env
```

Bring the profile up:

```powershell
docker compose up -d
```

## E2E environment contract

The matching `NKafka` E2E contract is:

```powershell
$env:NKAFKA_E2E_ENABLED="true"
$env:NKAFKA_E2E_TOPOLOGY_MODE="zk"
$env:NKAFKA_E2E_SECURITY_PROFILE="plaintext"
$env:NKAFKA_E2E_KAFKA_VERSION="3.9.1"
$env:NKAFKA_E2E_BOOTSTRAP_SERVERS="localhost:29092"
```

## Automated matrix run

PowerShell:

```powershell
.\run-api-versions-matrix.ps1
```

Shell:

```sh
./run-api-versions-matrix.sh
```

Both scripts run the real-broker `ApiVersions` E2E scenario against the current ZooKeeper plaintext topology for the default Kafka version set:

- `3.7.1`
- `3.8.0`
- `3.9.1`
