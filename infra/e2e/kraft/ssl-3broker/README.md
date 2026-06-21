# KRaft SSL 3-broker full environment

This profile bootstraps a three-broker KRaft cluster with external SSL-only connectivity.

The start scripts also ensure that the cluster contains one topic with the expected deterministic layout:

- topic name: configurable at launch time
- partitions: `9`
- replica factor: `3`
- leader distribution: `3` partitions per broker

## Start

PowerShell:

```powershell
.\start-environment.ps1 -TopicName test_topic
```

Shell:

```sh
./start-environment.sh test_topic
```

## Stop

PowerShell:

```powershell
.\stop-environment.ps1
```

Shell:

```sh
./stop-environment.sh
```

## Notes

- external broker ports are `29091`, `29092`, `29093`
- external connectivity is `SSL` only
- the scripts generate local keystore material into `./certs` when needed
- topic creation uses the internal plaintext listener inside the Docker network so that the external interface stays SSL-only
