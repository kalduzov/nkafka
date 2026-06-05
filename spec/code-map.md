# Карта кода NKafka

## Назначение документа

Этот документ нужен как рабочая карта кодовой базы для разработки через агентов.

Его задача:

- показать основные участки кода и их границы ответственности
- дать быстрый вход в нужные подсистемы
- фиксировать примерную зрелость участков
- разделять зоны: уже доработано, частично доработано, не завершено

Этот файл не заменяет код и тесты. Он нужен как навигационная и planning-спека.

## Как читать карту

Статусы в этом документе:

- `Доработано`: есть рабочая реализация, кодовая структура устойчива, есть признаки тестового покрытия
- `Частично`: базовая реализация есть, но видны явные ограничения, заглушки или неполное покрытие возможностей
- `Не завершено`: интерфейс или каркас есть, но реализация отсутствует либо feature явно недоведена

Оценка построена по:

- структуре `src/` и `tests/`
- наличию рабочих entrypoint-ов
- наличию заглушек, `NotImplementedException`, `NotSupportedException`, пустых методов
- наличию unit / integration tests

## Общая структура

Основные каталоги кода:

- [src/NKafka](K:\nkafka\src\NKafka) — основная библиотека
- [src/NKafka.MessageGenerator](K:\nkafka\src\NKafka.MessageGenerator) — генератор protocol/message classes
- [tests/NKafka.Tests](K:\nkafka\tests\NKafka.Tests) — unit tests
- [tests/integration](K:\nkafka\tests\integration) — integration tests
- [tests/NKafka.MessageGenerator.Tests](K:\nkafka\tests\NKafka.MessageGenerator.Tests) — tests генератора

Крупные подсистемы `src/NKafka`:

- `Clients`: 79 `.cs` файлов
- `Messages`: 107 `.cs` файлов
- `Protocol`: 35 `.cs` файлов
- `Connection`: 32 `.cs` файла
- `Serialization`: 26 `.cs` файлов
- `Config`: 19 `.cs` файлов

## Архитектурный срез

### 1. Cluster layer

Главная точка входа:

- [KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs)
- [IKafkaCluster.cs](K:\nkafka\src\NKafka\IKafkaCluster.cs)

Отвечает за:

- создание cluster context
- lifecycle библиотеки
- metadata refresh
- создание producer / consumer / admin clients
- хранение broker/topic/partition metadata
- координацию connector pool

Текущее состояние:

- `Частично`

Что доработано:

- cluster действительно является центральной точкой входа
- есть metadata refresh и кэширование topology state
- есть фабрики для producer / consumer / admin
- есть фоновые задачи обновления metadata

Что не завершено или ограничено:

- `GetOffset()` пока возвращает `Offset.Unset`
- заметна зависимость части функциональности от текущего состояния metadata
- совместимость и fallback-paths ещё не выглядят полностью закрытыми на уровне cluster abstraction

### 2. Connection layer

Основные файлы:

- [Connection/KafkaConnector.cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.cs)
- [Connection/KafkaConnector.ProcessResponses.cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.ProcessResponses.cs)
- [Connection/KafkaConnector.Auth..cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.Auth..cs)
- [Connection/KafkaConnectorPool.cs](K:\nkafka\src\NKafka\Connection\KafkaConnectorPool.cs)

Отвечает за:

- открытие и восстановление TCP/SSL/SASL соединений
- request/response exchange
- inflight request tracking
- connector pooling
- dedicated/shared connections
- auth handshake и broker capability discovery

Текущее состояние:

- `Частично`

Что доработано:

- есть рабочий connector abstraction
- есть pooling
- есть request serialization и response processing
- есть `ApiVersions` negotiation
- есть SSL/SASL wiring на уровне конфигурации и connector

Что не завершено или ограничено:

- `NullConnector` — заглушка для технических сценариев, не рабочая реализация
- в [KafkaConnector.Auth..cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.Auth..cs) production auth path сейчас реально покрывает `PLAIN`, `SCRAM-SHA-256` и `SCRAM-SHA-512`
- `OAUTHBEARER` и `Kerberos/GSSAPI` остаются явно unsupported runtime paths, несмотря на более широкую конфигурационную поверхность SASL в экосистеме Kafka
- есть TODO о переносе части response processing из connector в pool

### 3. Protocol layer

Основные файлы:

- [Protocol/RequestBuilder.cs](K:\nkafka\src\NKafka\Protocol\RequestBuilder.cs)
- [Protocol/ResponseBuilder.cs](K:\nkafka\src\NKafka\Protocol\ResponseBuilder.cs)
- [Protocol/ApiKeys.cs](K:\nkafka\src\NKafka\Protocol\ApiKeys.cs)
- [Protocol/ApiVersion.cs](K:\nkafka\src\NKafka\Protocol\ApiVersion.cs)
- [Protocol/Buffers](K:\nkafka\src\NKafka\Protocol\Buffers)
- [Protocol/Records](K:\nkafka\src\NKafka\Protocol\Records)

Отвечает за:

- Kafka wire protocol primitives
- request/response materialization
- version-aware serialization
- flexible versions / tagged fields support
- record batch encoding/decoding
- buffer abstractions

Текущее состояние:

- `Доработано`

Что доработано:

- есть развитый protocol core
- есть generated builders и support versions
- есть records/batches/buffers
- явно поддерживается version-aware обработка сообщений
- есть большое количество message tests в `tests/NKafka.Tests/Messages`
- в текущем protocol-awareness stage добавлены `DescribeCluster` и `ConsumerGroupDescribe` contracts
- ручная fallback compatibility matrix расширена для stage API from `InitProducerId` through `ConsumerGroupDescribe`

Что ограничено:

- часть edge-cases зависит от generated contracts и актуальности JSON specs
- реальная полнота покрытия определяется тем, какие messages подключены к клиентским сценариям

### 4. Messages layer

Основной каталог:

- [Messages](K:\nkafka\src\NKafka\Messages)

Отвечает за:

- generated request/response classes Kafka protocol
- ручные partial extensions для отдельных сообщений
- support новых версий API через specs и generator

Текущее состояние:

- `Доработано`

Что доработано:

- каталог большой и уже покрывает значимое число Kafka APIs
- есть generated `.g.cs` и ручные partial overlays
- есть autogenerated tests для сообщений

Что ограничено:

- полнота определяется входными JSON specs и тем, насколько новые KIP-сообщения реально включены в клиентские workflows
- generated code нельзя рассматривать как бизнес-реализацию сам по себе, это инфраструктурный слой
- часть новых contracts уже `Protocol-known`/`Fallback-known`, но ещё не `Runtime-integrated`

### 5. Message Generator

Основные файлы:

- [src/NKafka.MessageGenerator/MessageGenerator.cs](K:\nkafka\src\NKafka.MessageGenerator\MessageGenerator.cs)
- [src/NKafka.MessageGenerator/ReadMethodGenerator.cs](K:\nkafka\src\NKafka.MessageGenerator\ReadMethodGenerator.cs)
- [src/NKafka.MessageGenerator/WriteMethodGenerator.cs](K:\nkafka\src\NKafka.MessageGenerator\WriteMethodGenerator.cs)
- [src/NKafka.MessageGenerator/Specifications](K:\nkafka\src\NKafka.MessageGenerator\Specifications)

Отвечает за:

- генерацию Kafka message classes из JSON specs
- генерацию read/write/equality/hash/toString
- генерацию request/response builders и support version tables

Текущее состояние:

- `Доработано`

Что доработано:

- generator самостоятелен и структурирован
- есть отдельный тестовый проект [tests/NKafka.MessageGenerator.Tests](K:\nkafka\tests\NKafka.MessageGenerator.Tests)
- generator уже используется как базовый путь развития message layer

Что ограничено:

- качество output зависит от качества specs
- часть protocol evolution требует ручных partial extensions поверх generated classes

## Клиентские подсистемы

### 6. Producer

Основные entrypoint-ы:

- [Clients/Producer/Producer.cs](K:\nkafka\src\NKafka\Clients\Producer\Producer.cs)
- [Clients/Producer/Producer.Transaction.cs](K:\nkafka\src\NKafka\Clients\Producer\Producer.Transaction.cs)
- [Clients/Producer/Internals](K:\nkafka\src\NKafka\Clients\Producer\Internals)

Отвечает за:

- public producer lifecycle
- append/send pipeline
- partitioning
- batching
- delivery handling
- transactional/idempotent semantics

Текущее состояние:

- `Частично`

Что доработано:

- базовый producer pipeline есть
- есть partitioner abstraction и стандартные partitioners
- есть accumulator / batch / sender internals
- есть metrics и logging

Что не завершено или ограничено:

- transactional слой явно незавершён
- в [Producer.Transaction.cs](K:\nkafka\src\NKafka\Clients\Producer\Producer.Transaction.cs) `ThrowIfNotTransactional()` сейчас всегда бросает исключение
- `ThrowIfInvalidGroupMetadata()` пустой
- в [TransactionManager.cs](K:\nkafka\src\NKafka\Clients\Producer\Internals\TransactionManager.cs) несколько ключевых методов не реализованы до конца:
  - `SendOffsetsToTransactionAsync()`
  - `CommitAsync()`
  - `AbortAsync()`
  - `LookupCoordinatorAsync()`
  - `ResetSequenceNumbers()`
  - `BumpIdempotentProducerEpochAsync()`
  - `InitializeTransactionsAsync()` содержит незавершённый `switch`

Вывод:

- обычный producer path выглядит жизнеспособным
- transactional/idempotent branch пока нельзя считать завершённой

### 7. Consumer

Основные entrypoint-ы:

- [Clients/Consumer/Consumer.cs](K:\nkafka\src\NKafka\Clients\Consumer\Consumer.cs)
- [Clients/Consumer/Internal/Fetcher.cs](K:\nkafka\src\NKafka\Clients\Consumer\Internal\Fetcher.cs)
- [Clients/Consumer/Internal/Coordinator.cs](K:\nkafka\src\NKafka\Clients\Consumer\Internal\Coordinator.cs)

Отвечает за:

- subscriptions
- group join/sync/heartbeat
- fetch loop
- offset commit/fetch
- deserialization и consumer channel delivery

Текущее состояние:

- `Частично`

Что доработано:

- есть рабочая подписка через channel-based consumer model
- есть coordinator/fetcher separation
- есть assignors (`Range`, `RoundRobin`)
- есть offset manager
- есть unit tests для consumer и coordinator

Что не завершено или ограничено:

- в [Coordinator.cs](K:\nkafka\src\NKafka\Clients\Consumer\Internal\Coordinator.cs) `StopSessionAsync()` пока фактически пустой
- там же часть error branches в `SyncGroupAsync()` заканчиваются `throw new NotSupportedException()`
- новый consumer protocol/KIP-848 виден на уровне messages/specs, но классический coordinator path остаётся основным
- assignment property в `Consumer` выглядит незавершённой как публичное отражение текущего состояния

Вывод:

- базовый consumer/group path есть
- graceful rebalance/session shutdown и часть advanced flows ещё требуют доработки

### 8. Admin client

Основные файлы:

- [Clients/Admin/IAdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\IAdminClient.cs)
- [Clients/Admin/AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs)

Отвечает за:

- topic admin operations
- cluster describe/list operations
- ACL/config admin surface

Текущее состояние:

- `Частично`

Что доработано:

- `CreateTopicsAsync()`
- `DeleteTopicsAsync()`
- `ListTopicsAsync()`
- `DescribeTopicsAsync()`
- `DescribeClusterAsync()`

Что не завершено:

- `DescribeAclsAsync()` возвращает пустой result
- `CreateAclsAsync()` возвращает пустой result
- `DeleteAclsAsync()` возвращает пустой result
- `DescribeConfigsAsync()` возвращает пустой result
- `AlterConfigsAsync()` возвращает пустой result
- `IncrementalAlterConfigsAsync()` возвращает пустой result
- protocol/fallback awareness для этих API уже добавлена, но orchestration и mapping не реализованы

Вывод:

- topic/cluster admin реализован частично
- ACL и config administration сейчас скорее API-carкас, чем готовая реализация

## Поддерживающие подсистемы

### 9. Config

Каталог:

- [Config](K:\nkafka\src\NKafka\Config)

Отвечает за:

- cluster/producer/consumer/security configuration
- validation
- enums и options

Текущее состояние:

- `Доработано`

Что доработано:

- есть отдельные config types для cluster, producer, consumer, broker, security
- есть unit tests для config
- config layer используется как основной public surface для настройки клиента

### 10. Serialization

Каталог:

- [Serialization](K:\nkafka\src\NKafka\Serialization)

Отвечает за:

- built-in serializers/deserializers
- typed serializer registry

Текущее состояние:

- `Доработано`

Что доработано:

- есть набор базовых primitive/string/guid/bytes serializers
- есть tests

Что ограничено:

- `NoneSerializer` / `NoneDeserializer` — технические sentinels, не runtime serializer implementations

### 11. Compressions

Каталог:

- [Compressions](K:\nkafka\src\NKafka\Compressions)

Отвечает за:

- no compression
- gzip
- lz4
- snappy
- zstd

Текущее состояние:

- `Доработано`

### 12. Diagnostics and Metrics

Каталоги:

- [Diagnostics](K:\nkafka\src\NKafka\Diagnostics)
- [Metrics](K:\nkafka\src\NKafka\Metrics)

Отвечают за:

- logging scopes
- Activity/OpenTelemetry hooks
- producer/consumer metrics abstractions

Текущее состояние:

- `Частично`

Что доработано:

- diagnostics встроены в cluster/admin/producer/consumer flow
- есть default/null metrics implementations

Что ограничено:

- client metrics roadmap шире текущей реализации
- advanced observability features из новых KIP-ов пока не выглядят полностью закрытыми

### 13. Security and SASL

Основные файлы:

- [Config/SaslSettings.cs](K:\nkafka\src\NKafka\Config\SaslSettings.cs)
- [Connection/Sasl](K:\nkafka\src\NKafka\Connection\Sasl)
- [Connection/KafkaConnector.Auth..cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.Auth..cs)

Отвечает за:

- security protocol settings
- SASL mechanism wiring
- SCRAM primitives
- OAuth bearer auth data

Текущее состояние:

- `Частично`

Что доработано:

- есть config model для SSL/SASL
- есть unified auth-session orchestration для `PLAIN`, `SCRAM-SHA-256` и `SCRAM-SHA-512`
- есть SCRAM classes, message parsing primitives и runtime connector wiring
- есть PLAIN, OAUTHBEARER и SCRAM providers, но `SaslOAuthBearerProvider` пока остаётся placeholder without real auth data generation

Что не завершено или ограничено:

- `OAUTHBEARER` и `Kerberos/GSSAPI` остаются явно unsupported runtime paths
- часть SASL support всё ещё требует дальнейшего hardening через integration-style verification и observability assertions

## Тестовый срез

### Unit tests

Основной проект:

- [tests/NKafka.Tests](K:\nkafka\tests\NKafka.Tests)

Что покрыто особенно хорошо:

- message serialization/deserialization
- protocol-related generated tests
- config
- consumer/connection базовые сценарии

Что покрыто заметно слабее:

- admin advanced operations
- transactional producer path
- end-to-end security combinations

### Integration tests

Проект:

- [tests/integration](K:\nkafka\tests\integration)

Что видно по текущей структуре:

- есть integration tests для admin cluster/topic scenarios
- явного широкого integration coverage для producer/consumer/transactions в текущем дереве не видно

### Generator tests

Проект:

- [tests/NKafka.MessageGenerator.Tests](K:\nkafka\tests\NKafka.MessageGenerator.Tests)

Состояние:

- generator testируется отдельно и это сильная сторона проекта

## Карта зрелости

### Доработано

- protocol buffers/records/builders
- message generation pipeline
- generated messages как инфраструктурный слой
- config layer
- базовые serializers/deserializers
- compression implementations

### Частично

- cluster orchestration
- connector/pool/auth runtime
- producer base pipeline
- consumer base pipeline
- diagnostics/metrics
- admin client overall
- security integration

### Не завершено

- transactional producer workflow
- ACL/config branches admin client
- часть consumer coordinator error/rebalance/session-stop flows
- часть SASL runtime integration

## Что агенту читать в первую очередь по задачам

Если задача про cluster lifecycle и metadata:

- [KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs)

Если задача про producer:

- [Clients/Producer/Producer.cs](K:\nkafka\src\NKafka\Clients\Producer\Producer.cs)
- [Clients/Producer/Internals](K:\nkafka\src\NKafka\Clients\Producer\Internals)
- [Clients/Producer/Producer.Transaction.cs](K:\nkafka\src\NKafka\Clients\Producer\Producer.Transaction.cs)

Если задача про consumer:

- [Clients/Consumer/Consumer.cs](K:\nkafka\src\NKafka\Clients\Consumer\Consumer.cs)
- [Clients/Consumer/Internal/Coordinator.cs](K:\nkafka\src\NKafka\Clients\Consumer\Internal\Coordinator.cs)
- [Clients/Consumer/Internal/Fetcher.cs](K:\nkafka\src\NKafka\Clients\Consumer\Internal\Fetcher.cs)

Если задача про admin:

- [Clients/Admin/IAdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\IAdminClient.cs)
- [Clients/Admin/AdminClient.cs](K:\nkafka\src\NKafka\Clients\Admin\AdminClient.cs)

Если задача про protocol/message layer:

- [Protocol](K:\nkafka\src\NKafka\Protocol)
- [Messages](K:\nkafka\src\NKafka\Messages)
- [src/NKafka.MessageGenerator](K:\nkafka\src\NKafka.MessageGenerator)
- [Protocol interaction spec](K:\nkafka\spec\protocol-interaction.md)

Если задача про безопасность:

- [Connection/KafkaConnector.Auth..cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.Auth..cs)
- [Connection/Sasl](K:\nkafka\src\NKafka\Connection\Sasl)
- [Config/SaslSettings.cs](K:\nkafka\src\NKafka\Config\SaslSettings.cs)

## Следующий полезный шаг

Логичное продолжение этой карты:

- сделать `spec/code-gaps.md` с file-level backlog
- или добавить ownership/status table по каждому client-facing feature: producer, consumer, admin, transactions, SASL, KIP-848, KIP-951 и т.д.
