# NKafka Real Kafka E2E Infrastructure

This folder contains normalized broker profiles for real `E2E` runs.

The repository now has profile directories for the full current topology/security matrix:

- `kraft/plaintext`
- `kraft/ssl`
- `kraft/sasl-plain`
- `kraft/sasl-scram256`
- `kraft/sasl-scram512`
- `kraft/sasl-oauthbearer`
- `kraft/sasl-ssl-plain`
- `kraft/sasl-ssl-scram256`
- `kraft/sasl-ssl-scram512`
- `kraft/sasl-ssl-oauthbearer`
- `zk/plaintext`
- `zk/ssl`
- `zk/sasl-plain`
- `zk/sasl-scram256`
- `zk/sasl-scram512`
- `zk/sasl-oauthbearer`
- `zk/sasl-ssl-plain`
- `zk/sasl-ssl-scram256`
- `zk/sasl-ssl-scram512`
- `zk/sasl-ssl-oauthbearer`

Shared launch helpers live under [infra/e2e/scripts](K:\nkafka\infra\e2e\scripts).

A dedicated full environment for a three-broker `kraft + ssl` cluster also exists at:

- [infra/e2e/kraft/ssl-3broker](K:\nkafka\infra\e2e\kraft\ssl-3broker)

Each profile should stay version-pinned through environment variables instead of using floating `latest` tags.
