# KRAFT sasl-ssl-scram512 E2E profile

This profile bootstraps a single-broker Kafka environment for the $(System.Collections.Hashtable.Topology) topology with the $(System.Collections.Hashtable.Security) security profile.

## Notes

- SSL keystore material is generated automatically by the launch scripts into `./certs`.
- SCRAM user credentials are bootstrapped automatically after the broker starts through the internal plaintext admin listener.

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

