# KRaft plaintext profile

This is the first normalized broker profile for real `E2E` runs.

Purpose:

- bootstrap a real Kafka broker with `kraft + plaintext`
- provide a deterministic target for the first `ApiVersions` smoke scenario

## Start

1. Copy `.env.example` to `.env`.
2. Run:

```powershell
docker compose up -d
```

## Test environment contract

Use the same Kafka version in the broker profile and in the `E2E` test inputs:

```powershell
$env:NKAFKA_E2E_ENABLED="true"
$env:NKAFKA_E2E_TOPOLOGY_MODE="kraft"
$env:NKAFKA_E2E_SECURITY_PROFILE="plaintext"
$env:NKAFKA_E2E_KAFKA_VERSION="3.9.1"
$env:NKAFKA_E2E_BOOTSTRAP_SERVERS="localhost:29092"
```

Then run a version-specific `ApiVersions` scenario, for example:

```powershell
dotnet test tests/integration/NKafka.IntegrationTests.Kafka_3_9/NKafka.IntegrationTests.Kafka_3_9.csproj --no-restore -f net9.0 --filter "FullyQualifiedName~ApiVersionsE2ETests"
```

## Run the three-version matrix automatically

The profile also contains a PowerShell automation script that:

- starts the broker for each selected Kafka version
- waits until the advertised host port is reachable from the test runner
- runs the matching version-specific `E2E` assembly
- tears the environment down before moving to the next version

```powershell
.\run-api-versions-matrix.ps1
```

Unix-like environments can run the same matrix through:

```sh
./run-api-versions-matrix.sh
```
