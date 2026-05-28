# Code Gaps NKafka

## Назначение документа

Этот файл фиксирует не общую карту кода, а конкретные пробелы и недоработанные участки, которые важны для агентной разработки.

Цель:

- превратить наблюдения из `code-map.md` в рабочий backlog
- дать агентам быстрый список мест, куда можно заходить с задачами
- отделить feature-ready части от каркасов и незавершённых веток

Связанные документы:

- [Code map](K:\nkafka\spec\code-map.md)
- [Base spec](K:\nkafka\spec\index.md)
- [Technical requirements](K:\nkafka\spec\technical-requirements.md)

## Как читать таблицу

Колонки:

- `Area` — крупная подсистема
- `File` — основной файл или каталог входа
- `Gap` — в чём именно пробел
- `Impact` — почему это важно
- `Priority` — насколько срочно стоит закрывать
- `Suggested next step` — минимальный разумный следующий шаг

Приоритеты:

- `P0` — блокирует современные клиентские сценарии или делает feature misleading
- `P1` — важная клиентская функциональность, но не стоппер для базового пути
- `P2` — желательная доработка, улучшение качества или расширение покрытия

## Feature backlog

| Area | File | Gap | Impact | Priority | Suggested next step |
|---|---|---|---|---|---|
| Cluster | [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs) | `GetOffset()` возвращает `Offset.Unset` без реальной реализации | Offset-related cluster API фактически не готов | P1 | Либо реализовать через `ListOffsets`, либо убрать misleading surface до появления полноценной реализации |
| Cluster | [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs) | Metadata lifecycle зависит от текущего локального состояния и частично смешивает orchestration с caching | Усложняет feature work и может скрывать race conditions | P2 | Выделить отдельно responsibilities: metadata fetch, topology cache, background refresh |
| Connection | [src/NKafka/Connection/KafkaConnector.Auth..cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.Auth..cs) | Runtime auth switch использует только `PLAIN` и `OAUTHBEARER` | SCRAM path подготовлен инфраструктурно, но не работает end-to-end | P0 | Добавить полноценный runtime path для SCRAM и закрыть его integration tests |
| Connection | [src/NKafka/Connection/NullConnector.cs](K:\nkafka\src\NKafka\Connection\NullConnector.cs) | Заглушка с `NotImplementedException` | Может вводить в заблуждение при рефакторинге и тестовых сценариях | P2 | Ограничить использование только test-helper контекстом или сделать safer null-object contract |
| Connection | [src/NKafka/Connection/KafkaConnector.cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.cs) | Response processing всё ещё частично живёт внутри connector | Труднее масштабировать и упростить concurrency model | P2 | Отдельно описать целевую модель и вынести обработку ответов ближе к pool/runtime orchestration |
| Producer | [src/NKafka/Clients/Producer/Producer.Transaction.cs](K:\nkafka\src\NKafka\Clients\Producer\Producer.Transaction.cs) | `ThrowIfNotTransactional()` сейчас всегда бросает исключение | Transactional producer API фактически unusable | P0 | Реализовать корректную проверку transactional mode вместо unconditional throw |
| Producer | [src/NKafka/Clients/Producer/Producer.Transaction.cs](K:\nkafka\src\NKafka\Clients\Producer\Producer.Transaction.cs) | `ThrowIfInvalidGroupMetadata()` пустой | Нет валидации critical input для transaction offsets flow | P1 | Добавить валидацию `ConsumerGroupMetadata` и тесты на invalid metadata |
| Producer | [src/NKafka/Clients/Producer/Internals/TransactionManager.cs](K:\nkafka\src\NKafka\Clients\Producer\Internals\TransactionManager.cs) | `InitializeTransactionsAsync()` содержит незавершённый `switch` по response code | Инициализация transactional state machine не завершена | P0 | Реализовать обработку кодов ошибок и переходы состояний |
| Producer | [src/NKafka/Clients/Producer/Internals/TransactionManager.cs](K:\nkafka\src\NKafka\Clients\Producer\Internals\TransactionManager.cs) | `SendOffsetsToTransactionAsync()` пока `Task.CompletedTask`, хотя `TxnOffsetCommit` уже добавлен как protocol/fallback-known API | Нет transactional offsets flow, несмотря на готовые protocol contracts | P0 | Реализовать AddOffsetsToTxn / TxnOffsetCommit path и покрыть тестами |
| Producer | [src/NKafka/Clients/Producer/Internals/TransactionManager.cs](K:\nkafka\src\NKafka\Clients\Producer\Internals\TransactionManager.cs) | `CommitAsync()` и `AbortAsync()` не реализованы | Нельзя завершить transaction корректно | P0 | Реализовать `EndTxn` flow и state transitions |
| Producer | [src/NKafka/Clients/Producer/Internals/TransactionManager.cs](K:\nkafka\src\NKafka\Clients\Producer\Internals\TransactionManager.cs) | `LookupCoordinatorAsync()` пустой | Нет coordinator recovery для transactional path | P1 | Реализовать `FindCoordinator(Transaction)` и retry/recovery semantics |
| Producer | [src/NKafka/Clients/Producer/Internals/TransactionManager.cs](K:\nkafka\src\NKafka\Clients\Producer\Internals\TransactionManager.cs) | `ResetSequenceNumbers()` и `BumpIdempotentProducerEpochAsync()` пустые | Не закрыт idempotent/epoch recovery branch | P1 | Реализовать sequence reset и epoch bump semantics |
| Producer | [src/NKafka/Clients/Producer/Producer.cs](K:\nkafka\src\NKafka\Clients\Producer\Producer.cs) | Close/flush/lifecycle path выглядит минималистично и не похож на завершённый graceful shutdown | Риск потери данных или неявного поведения при shutdown | P1 | Уточнить expected shutdown contract и закрепить тестами |
| Consumer | [src/NKafka/Clients/Consumer/Internal/Coordinator.cs](K:\nkafka\src\NKafka\Clients\Consumer\Internal\Coordinator.cs) | `StopSessionAsync()` фактически пустой | Session shutdown и leave/rebalance path не завершён | P0 | Реализовать корректный stop sequence: heartbeat stop, leave group, cleanup |
| Consumer | [src/NKafka/Clients/Consumer/Internal/Coordinator.cs](K:\nkafka\src\NKafka\Clients\Consumer\Internal\Coordinator.cs) | `SyncGroupAsync()` часть error cases завершает `NotSupportedException` | Rebalance error handling неполный | P0 | Закрыть все известные Kafka error branches и описать retry/rejoin semantics |
| Consumer | [src/NKafka/Clients/Consumer/Consumer.cs](K:\nkafka\src\NKafka\src\NKafka\Clients\Consumer\Consumer.cs) | `Assignment` выглядит как незавершённая публичная проекция текущего состояния | Клиент не получает надёжное отражение assignment state | P1 | Синхронизировать assignment state с coordinator/subscription model |
| Consumer | [src/NKafka/Clients/Consumer/Internal](K:\nkafka\src\NKafka\Clients\Consumer\Internal) | KIP-848 message support и fallback knowledge уже есть, но основной runtime path всё ещё classic-oriented | Новый consumer protocol пока не доведён до runtime feature level | P1 | Отдельно выделить task на KIP-848 runtime integration |
| Admin | [src/NKafka/Clients/Admin/AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs) | `DescribeAclsAsync()` возвращает пустой result, хотя API уже protocol/fallback-known | ACL admin API misleading | P0 | Реализовать request/response mapping для DescribeAcls |
| Admin | [src/NKafka/Clients/Admin/AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs) | `CreateAclsAsync()` возвращает пустой result, хотя API уже protocol/fallback-known | ACL create фактически отсутствует | P0 | Реализовать CreateAcls flow и tests |
| Admin | [src/NKafka/Clients/Admin/AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs) | `DeleteAclsAsync()` возвращает пустой result, хотя API уже protocol/fallback-known | ACL delete фактически отсутствует | P0 | Реализовать DeleteAcls flow и tests |
| Admin | [src/NKafka/Clients/Admin/AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs) | `DescribeConfigsAsync()` возвращает пустой result, хотя API уже protocol/fallback-known | Config admin API не готов | P1 | Реализовать DescribeConfigs request/response path |
| Admin | [src/NKafka/Clients/Admin/AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs) | `AlterConfigsAsync()` возвращает пустой result, хотя API уже protocol/fallback-known | Legacy config mutation API не реализован | P1 | Реализовать basic flow или явно пометить unsupported |
| Admin | [src/NKafka/Clients/Admin/AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs) | `IncrementalAlterConfigsAsync()` возвращает пустой result, хотя API уже protocol/fallback-known | Современный config mutation API не реализован | P1 | Реализовать incremental alter configs flow |
| Security | [src/NKafka/Connection/Sasl](K:\nkafka\src\NKafka\Connection\Sasl) | SCRAM primitives есть, но не встроены в production auth path | Неполная поддержка client-side security features | P0 | Связать SCRAM state machine с connector auth flow |
| Security | [src/NKafka/Config/SaslSettings.cs](K:\nkafka\src\NKafka\Config\SaslSettings.cs) | Конфигурационная поверхность шире, чем подтверждённая runtime-поддержка | Риск ложного ощущения поддерживаемых механизмов | P1 | Явно описать supported/unsupported combinations и закрепить validation |
| Diagnostics | [src/NKafka/Metrics](K:\nkafka\src\NKafka\Metrics) | Есть базовые metrics types, но нет явной feature-complete observability model | Сложнее развивать telemetry и client metrics KIPs | P2 | Сформировать отдельную telemetry spec и feature map |
| Tests | [tests/NKafka.IntegrationTests](K:\nkafka\tests\NKafka.IntegrationTests) | Integration coverage узкое и заметно смещено в admin scenarios | High-risk runtime features недопроверены end-to-end | P1 | Добавить integration tests для producer, consumer, transactions, security |
| Tests | [tests/NKafka.Tests](K:\nkafka\tests\NKafka.Tests) | Транзакционный producer path покрыт слабо относительно сложности | Риск регрессий в незавершённой state machine | P1 | Добавить focused unit tests на TransactionManager state transitions |
| KIP Runtime | [spec/KIP/client-side-kips-table.md](K:\nkafka\spec\KIP\client-side-kips-table.md) | Есть аналитика KIP-ов, но нет прямой трассировки `KIP -> files -> runtime readiness` | Труднее планировать работы по modern Kafka support | P2 | Завести отдельную feature traceability matrix по KIP-ам |

## Recommended implementation waves

### Wave 1

Закрыть misleading public surfaces, которые уже есть, но фактически не работают:

- transactional producer API
- ACL admin API
- config admin API
- consumer session stop / rebalance error handling
- SCRAM runtime auth

### Wave 2

Довести modern client capabilities:

- KIP-848 runtime integration
- idempotent/transaction recovery paths
- config/ACL admin completeness
- stronger integration coverage

### Wave 3

Улучшить maintainability and operability:

- connection/runtime refactoring
- observability model
- feature traceability matrix
- metadata orchestration cleanup

## Agent workflow recommendation

Если агент берёт задачу из этого файла, рекомендуемый порядок работы:

1. Открыть указанный `File`.
2. Сверить контекст с [code-map.md](K:\nkafka\spec\code-map.md).
3. Проверить связанные tests и message specs.
4. Реализовать минимально законченный vertical slice.
5. Обновить `spec/`, если статус участка изменился.
