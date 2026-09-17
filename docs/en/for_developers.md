# For developers

[Русский](../ru/for_developers.md)

## Dependencies on other clients

The code has a dependency on the underlying kafka client on Java.
This is necessary to get the description of the contracts that are saved in the client's source code as `json` files.

Based on these files, the built-in tool generates message contracts and serialization/deserialization methods for the protocol.

The base branch and version of the Java client for this repository is the **4.3** branch.

> If changes are made to the contract generation algorithms, it is necessary to regenerate all messages and, if necessary, tests for these messages.

## Deploying Infrastructure

The **infra** folder contains a set of tools for deploying **kafka** to **docker** to test the client.

> According to the current infrastructure description, only `docker-compose.yml` and `docker-compose.big-cluster.yml` files are fully functional. The rest of the deployment options need to be improved.

> To run **integration tests**, you will also need to deploy the `docker-compose.yml` file.

For the target real-broker matrix and the new `E2E` environment contract, see:

- [spec/real-kafka-e2e-plan.md](../../spec/real-kafka-e2e-plan.md)

## Documentation languages

English is the base language in `docs/en`; Russian is maintained in `docs/ru`. Both directories must contain the same relative file paths, document order, section structure, examples, and contract details. Create or update both versions in the same change. A missing translation is an incomplete documentation change.

Keep identifiers and filenames identical across languages. Translate explanations and headings, and keep code examples equivalent. Links within a language directory must stay in that language where a translation exists. Every page links to its counterpart. `docs/index.md` is the shared language selector.

Internal specifications in `spec/` remain separate from this library documentation and follow repository rules. Source XML comments remain in English; related usage guides and explanations must be available in both documentation languages.
