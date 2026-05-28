# NKafka Project Information for AI Agents

This document provides essential information for AI agents working on the NKafka project.

## Entry Point

`AGENTS.md` is the primary entry point for any agent working in this repository.

Before making implementation decisions, an agent should build context in the following order:

1. Read this file first.
2. Read the base specification: [spec/index.md](K:\nkafka\spec\index.md).
3. Read the technical requirements: [spec/technical-requirements.md](K:\nkafka\spec\technical-requirements.md).
4. Read specialized documents from `spec/` as needed.
5. Read relevant source code, tests, and message specifications.
6. If needed, consult the official Kafka specifications and KIPs.

If there is ambiguity:

- `AGENTS.md` defines repository-level working rules and context order
- [spec/index.md](K:\nkafka\spec\index.md) defines the base client design principles
- [spec/technical-requirements.md](K:\nkafka\spec\technical-requirements.md) defines the project-level technical requirements
- specialized files in `spec/` refine topic-specific context
- code and tests remain the executable source of truth for actual behavior
