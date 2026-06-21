# ZK sasl-oauthbearer E2E profile

This profile bootstraps a single-broker Kafka environment for the $(System.Collections.Hashtable.Topology) topology with the $(System.Collections.Hashtable.Security) security profile.

## Notes

- The broker profile is real and selectable, but current `NKafka` runtime support is expected to stay a deterministic non-success path.

## Automated matrix run

PowerShell:

```powershell
.\run-api-versions-matrix.ps1
```

Shell:

```sh
./run-api-versions-matrix.sh
```

Both wrappers delegate to the shared E2E matrix launcher and select this profile explicitly.

