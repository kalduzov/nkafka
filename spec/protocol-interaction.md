# Спецификация взаимодействия с Kafka protocol

## Назначение документа

Этот документ фиксирует целевую модель взаимодействия `NKafka` с Kafka на уровне wire protocol.

Он нужен для того, чтобы:

- отделить transport/protocol layer от client API layer
- дать агентам единое описание стадий взаимодействия с брокером
- зафиксировать boundaries между `Connection`, `Protocol`, `Messages` и higher-level clients
- упростить реализацию и ревью protocol-related изменений

Этот документ должен использоваться вместе с:

- [Base spec](K:\nkafka\spec\index.md)
- [Technical requirements](K:\nkafka\spec\technical-requirements.md)
- [Network interaction spec](K:\nkafka\spec\network-interaction.md)
- [Code map](K:\nkafka\spec\code-map.md)
- [Code gaps](K:\nkafka\spec\code-gaps.md)

## Scope

В scope этого документа входят:

- TCP/SSL/SASL connection lifecycle
- request/response framing
- request header / response header handling
- API version negotiation
- message serialization/deserialization
- metadata-driven routing
- protocol error handling
- compatibility behavior across Kafka versions

Не входят в scope:

- public usability of producer / consumer / admin API
- business semantics конкретного user scenario
- broker-side internals
- Kafka Streams / Connect / controller-only behavior

## Sources of truth

Для protocol-related решений приоритет источников такой:

1. Официальная Kafka protocol documentation: [Apache Kafka 4.3 Protocol Guide](https://kafka.apache.org/43/design/protocol/)
2. JSON message specifications в `resources/message/`
3. Reference Kafka repository mirror в [kafka/docs/design/protocol.md](K:\nkafka\kafka\docs\design\protocol.md)
4. Generated and manual protocol code в `src/NKafka/Protocol` и `src/NKafka/Messages`
5. Tests

Если есть расхождение между реализацией и официальным protocol contract, исправляется реализация, а не спецификация протокола.

Основная внешняя статья, описывающая wire protocol и message format:

- [Apache Kafka 4.3 Protocol Guide](https://kafka.apache.org/43/design/protocol/)

## Layer model

### Layer 1. Transport

Основной код:

- [src/NKafka/Connection](K:\nkafka\src\NKafka\Connection)

Отвечает за:

- socket connection
- SSL wrapping
- SASL authentication
- byte streaming
- inflight request tracking
- reconnection and connector state

Transport layer не должен знать consumer/producer business logic.

### Layer 2. Protocol primitives

Основной код:

- [src/NKafka/Protocol](K:\nkafka\src\NKafka\Protocol)

Отвечает за:

- primitive types
- request/response builders
- buffer readers/writers
- ApiKeys / ApiVersion / ErrorCodes
- record batch encoding
- tagged fields and flexible version mechanics

Protocol layer не должен принимать продуктовые решения о retry, metadata refresh или rebalance.

### Layer 3. Message contracts

Основной код:

- [src/NKafka/Messages](K:\nkafka\src\NKafka\Messages)
- [src/NKafka.MessageGenerator](K:\nkafka\src\NKafka.MessageGenerator)

Отвечает за:

- versioned request/response message contracts
- generated read/write logic
- support version tables
- manual partial extensions, если они действительно нужны

Message layer описывает contract, но не orchestration сценария.

### Layer 4. Client orchestration

Основной код:

- [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs)
- [src/NKafka/Clients](K:\nkafka\src\NKafka\Clients)

Отвечает за:

- выбор broker/connector
- metadata refresh strategy
- consumer group coordination
- producer batching and routing
- admin workflows

Именно этот слой превращает protocol primitives в клиентское поведение.

## Connection lifecycle

### 1. Bootstrap

Клиент получает список bootstrap endpoints из cluster config.

Цель bootstrap phase:

- установить хотя бы одно рабочее соединение
- получить metadata
- получить supported API versions у брокера

Ожидаемое поведение:

- использовать bootstrap servers как seed list, а не как окончательную routing map
- после успешной metadata загрузки перейти к broker-specific routing

### 2. Open connection

Transport layer открывает TCP socket к broker endpoint.

Если включён SSL:

- сначала завершается SSL handshake

Если включён SASL:

- выполняется SASL sequence в соответствии с поддерживаемой схемой Kafka

### 3. ApiVersions negotiation

После установления connection клиент должен иметь способ определить supported API ranges для этого broker connection.

Базовые правила:

- `ApiVersionsRequest` используется как основной механизм negotiation
- полученные support versions валидны для конкретного connection
- после reconnect/connection reset они должны считаться устаревшими
- fallback matrix in [src/NKafka/Protocol/SupportVersionsExtensions.cs](K:\nkafka\src\NKafka\Protocol\SupportVersionsExtensions.cs) must be kept in sync with the Kafka version baselines supported by the library
- when a new Kafka baseline is added, its fallback API ranges are taken from the `validVersions` values of the corresponding Kafka branch request JSON specs

### 4. Steady-state request processing

После успешного connection setup транспорт должен:

- сериализовать request header и body
- отправлять request bytes
- сопоставлять response по `CorrelationId`
- десериализовать response using the correct API version

### 5. Reconnect / recover

При socket error, disconnect, stale metadata или protocol-routing error:

- connection state признаётся недействительным
- API support/version state для этого connection пересматривается
- routing/metadata может потребовать refresh
- pending operations обрабатываются согласно policy конкретного higher layer

## Request/response model

Kafka communication в `NKafka` должна следовать модели:

- all requests are client-initiated
- each request has exactly one response, если protocol явно не задаёт исключение
- request framing size-delimited
- request/response headers version-aware

Обязательные элементы request path:

- `message_size`
- versioned request header
- versioned body
- correct `ApiKey`
- correct `ApiVersion`
- `CorrelationId`
- `ClientId`

Обязательные элементы response path:

- response size
- versioned response header
- body deserialization на основании ожидаемого API/version pair

## API version policy

### Selection rule

Для каждого broker connection клиент должен использовать:

- максимальную версию API, поддерживаемую и клиентом, и брокером

Если пересечения версий нет:

- operation должна завершаться явной protocol/compatibility error

### Version scope

API version decision принимается:

- per broker
- per connection state
- per API key

Нельзя предполагать, что однажды полученная support table автоматически применима ко всем reconnect scenarios.

### Flexible versions and tagged fields

Если версия сообщения поддерживает flexible versions:

- serialization/deserialization должна учитывать compact encodings
- tagged fields должны читаться и писаться корректно

Если версия не поддерживает tagged fields:

- попытка сериализации incompatible fields должна приводить к explicit failure

## Metadata-driven routing

Metadata нужна для:

- broker discovery
- leader discovery
- topic/partition lookup
- controller/cluster info
- retry after stale topology

Базовые правила:

- metadata cache разрешена и ожидаема
- metadata refresh не должен быть постоянным polling без причины
- refresh запускается на bootstrap, при необходимости, и при topology/protocol hints

Триггеры refresh:

- unknown topic/partition routing
- not leader / stale leader hints
- broken broker connectivity
- explicit admin/client request

## Error model

Ошибки нужно разделять по слоям:

### Transport errors

Примеры:

- socket failure
- SSL failure
- connection closed
- timeout at transport/request level

Обычно ведут к:

- reconnect
- request failure
- metadata invalidation при необходимости

### Protocol errors

Примеры:

- unsupported version
- malformed request assumptions
- response parsing mismatch
- Kafka protocol error code from broker

Обычно ведут к:

- explicit exception
- retry only when the specific API semantics allow it

### Semantic client errors

Примеры:

- invalid consumer state
- transaction state violation
- wrong topic/group metadata in higher-level layer

Они должны обрабатываться выше transport/protocol layers.

## Security sequence

### SSL

Если security protocol требует SSL:

- Kafka protocol requests не начинаются до завершения SSL handshake

### SASL

Ожидаемая последовательность:

1. Optional `ApiVersionsRequest`
2. `SaslHandshakeRequest`
3. Authentication token exchange
4. Transition to normal Kafka API requests

Поддержка SASL mechanism в config не должна автоматически означать production-ready runtime support.

Для каждого mechanism:

- должна быть явная runtime implementation
- должны быть tests
- должна быть documented compatibility status

## Message generation policy

Message contracts должны поддерживаться через generator-first model.

Базовые правила:

- `.g.cs` files не редактируются вручную
- change начинается со specification/generator layer
- manual partial extension допустим только когда generator contract сам по себе недостаточен

Reference baseline:

- generation model должна оставаться согласованной с reference Kafka/Java contract model
- этот baseline обновляется отдельно по мере изменения Kafka protocol specifications
- divergence от reference behavior должна быть осознанной и задокументированной

## Testing policy for protocol interaction

Минимальный набор тестов для protocol-related change:

- unit tests for buffer/serialization logic
- message serialization/deserialization tests
- generator-driven tests for generated contracts
- connection/auth tests, если меняется transport/security path
- integration tests, если change влияет на broker interoperability

Особое правило:

- для каждого изменённого protocol module должен быть unit-test
- для autogenerated contracts тесты тоже должны обновляться через generated/generator-driven path

## Design constraints

Реализация protocol interaction должна учитывать:

- AOT compatibility
- `.editorconfig` rules
- nullable correctness
- predictable allocation profile
- English comments/log messages by default
- resource-based localization pattern for reusable/user-visible text

## Current implementation notes

По текущему состоянию кодовой базы:

- protocol core и message generation выглядят зрелыми
- connection/auth/runtime orchestration стала заметно более формализованной
- `PLAIN`, `SCRAM-SHA-256` и `SCRAM-SHA-512` подключены к production auth path
- `OAUTHBEARER` пока остаётся partially wired mechanism, а не production-ready auth flow
- `Kerberos/GSSAPI` остаётся unsupported runtime behavior
- transactional producer protocol path не завершён
- часть admin и consumer protocol workflows пока неполная

См. также:

- [Code map](K:\nkafka\spec\code-map.md)
- [Code gaps](K:\nkafka\spec\code-gaps.md)

## What to update when protocol behavior changes

Если меняется protocol interaction behavior, нужно проверить:

1. `resources/message/*.json`
2. generator output / related tests
3. `src/NKafka/Protocol`
4. `src/NKafka/Connection`
5. affected client orchestration code
6. `spec/technical-requirements.md`
7. этот документ
