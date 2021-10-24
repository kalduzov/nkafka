# For developers

## Dependencies on other clients 

The code has a dependency on the underlying kafka client on Java.
This is necessary to get the description of the contracts that are saved in the client's source code as `json` files.

Based on these files, the built-in tool generates message contracts and serialization/deserialization methods for the protocol.

The base branch and version of the Java client for this repository is the **3.4** branch.

> If changes are made to the contract generation algorithms, it is necessary to regenerate all messages and, if necessary, tests for these messages.

## Deploying Infrastructure

The **infra** folder contains a set of tools for deploying **kafka** to **docker** to test the client.

> At the moment, only `docker-compose.yml` and `docker-compose.big-cluster.yml` files are fully functional. The rest of the deployment options need to be improved.

