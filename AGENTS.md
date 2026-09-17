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

## Языки документации библиотеки

- Английский — базовый язык документации в `docs/en`; русский перевод находится в `docs/ru`.
- Для всех языков обязательны одинаковые относительные пути файлов, порядок документов и структура разделов. Примеры и описания контрактов должны соответствовать друг другу.
- Новая или изменённая документация добавляется сразу на обоих языках в рамках одного изменения.
- `docs/index.md` служит общей страницей выбора языка. Внутренние ссылки ведут на выбранный язык, если перевод доступен; страницы содержат ссылку на свой перевод.
- При добавлении или перемещении страниц обновляются ссылки и `NKafka.slnx` для обеих языковых папок.
- Эти правила относятся к документации библиотеки в `docs/`. Спецификации проектирования в `spec/` сохраняют отдельную структуру.
