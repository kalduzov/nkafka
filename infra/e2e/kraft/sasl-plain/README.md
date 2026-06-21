# KRAFT sasl-plain E2E profile

This profile bootstraps a single-broker Kafka environment for the $(System.Collections.Hashtable.Topology) topology with the $(System.Collections.Hashtable.Security) security profile.

## Notes

- This profile is meant to stay close to the default real-broker smoke path.

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

