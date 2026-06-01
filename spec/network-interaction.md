# Спецификация сетевого взаимодействия NKafka

## Назначение документа

Этот документ фиксирует целевую архитектуру и правила развития сетевого слоя `NKafka`.

Он нужен, чтобы:

- отделить transport/session concerns от cluster orchestration
- зафиксировать boundaries между `KafkaCluster`, `KafkaConnectorPool` и `KafkaConnector`
- определить целевую модель shared/dedicated connections
- описать lifecycle подключения, reconnect и response processing
- превратить текущие code-level наблюдения в планируемую архитектуру изменений

Документ следует читать вместе с:

- [Base spec](K:\nkafka\spec\index.md)
- [Technical requirements](K:\nkafka\spec\technical-requirements.md)
- [Network layer decoupling plan](K:\nkafka\spec\network-layer-decoupling-plan.md)
- [Protocol interaction spec](K:\nkafka\spec\protocol-interaction.md)
- [Code map](K:\nkafka\spec\code-map.md)
- [Code gaps](K:\nkafka\spec\code-gaps.md)

## Scope

В scope этого документа входят:

- TCP/SSL/SASL connection establishment
- connector pool topology and routing
- shared vs dedicated connection policy
- request send path and inflight tracking
- response read loop and correlation by `CorrelationId`
- reconnect behavior and invalidation rules
- metadata interaction with the connection layer
- runtime boundaries between `Cluster`, `Pool` and `Connector`

Не входят в scope:

- public producer / consumer / admin API design
- detailed semantics of individual Kafka APIs
- generated message contracts as such
- broker-side networking internals

## Why a separate spec is needed

По текущему состоянию кодовой базы сетевой слой уже функционален, но остаётся архитектурно смешанным:

- `KafkaConnector` одновременно отвечает за socket lifecycle, SSL/SASL setup, request write path, response reader loop и inflight completion
- `KafkaConnectorPool` совмещает bootstrap routing, broker registry, connector creation и часть lifecycle orchestration
- metadata refresh внутри `KafkaCluster` тесно связан с тем, как pool выбирает физическое соединение
- часть response processing уже помечена в коде как кандидат на вынос из connector

Для дальнейшей модификации сетевого взаимодействия нужен явный target architecture document, чтобы изменения были согласованными и не превращались в локальные точечные правки.

## Sources of truth

Приоритет источников для network-layer решений:

1. [AGENTS.md](K:\nkafka\AGENTS.md)
2. [spec/index.md](K:\nkafka\spec\index.md)
3. [spec/technical-requirements.md](K:\nkafka\spec\technical-requirements.md)
4. [spec/protocol-interaction.md](K:\nkafka\spec\protocol-interaction.md)
5. Код в `src/NKafka/Connection` и `src/NKafka/KafkaCluster.cs`
6. Tests
7. Kafka protocol documentation, когда network behavior определяется protocol requirements

Если целевая архитектура этого документа конфликтует с фактическим кодом, код рассматривается как текущее состояние, а этот документ как target state для модификации.

## Design goals

Сетевой слой `NKafka` должен:

- минимизировать число физических соединений без потери корректности
- сохранять `cluster-first` model, где cluster владеет routing и metadata decisions
- держать transport logic независимой от producer / consumer business logic
- обеспечивать version-aware и connection-aware protocol negotiation
- быть предсказуемым по аллокациям и количеству фоновых задач
- позволять постепенно расширять security support без переписывания базового send/receive path
- поддерживать AOT-friendly и testable design

## Non-goals

Этот документ не предлагает:

- переписать message serialization/deserialization
- немедленно перевести весь networking на `System.IO.Pipelines`
- расширять public API ради внутреннего рефакторинга
- дублировать всю protocol semantics из `protocol-interaction.md`

## Current-state summary

На текущий момент:

- `KafkaCluster` выбирает соединение для service requests и metadata refresh
- `KafkaConnectorPool` хранит seed endpoints, broker registry и набор connectors по `Node`
- `KafkaConnector` хранит одно физическое соединение и таблицу `SupportVersions`
- `KafkaConnector` сам открывает socket, поднимает `NetworkStream` или `SslStream`, выполняет `ApiVersions` negotiation и SASL authentication
- `KafkaConnector` сам же ведёт write path, inflight dictionary и response reader loop
- dedicated connector создаётся по требованию, shared connector переиспользуется через pool

Основные проблемы текущего состояния:

- слишком много responsibility внутри одного connector class
- connection state и response processing связаны сильнее, чем нужно
- reconnect behavior не оформлен как явная state machine
- invalidation policy для `SupportVersions`, inflight requests и metadata hints описана не полностью
- security runtime path всё ещё требует дальнейшего hardening, но production auth path уже покрывает `PLAIN`, `OAUTHBEARER`, `SCRAM-SHA-256` и `SCRAM-SHA-512`

## Target architecture

### Layer split

Целевая модель сетевого слоя:

1. `KafkaCluster`
2. `KafkaConnectorPool`
3. `KafkaConnector`
4. transport/session internals

### `KafkaCluster` responsibilities

`KafkaCluster` должен:

- владеть bootstrap configuration и cluster metadata
- решать, когда требуется metadata refresh
- выбирать broker target по cluster semantics
- запрашивать shared или dedicated connector у pool
- агрегировать broker capabilities на cluster level

`KafkaCluster` не должен:

- управлять socket-level lifecycle напрямую
- знать детали response reader loop
- принимать transport-level решения о том, как читать поток

### `KafkaConnectorPool` responsibilities

`KafkaConnectorPool` должен:

- хранить seed endpoints до появления broker metadata
- поддерживать mapping `nodeId -> shared connectors`
- создавать dedicated connectors по запросу
- знать policy выбора shared connector для конкретного broker
- уметь обновлять registry после metadata changes
- убирать permanently dead connectors из active registry

`KafkaConnectorPool` не должен:

- разбирать protocol messages
- владеть topic metadata
- принимать решения уровня producer / consumer semantics

### `KafkaConnector` responsibilities

`KafkaConnector` должен представлять одно физическое broker connection session и отвечать за:

- TCP connect
- optional SSL handshake
- optional SASL authentication
- `ApiVersions` negotiation для данного connection
- send path для versioned Kafka requests
- correlation `CorrelationId -> pending response`
- response read and completion for requests, назначенных этому connector
- local connection metrics and health state

`KafkaConnector` не должен:

- владеть cluster-wide routing logic
- решать, когда обновлять metadata
- содержать business retry policy выше transport/protocol уровня

### Transport/session internals

Внутренности transport/session должны со временем быть выделены логически, даже если физически остаются в тех же файлах на первом этапе.

Минимально должны быть различимы следующие concerns:

- connect/auth session establishment
- request serialization and write
- response framing and correlation
- connection state transitions

Это не обязательно означает мгновенное появление новых public types, но архитектурно эти зоны должны развиваться раздельно.

## Connection model

### Shared connections

Shared connection используется для:

- metadata requests
- producer/admin service requests, которым не нужен session affinity
- обычных broker-targeted requests, если Kafka semantics не требуют выделенного канала

Правила:

- pool должен переиспользовать shared connector, если он жив и пригоден к записи
- при наличии нескольких shared connectors к одному broker должен выбираться least-loaded connector
- pool не должен создавать новые shared connectors бесконтрольно; рост числа shared connections должен быть policy-driven, а не случайным побочным эффектом retry path

### Dedicated connections

Dedicated connection используется, когда higher-level runtime требует connection affinity или изоляцию от shared traffic.

Базово к таким сценариям относятся:

- consumer coordinator-bound workflows, если для них уже используется выделенное соединение
- long-lived session paths, где нежелательно делить inflight queue с service traffic
- future transactional/coordinator-sensitive flows, если это будет подтверждено runtime requirements

Правила:

- dedicated connector создаётся только явным запросом от higher-level layer
- dedicated connector не участвует в общем balancing shared requests
- lifecycle dedicated connector привязан к owning client/session

## Connection lifecycle

### State model

Для physical connection вводится следующая логическая state model:

1. `Closed`
2. `Connecting`
3. `Negotiating`
4. `Authenticating`
5. `Open`
6. `Faulted`
7. `Closing`

Текущее enum-состояние в коде может временно оставаться проще, но дальнейшие изменения должны ориентироваться на эту модель.

### Open sequence

Нормальная последовательность открытия connection:

1. Open TCP socket to endpoint
2. Wrap stream with SSL if required
3. Start connection-scoped session
4. Perform `ApiVersions` negotiation when enabled
5. Perform SASL handshake/authentication when configured
6. Publish `SupportVersions` for this connector
7. Mark connector as `Open`

### ApiVersions rules

`SupportVersions` должны трактоваться как connection-scoped state.

Правила:

- таблица применима только к конкретному physical connection
- после reconnect или connection reset таблица должна считаться устаревшей
- cluster-level aggregated API versions должны пересчитываться из currently opened connectors
- fallback matrix остаётся backup path, а не заменой успешного runtime negotiation

### Reconnect rules

Reconnect должен запускаться только когда:

- connector is closed or faulted
- stream became unreadable or unwritable
- connection establishment previously failed and caller retries through higher layer

Reconnect не должен:

- silently preserve stale inflight requests
- сохранять старую `SupportVersions` table как будто она относится к новой session
- скрывать protocol parsing errors как transport reconnect cases

## Request path

### Send responsibilities

При отправке запроса connector должен:

- определить effective API version через `SupportVersions`
- построить version-correct request header
- назначить уникальный `CorrelationId` в рамках connector session
- зарегистрировать pending response before or atomically with write start
- записать framed request в stream

### Inflight rules

Для inflight requests нужны следующие инварианты:

- один `CorrelationId` соответствует ровно одному pending request
- pending request must know expected `ApiKey` and `ApiVersion`
- request timeout должен завершать pending task deterministically
- failed write must remove pending request from inflight registry
- connection shutdown must complete or fail all pending requests predictably

### Internal vs external requests

В коде уже есть distinction `isInternalRequest`.

Целевая семантика:

- internal request допускается использовать для bootstrap/session setup paths
- internal request bypasses only higher-level user-facing validation
- internal request не должен обходить transport safety invariants

## Response path

### Response reader model

Для одного physical connection должен существовать ровно один активный read loop, отвечающий за последовательное чтение Kafka frames из stream.

Read loop должен:

- читать `message_size`
- дочитывать response body полностью
- извлекать `CorrelationId`
- находить pending request
- передавать body в response builder с ожидаемыми `ApiKey` и `ApiVersion`
- завершать pending task result или error

### Concurrency rules

Допускается параллельная parse/completion phase, но только если сохраняются следующие гарантии:

- чтение из stream остаётся single-reader
- buffer ownership однозначен
- response body не возвращается в pool до завершения parse
- out-of-order completion допустим только если correlation correctness не нарушается

### Target direction for response processing

Текущий TODO в коде про возможный вынос response processing из `KafkaConnector` в pool отражает допустимое направление, но его нужно уточнить.

Целевое правило:

- ownership of physical stream stays with connector
- если parse/completion orchestration выносится из connector, это не должно размывать границу: pool координирует connectors, а не становится владельцем socket session

Иными словами, можно выносить часть coordination/mechanics, но не делать pool новым transport object.

## Metadata interaction

Сетевой слой должен быть совместим с metadata-driven cluster model.

Правила:

- bootstrap endpoints используются только как seed list
- после успешного metadata refresh routing должен опираться на broker ids and broker endpoints from cluster metadata
- metadata update может добавлять, переиспользовать или удалять connectors в pool
- metadata refresh itself не должен зависеть от существования controller, если protocol этого не требует
- transport failures могут быть сигналом к metadata invalidation, но не заменяют metadata policy целиком

## Error model

### Transport failures

Примеры:

- socket connect error
- stream read/write failure
- SSL handshake failure
- remote disconnect

Поведение:

- connector переходит в `Faulted` или `Closed`
- pending inflight requests завершаются transport-level exception
- caller/higher layer решает retry и metadata refresh policy

### Session setup failures

Примеры:

- `ApiVersions` negotiation failure
- SASL handshake or authentication failure

Поведение:

- connector не считается `Open`
- `SupportVersions` не публикуется как валидная для steady state
- ошибка поднимается вызывающему коду явно

### Protocol response failures

Примеры:

- correlation mismatch
- malformed response frame
- response parse failure

Поведение:

- ошибка трактуется как connection-scoped failure, если невозможно доверять дальнейшему чтению stream
- connector должен быть сброшен, если stream alignment потерян

## Security model

### SSL

Требования:

- SSL handshake должен завершиться до любых обычных Kafka API requests
- validation policy должна быть управляемой конфигурацией, без скрытых insecure defaults в production-oriented path

### SASL

Требования:

- runtime support должна быть explicit per mechanism
- mechanism, указанный в config, не должен считаться supported, если для него нет end-to-end runtime path
- SCRAM integration должна быть оформлена как отдельный, но совместимый с общим connector lifecycle authentication path

Минимальная roadmap expectation:

- сохранить рабочий путь для `PLAIN`
- сохранить рабочий путь для `OAUTHBEARER`
- встроить SCRAM без дублирования send/receive logic и без special-case обходов протокольного слоя

Текущее состояние:

- `PLAIN`, `OAUTHBEARER`, `SCRAM-SHA-256` и `SCRAM-SHA-512` идут через общий connector auth pipeline
- `Kerberos/GSSAPI` остаётся явно unsupported runtime path

## Observability requirements

Сетевой слой должен публиковать достаточно сигналов для диагностики:

- connection open/close/fault events
- request send and response receive traces
- timeout and reconnect counters
- explicit state transition logs for physical connector lifecycle
- SASL mechanism selection logs for authenticated session setup
- auth and negotiation failures
- inflight request count and selected connector metrics, где это уже поддерживается системой метрик

Observability не должна требовать knowledge of producer/consumer internals внутри connector.

## Performance constraints

Любая модификация сетевого слоя должна оцениваться по следующим ограничениям:

- не увеличивать число физических соединений без явной причины
- не вводить лишние копирования response body
- не создавать unbounded background tasks per request
- не разрушать reuse буферов и pooled allocations
- не делать send path зависимым от heavyweight locks в hot path

`System.IO.Pipelines` можно рассматривать как future optimization path, но только если это улучшает measurable behavior и не усложняет API/version handling.

## Migration strategy

Изменения сетевого слоя рекомендуется делать волнами.

### Wave 1. Spec-aligned cleanup

- зафиксировать target responsibilities слоёв
- описать явные connector state transitions
- определить deterministic behavior для inflight cancellation и connection reset
- устранить misleading assumptions о поддерживаемых SASL mechanisms

### Wave 2. Connector internals refactoring

- отделить session establishment logic от steady-state send/receive path
- стабилизировать single-reader response loop
- формализовать invalidation `SupportVersions` after reconnect
- подготовить безопасную точку интеграции SCRAM

### Wave 3. Pool and orchestration cleanup

- уточнить lifecycle seed vs broker connectors
- отделить topology updates от connector creation policy
- пересмотреть место dedicated connections в consumer/transaction flows

### Wave 4. Extended hardening

- расширить integration coverage для reconnect and security scenarios
- оценить вынесение response coordination в отдельный internal component, если это действительно уменьшает сложность
- при необходимости провести performance verification in `benchmarks/`

## Testing requirements

Для изменений в сетевом взаимодействии обязательны:

- unit tests для connector state transitions
- unit tests для inflight registration/removal and timeout behavior
- unit tests для reconnect invalidation `SupportVersions`
- unit tests для pool selection policy: seed/shared/dedicated
- integration tests для bootstrap + metadata + request flow
- integration tests для SSL/SASL combinations, если меняется auth path
- regression tests для response correlation и connection reset on broken stream

Если конкретный шаг пока не покрывается integration tests, это должно быть явно отмечено в change description или в следующем spec update.

## Completion criteria for the networking rework

Сетевой слой можно считать приведённым к этой спецификации, когда:

- boundaries между `KafkaCluster`, `KafkaConnectorPool` и `KafkaConnector` явно соблюдаются
- connection lifecycle описан в коде так же чётко, как в документе
- reconnect path deterministic for inflight requests and `SupportVersions`
- security support matrix соответствует реальной runtime integration
- metadata-driven routing не требует knowledge of connector internals beyond declared interfaces
- tests покрывают основные transport/session regressions

## Immediate next implementation targets

Наиболее логичные первые задачи после этой спецификации:

1. Зафиксировать connection state machine в коде и тестах
2. Явно описать и реализовать invalidation behavior для inflight requests при reset/reconnect
3. Вынести или логически отделить session setup (`connect -> SSL -> ApiVersions -> SASL`) от steady-state request processing
4. Уточнить supported SASL matrix и подготовить SCRAM integration path
