# NKafka Real Kafka E2E Infrastructure

This folder contains normalized broker profiles for real `E2E` runs.

The first profile is intentionally narrow:

- topology: `kraft`
- security: `plaintext`
- purpose: bootstrap the first real-broker `ApiVersions` scenario

Each profile should stay version-pinned through environment variables instead of using floating `latest` tags.
