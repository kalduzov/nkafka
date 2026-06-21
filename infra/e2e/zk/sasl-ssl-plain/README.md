# ZK sasl-ssl-plain E2E profile

This profile bootstraps a single-broker Kafka environment for the $(System.Collections.Hashtable.Topology) topology with the $(System.Collections.Hashtable.Security) security profile.

## Notes

- SSL keystore material is generated automatically by the launch scripts into `./certs`.

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

