# План развязывания сетевого слоя NKafka

## Назначение документа

Этот документ фиксирует практический план развязывания сетевого слоя `NKafka` поверх целевой архитектуры, описанной в [spec/network-interaction.md](K:\nkafka\spec\network-interaction.md).

Если [spec/network-interaction.md](K:\nkafka\spec\network-interaction.md) отвечает на вопрос "каким должен быть сетевой слой", то этот файл отвечает на вопрос "в каком порядке и с какими промежуточными результатами к нему прийти".

Документ нужен, чтобы:

- превратить target architecture в поэтапный refactoring backlog
- отделить верхнеуровневые архитектурные решения от file-level правок
- зафиксировать критерии завершения по этапам
- заранее собрать открытые вопросы, которые нужно уточнить до активного рефакторинга

Документ следует читать вместе с:

- [AGENTS.md](K:\nkafka\AGENTS.md)
- [Base spec](K:\nkafka\spec\index.md)
- [Technical requirements](K:\nkafka\spec\technical-requirements.md)
- [Network interaction spec](K:\nkafka\spec\network-interaction.md)
- [Protocol interaction spec](K:\nkafka\spec\protocol-interaction.md)
- [Code map](K:\nkafka\spec\code-map.md)
- [Code gaps](K:\nkafka\spec\code-gaps.md)

## Scope

В scope этого плана входят:

- развязка responsibilities между `KafkaCluster`, `KafkaConnectorPool` и `KafkaConnector`
- уточнение внутренних runtime-контрактов connection layer
- формализация connection lifecycle и reconnect semantics
- отделение session setup от steady-state request/response path
- cleanup topology/orchestration вокруг shared и dedicated connectors
- подготовка безопасной точки для SCRAM integration и дальнейшего security hardening

Не входят в scope:

- redesign public producer / consumer / admin API
- переписывание protocol/message generation
- немедленный переход на `System.IO.Pipelines`
- крупные performance-эксперименты без предварительной стабилизации архитектуры

## Sources of truth

Приоритет источников для решений в рамках этого плана:

1. [AGENTS.md](K:\nkafka\AGENTS.md)
2. [spec/index.md](K:\nkafka\spec\index.md)
3. [spec/technical-requirements.md](K:\nkafka\spec\technical-requirements.md)
4. [spec/network-interaction.md](K:\nkafka\spec\network-interaction.md)
5. [spec/protocol-interaction.md](K:\nkafka\spec\protocol-interaction.md)
6. Код в `src/NKafka/Connection` и [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs)
7. Tests

Если код и этот план расходятся, код считается текущим состоянием, а этот документ - планом перехода к target state. Если сам план расходится с [spec/network-interaction.md](K:\nkafka\spec\network-interaction.md), приоритет у network spec.

## Problem statement

По текущему состоянию кодовой базы сетевой слой функционален, но остаётся связанным сильнее, чем нужно:

- `KafkaCluster` знает о сетевом поведении больше, чем должен знать cluster orchestration layer
- `KafkaConnectorPool` совмещает registry, routing policy, connector creation и часть lifecycle orchestration
- `KafkaConnector` объединяет transport session, auth setup, write path, read loop, inflight registry и response completion
- reconnect, inflight invalidation и `SupportVersions` invalidation описаны неполно и не везде выражены как явные правила
- security surface шире, чем реально подтверждённый runtime path

Главный риск такого состояния - дальнейшие изменения в metadata, consumer coordination, security и retry semantics будут становиться всё дороже и опаснее, потому что каждое улучшение будет затрагивать сразу несколько уровней.

## Target outcomes

После завершения плана сетевой слой должен удовлетворять следующим условиям:

- responsibilities `Cluster / Pool / Connector / Session internals` выражены явно и подтверждаются кодовой структурой
- connection lifecycle оформлен как явная state machine с понятными переходами и side effects
- `SupportVersions` трактуется строго как connection-scoped state и детерминированно инвалидируется
- inflight request handling на write/reset/timeout path ведёт себя предсказуемо
- session setup (`connect -> SSL -> ApiVersions -> SASL`) отделён от steady-state request processing
- pool работает как topology and connector policy layer, но не превращается во владельца transport stream
- реальная security support matrix совпадает с тем, что разрешает runtime
- основные transport/session regressions прикрыты unit и integration tests

## Design principles for the refactoring

Весь план должен реализовываться с соблюдением следующих принципов:

- cluster-first: routing и metadata decisions остаются на уровне `KafkaCluster`
- minimal surface change: без лишнего расширения public API
- generator-safe: изменения не должны тянуть ручное редактирование generated artifacts
- performance-aware: не вводить лишние фоновые задачи, копирования и соединения
- AOT-friendly: новые abstractions должны быть простыми и статически анализируемыми
- migration-friendly: каждый этап должен оставлять код в рабочем состоянии

## Implementation strategy

Рефакторинг рекомендуется выполнять волнами, каждая из которых заканчивается рабочим и тестируемым промежуточным состоянием.

### Wave 0. Baseline and architecture alignment

Цель:

- зафиксировать, как текущий код соотносится с target architecture
- определить минимальный набор внутренних контрактов перед техническим рефакторингом

Задачи:

- собрать матрицу ответственностей `KafkaCluster / KafkaConnectorPool / KafkaConnector / internal session parts`
- перечислить текущие boundary violations по ключевым файлам
- выделить candidate internal contracts без обязательства сразу вводить все новые типы
- определить, какие изменения можно делать без изменения public API

Артефакты:

- этот документ
- уточнения к [spec/network-interaction.md](K:\nkafka\spec\network-interaction.md), если появятся расхождения
- рабочий backlog по этапам и файлам

Критерий завершения:

- у команды есть согласованная верхнеуровневая карта изменений
- перед началом правок понятны ownership boundaries и ожидаемые deliverables следующих волн

### Wave 1. Boundary extraction at the top level

Цель:

- развязать слой на уровне ответственности, не начиная с агрессивного дробления `KafkaConnector`

Задачи:

- зафиксировать, какие решения принадлежат `KafkaCluster`, а какие должны уйти в pool
- зафиксировать, какую минимальную информацию cluster может получать о connector без знания stream internals
- описать dedicated/shared ownership model как явный контракт
- определить, какие metadata signals транспорт может поднимать наверх, а какие решения всё равно остаются за cluster

Предполагаемые точки кода:

- [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs)
- [src/NKafka/Connection/IKafkaConnectorPool.cs](K:\nkafka\src\NKafka\Connection\IKafkaConnectorPool.cs)
- [src/NKafka/Connection/KafkaConnectorPool.cs](K:\nkafka\src\NKafka\Connection\KafkaConnectorPool.cs)
- [src/NKafka/Connection/IKafkaConnector.cs](K:\nkafka\src\NKafka\Connection\IKafkaConnector.cs)

Критерий завершения:

- cluster/pool boundary описана и выражена в коде хотя бы минимально
- уменьшено количество knowledge leaks о connector internals выше pool level
- открытые decisions для state machine и session setup зафиксированы отдельно

### Wave 2. Connection state machine

Цель:

- сделать lifecycle physical connection явным и тестируемым

Задачи:

- ввести целевую state model `Closed -> Connecting -> Negotiating -> Authenticating -> Open -> Faulted -> Closing`
- определить разрешённые переходы и side effects
- централизовать invalidation behavior для inflight requests, `_stream`, `SupportVersions` и background read loop
- отделить transport-fatal, protocol-fatal и recoverable setup failures

Предполагаемые точки кода:

- [src/NKafka/Connection/KafkaConnector.cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.cs)
- [src/NKafka/Connection/KafkaConnector.ProcessResponses.cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.ProcessResponses.cs)
- unit tests around connector state transitions

Критерий завершения:

- reset/reconnect path deterministic
- unit tests покрывают state transitions и inflight cleanup semantics

#### Wave 2 state-machine contract

Ниже зафиксирован обязательный lifecycle contract для physical connector session.

##### Logical states

Для одного `KafkaConnector` вводится следующая целевая state model:

1. `Closed`
2. `Connecting`
3. `Negotiating`
4. `Authenticating`
5. `Open`
6. `Faulted`
7. `Closing`

##### State meanings

`Closed`:

- physical session отсутствует
- connector не владеет usable stream for send/receive
- `SupportVersions` не считаются валидными

`Connecting`:

- начинается physical open sequence
- создаётся socket/session transport foundation
- steady-state requests через connector ещё недопустимы

`Negotiating`:

- transport уже поднят достаточно, чтобы выполнять connection-scoped protocol setup
- выполняется `ApiVersions` negotiation и связанные setup actions до steady state

`Authenticating`:

- transport и protocol negotiation уже дошли до authentication phase
- выполняется SASL sequence, если она требуется конфигурацией

`Open`:

- connector завершил session establishment
- stream считается пригодным для steady-state writes and reads
- `SupportVersions` опубликованы и валидны для этой physical session

`Faulted`:

- session признана недоверенной или unusable
- дальнейшее использование текущего stream/negotiation state недопустимо
- connector требует controlled reset/reopen path

`Closing`:

- выполняется controlled shutdown or reset
- inflight cleanup, stream disposal и state invalidation уже начаты

##### Allowed transitions

Разрешённые переходы:

- `Closed -> Connecting`
- `Connecting -> Negotiating`
- `Negotiating -> Authenticating`
- `Negotiating -> Open`
- `Authenticating -> Open`
- `Connecting -> Faulted`
- `Negotiating -> Faulted`
- `Authenticating -> Faulted`
- `Open -> Faulted`
- `Open -> Closing`
- `Faulted -> Closing`
- `Closing -> Closed`
- `Faulted -> Connecting`
- `Closed -> Closing` допускается только в dispose-oriented paths, если implementation это упрощает

Нежелательные переходы, которые не должны происходить напрямую:

- `Open -> Connecting` без промежуточного reset/closing semantics
- `Faulted -> Open` без нового session establishment
- `Closed -> Open` без прохождения setup phases
- `Connecting -> Open` в обход negotiation/auth steps, кроме случаев, где protocol setup явно не требуется и это задокументировано как equivalent path

##### Required side effects by transition

`Closed -> Connecting`:

- создаётся новая попытка session establishment
- старая capability state не должна переживать этот переход
- connector должен рассматриваться как entering a new physical session scope

`Connecting -> Negotiating`:

- transport готов для protocol-level setup
- write/read loops steady state ещё не считаются fully active

`Negotiating -> Authenticating`:

- `ApiVersions` negotiation завершена настолько, насколько это требуется для auth path
- connector ещё не считается `Open`

`Negotiating -> Open`:

- допустим только если auth не требуется
- `SupportVersions` должны быть опубликованы до входа в `Open`

`Authenticating -> Open`:

- auth path завершён успешно
- connector разрешает steady-state send/receive
- `SupportVersions` уже валидны для текущей session

`Any setup state -> Faulted`:

- setup sequence aborted
- connector не должен публиковаться как `Open`
- `SupportVersions` должны считаться невалидными

`Open -> Faulted`:

- текущему stream/session больше нельзя доверять
- дальнейшее steady-state использование недопустимо

`Open/Faulted -> Closing`:

- начинается единый cleanup path
- новые steady-state writes больше не должны приниматься
- inflight requests должны завершиться детерминированно

`Closing -> Closed`:

- stream/socket/session resources освобождены или переведены в non-usable state
- inflight registry очищен
- `SupportVersions` невалидны

##### Inflight invariants tied to the state machine

State machine должна обеспечивать следующие invariants:

- в `Open` connector может иметь inflight requests
- в `Connecting`, `Negotiating` и `Authenticating` inflight requests допускаются только для setup-internal operations, если implementation использует тот же send path
- при переходе в `Closing` все inflight requests должны быть завершены predictably
- после перехода в `Closed` inflight registry должен быть пуст
- после перехода в `Faulted` нельзя silently сохранить старые pending requests как будто session ещё валидна

##### `SupportVersions` lifecycle rules

`SupportVersions` трактуются как connection-scoped state.

Обязательные правила:

- `SupportVersions` публикуются только при успешном завершении setup sequence
- `SupportVersions` валидны только в `Open`
- любой переход в `Faulted` инвалидирует `SupportVersions`
- любой reset/close path инвалидирует `SupportVersions`
- новый `Connecting` означает новую session scope и не может reuse старые negotiated versions

##### Explicit Wave 2 decisions

Для `Wave 2` дополнительно зафиксированы следующие обязательные решения:

- `SupportVersions` очищаются при любом `disconnect` и любом `reset`
- все inflight requests централизованно завершаются при любом `disconnect` и любом `reset`
- `Open` публикуется только после полного session setup
- `auth failure` обрабатывается отдельно от обычного transport disconnect

Эти решения считаются частью собственного lifecycle contract `NKafka`.
Они опираются на хорошие практики из клиентской экосистемы Kafka, но не означают копирование чужой архитектуры или layering model.

##### Response-loop interaction rules

Для read path вводятся такие обязательные правила:

- steady-state response loop принадлежит только `Open` session
- если response loop теряет stream alignment или получает transport-fatal failure, connector должен перейти в `Faulted`
- `Faulted` read path должен привести к controlled cleanup, а не оставлять dangling inflight tasks
- response loop не должен жить дольше, чем session, которой он принадлежит

##### Error-class mapping to state transitions

`Transport failures`:

- обычно переводят connector в `Faulted`
- затем запускают controlled transition `Faulted -> Closing -> Closed` либо новый reconnect attempt через higher-level path

`Session setup failures`:

- переводят connector в `Faulted`
- не допускают публикацию `Open`
- не допускают публикацию valid `SupportVersions`

`Protocol-fatal response/parsing failures`:

- если доверие к дальнейшему чтению stream потеряно, переводят connector в `Faulted`

`Caller cancellation`:

- сама по себе не должна автоматически трактоваться как connection fault
- но если cancellation произошла посреди cleanup/reset, финальный state всё равно должен быть deterministic

##### Implementation notes for Wave 2

Для `Wave 2` не требуется сразу идеально разнести все transport concerns по новым типам.

Достаточно, чтобы:

- state changes больше не происходили хаотично из разных мест
- reset semantics была централизована
- inflight failure and `SupportVersions` invalidation были привязаны к transition rules
- code and tests выражали ту же lifecycle model, что и эта секция

##### Current implementation status

На текущем этапе в коде уже реализованы следующие части `Wave 2`:

- `KafkaConnector` переведён на lifecycle states `Closed / Connecting / Negotiating / Authenticating / Open / Faulted / Closing`
- `Open` публикуется только из setup path после завершения session establishment
- `SupportVersions` инвалидируются на fault, reset и dispose paths
- inflight requests централизованно завершаются в общем cleanup path
- response loop fault path больше не остаётся isolated background failure и переводит connector в controlled cleanup

При этом `Wave 2` ещё не считается полностью закрытой, пока соответствующие unit tests не подтвердят cleanup и invalidation semantics.

### Wave 3. Session establishment extraction

Цель:

- отделить setup connection session от steady-state request handling

Задачи:

- логически или физически отделить:
  - TCP connect
  - SSL handshake
  - `ApiVersions` negotiation
  - SASL handshake/authentication
  - publication of `SupportVersions`
- добиться того, чтобы `Open` публиковался только после полного завершения setup sequence
- подготовить единый lifecycle hook для новых SASL механизмов, включая SCRAM

Предполагаемые точки кода:

- [src/NKafka/Connection/KafkaConnector.cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.cs)
- [src/NKafka/Connection/KafkaConnector.Auth..cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.Auth..cs)
- [src/NKafka/Connection/Sasl](K:\nkafka\src\NKafka\Connection\Sasl)

Критерий завершения:

- setup sequence читается как отдельный lifecycle path
- auth integration больше не размазывается по steady-state send/receive коду

#### Wave 3 concrete extraction plan

`Wave 3` не требует немедленного выделения новых public или even separate internal types.
На первом проходе достаточно превратить текущий setup path в явный pipeline внутри `KafkaConnector`.

##### Target setup pipeline

Целевой setup pipeline для одной physical session:

1. `EstablishTransportAsync`
2. `NegotiateApiVersionsAsync`
3. `AuthenticateSessionAsync`
4. `PublishOpenState`

Этот pipeline должен оставаться единственным местом, где connector проходит путь от `Closed` до `Open`.

##### Step responsibilities

`EstablishTransportAsync`:

- выполняет TCP connect
- создаёт `NetworkStream`
- выполняет SSL handshake, если он нужен
- не публикует `SupportVersions`
- не публикует `Open`

`NegotiateApiVersionsAsync`:

- выполняет `ApiVersions` request/response sequence
- строит capability snapshot для текущей session
- не публикует `Open`
- не смешивает transport work и SASL work

`AuthenticateSessionAsync`:

- выполняет SASL handshake/authentication, если это требуется конфигурацией
- использует уже установленный transport и уже negotiated protocol context
- не должен сам принимать решения о publication `Open`

`PublishOpenState`:

- публикует `SupportVersions` как валидные для текущей session
- переводит connector в `Open`
- считается последним шагом successful setup sequence

##### Extraction boundaries

При извлечении setup pipeline нужно сохранить следующие границы:

- setup logic не должна утекать обратно в `SendAsync(...)`, кроме необходимого internal send path для setup requests
- steady-state request handling не должно владеть transport/session establishment semantics
- `KafkaConnector.Auth..cs` остаётся местом auth-specific logic, но orchestration auth phase должна читаться из setup pipeline
- publication of `SupportVersions` должна происходить только после успешного завершения negotiation/auth path

##### Recommended implementation order

Для первого прохода `Wave 3` рекомендуется такой порядок:

1. выделить единый private setup entrypoint, который заменит монолитный `ReEstablishConnectionAsync()`
2. вынести transport establishment в отдельный private method
3. вынести `ApiVersions` negotiation и capability publication в отдельный private method
4. вынести auth orchestration в отдельный private method
5. оставить internal send path на месте, но сделать setup sequence читаемым сверху вниз

##### Non-goals for the first pass

На первом проходе `Wave 3` не требуется:

- выносить setup pipeline в отдельный класс
- переписывать response loop
- менять pool/cluster orchestration
- завершать SCRAM integration

##### Success signal

`Wave 3` считается продвинутой вперёд, когда `KafkaConnector` можно читать так:

- connector decides whether a new session must be established
- setup pipeline establishes transport
- setup pipeline negotiates protocol capabilities
- setup pipeline authenticates if needed
- only then connector publishes `Open`

##### Current implementation status

На текущем этапе в коде уже реализованы следующие части `Wave 3`:

- `OpenAsync()` и lazy reconnect идут через один canonical session-establishment path
- setup sequence читается как явный pipeline:
  - `EstablishTransportAsync`
  - `NegotiateApiVersionsAsync`
  - `AuthenticateSessionAsync`
  - `PublishOpenState`
- transport establishment, `ApiVersions` negotiation и SASL orchestration разделены на отдельные internal steps
- focused tests покрывают negotiation publication, SSL transport branch и повторное использование уже открытой session без нового connect

`Wave 3` можно считать завершённой, если дальнейшая работа больше не требует возвращаться к монолитному setup flow внутри `KafkaConnector`.

### Wave 4. Steady-state request/response cleanup

Цель:

- упростить рабочий send/receive path одного physical connection

Задачи:

- формализовать invariants around inflight registration/removal
- стабилизировать single-reader response loop
- описать правила timeout, cancellation, failed write и connection reset
- при необходимости вынести coordination-механику из connector в отдельный internal component, не делая pool владельцем stream

Предполагаемые точки кода:

- [src/NKafka/Connection/KafkaConnector.cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.cs)
- [src/NKafka/Connection/KafkaConnector.ProcessResponses.cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.ProcessResponses.cs)

Критерий завершения:

- write/timeout/reset behavior предсказуем и документирован
- response correlation остаётся корректным при concurrent completion

#### Wave 4 concrete steady-state invariants

`Wave 4` фокусируется только на steady-state path уже установленной physical session.
Setup pipeline из `Wave 3` считается входным условием и не смешивается с этими invariants.

##### Request send invariants

- обычный broker request не должен вычислять effective API version до того, как connector убедился, что current session established
- каждый outgoing request получает correlation id ровно один раз
- inflight registration происходит до фактической отправки bytes в stream
- failed write не должен оставлять request в inflight registry
- steady-state request не должен silently использовать invalidated `SupportVersions`

##### Inflight ownership rules

- один correlation id соответствует ровно одному pending response completion source
- inflight request может завершиться только одним из путей:
  - successful response
  - caller cancellation
  - failed write
  - connector cleanup/reset/disconnect
- completion path не должен пытаться завершать один и тот же request повторно разными исходами
- после successful response или failed completion request должен исчезнуть из inflight registry

##### Response loop invariants

- у одной physical session есть ровно один active response reader
- response loop принадлежит конкретной session и не должен продолжать работу после её cleanup
- response correlation всегда использует response header correlation id как единственный ключ lookup
- response parsing failure для одного frame считается connection-scoped failure, а не локальной ошибкой одного request

##### Timeout and cancellation rules

- caller cancellation завершает конкретный request, но не должна автоматически считаться connection failure
- request timeout policy должна быть явной и не зависеть от случайного поведения background tasks
- если timeout policy выбирает reset/disconnect, все затронутые inflight requests должны завершаться предсказуемо

##### Failed write rules

- если request не удалось записать в stream целиком, он не должен оставаться зарегистрированным как pending response
- failed write должен завершать request понятной transport/protocol exception category
- failed write не должен оставлять response loop в ожидании response для request, который фактически не был отправлен

##### Current implementation status

На текущем этапе в коде уже реализованы следующие части `Wave 4`:

- `SendAsync(...)` сначала убеждается, что session established, и только потом выбирает effective API version
- inflight registration происходит до записи bytes в stream, а failed write сразу удаляет request из inflight registry
- caller cancellation и request timeout идут через единый request lifetime contract, но завершаются разными outcome:
  - caller cancellation -> canceled request
  - timeout -> `ProtocolKafkaException(ErrorCodes.RequestTimedOut, ...)`
- response loop остаётся single-reader и привязан к конкретной physical session через session-scoped token and session id snapshot
- request completion semantics сведены к единому ownership rule:
  - request first leaves inflight registry
  - then it completes as response, cancellation, timeout, failed write or connection cleanup
- focused tests покрывают timeout и failed write alongside the existing cleanup and setup coverage

`Wave 4` можно считать завершённой, если дальнейшая работа больше не требует возвращаться к неявным races между send path, timeout/cancel path и response path внутри `KafkaConnector`.

### Wave 5. Pool topology and orchestration cleanup

Цель:

- привести `KafkaConnectorPool` к роли topology/policy layer

Задачи:

- отделить lifecycle seed connectors от lifecycle broker connectors
- уточнить правила создания новых shared connectors
- определить правила удаления permanently dead connectors
- сократить прямую зависимость metadata orchestration от деталей внутреннего состояния connector

Предполагаемые точки кода:

- [src/NKafka/Connection/KafkaConnectorPool.cs](K:\nkafka\src\NKafka\Connection\KafkaConnectorPool.cs)
- [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs)

Критерий завершения:

- pool принимает policy decisions, но не подменяет cluster metadata layer
- dedicated/shared lifecycle выражены явно

#### Wave 5 concrete extraction plan

`Wave 5` фокусируется только на topology/policy responsibilities around connector ownership.
Session establishment и steady-state request/response semantics из `Wave 2-4` считаются уже стабилизированными.

##### Target pool responsibilities

- pool хранит и различает:
  - bootstrap connectors
  - shared broker connectors
  - dedicated broker connectors
- pool решает:
  - какой shared connector отдать для конкретного broker
  - когда создать новый dedicated connector
  - когда topology update должна добавить, сохранить или удалить connector entry
- pool не решает:
  - metadata refresh policy
  - controller semantics
  - retry policy higher in the stack
  - transport/session internals одного `KafkaConnector`

##### Target cluster responsibilities

- `KafkaCluster` остаётся владельцем:
  - metadata snapshot
  - controller knowledge
  - service-routing semantics
  - decision when topology is usable or stale
- `KafkaCluster` должен передавать в pool уже готовый routing intent, а не спрашивать pool о metadata смысле

##### Wave 5 implementation steps

1. Зафиксировать topology vocabulary in code and spec:
   - bootstrap
   - shared broker
   - dedicated broker
2. Уточнить internal registry model inside `KafkaConnectorPool`:
   - какие коллекции считаются authoritative
   - что является identity для connector entry
3. Разделить lifecycle rules for:
   - bootstrap connectors
   - shared broker connectors
   - dedicated connectors
4. Пересмотреть `AddOrUpdateConnectorsAsync(...)` так, чтобы topology sync не смешивал:
   - connector creation
   - connector reuse
   - dead-entry cleanup
5. Явно определить rules for removing connectors that are no longer represented in metadata.
6. Явно определить ownership rule for dedicated connectors:
   - pool creates them
   - caller owns their usage
   - shared balancing never reuses them
7. Проверить `KafkaCluster` call sites against the clarified ownership model.
8. Добавить focused tests на seed/shared/dedicated lifecycle.

##### Wave 5 non-goals

- не выносить transport ownership из `KafkaConnector`
- не возвращать pool к knowledge of controller semantics
- не смешивать topology cleanup с SCRAM/security work
- не менять public cluster API без прямой необходимости

##### Current implementation target

Для первого прохода `Wave 5` достаточно добиться следующего:

- `KafkaConnectorPool` выражает явную registry model для bootstrap/shared/dedicated connectors
- topology update path не скрывает ownership decisions внутри случайных side effects
- `KafkaCluster` и pool больше не спорят о том, кто отвечает за metadata meaning vs connector ownership

##### Current implementation status

На текущем этапе в коде уже реализованы следующие части `Wave 5`:

- pool использует явную registry model:
  - `_seedConnectors` for bootstrap ownership
  - `_sharedConnectorsByNodeId` for metadata-driven broker traffic
  - `_dedicatedConnectorsByNodeId` for explicit dedicated ownership
  - `_brokerNodesById` as broker identity lookup
- bootstrap promotion оформлена как отдельный `CreateSharedConnector(...)` path
- endpoint matching в pool выполняется по semantic host/port identity, а не по reference equality объектов `EndPoint`
- dedicated connectors создаются и регистрируются отдельно от shared registry
- topology sync читается как отдельный lifecycle pipeline:
  - `RegisterBrokerNode(...)`
  - `EnsureSharedConnectorsForNode(...)`
  - `RemoveStaleSharedConnectors(...)`
  - `OpenSharedConnectorsAsync(...)`
  - `RemoveDeadSharedConnectors(...)`
  - `RemoveSharedConnectorsMissingFromMetadata(...)`
- если broker исчезает из metadata snapshot, pool:
  - удаляет shared connectors для этого broker
  - перестаёт routing metadata-driven traffic к нему
  - удаляет broker lookup entry, если для broker не осталось dedicated ownership
- focused tests подтверждают:
  - bootstrap connector promotion into shared registry
  - shared connector replacement when the same broker id moves to a new endpoint
  - dedicated connector creation is rejected after the broker disappears from metadata
  - `KafkaCluster.ProvideDedicatedConnector(...)` does not ask the pool for a broker that is absent from the current metadata snapshot

Оставшиеся вопросы для следующих шагов `Wave 5`:

- нужно ли явно чистить stale dedicated entries, если owning caller их больше не держит

Дополнительно после проверки call sites:

- `KafkaCluster` теперь трактует controller как valid only if it exists in the current metadata snapshot
- `KafkaCluster.ProvideDedicatedConnector(...)` валидирует broker presence against current metadata before asking pool for a dedicated connector

### Wave 6. Security hardening and SCRAM integration

Цель:

- привести security surface к честной и расширяемой runtime-модели

Задачи:

- встроить SCRAM в production auth path
- синхронизировать config validation с реальной support matrix
- расширить integration coverage для SSL/SASL combinations

Предполагаемые точки кода:

- [src/NKafka/Connection/KafkaConnector.Auth..cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.Auth..cs)
- [src/NKafka/Connection/Sasl](K:\nkafka\src\NKafka\Connection\Sasl)
- [src/NKafka/Config/SaslSettings.cs](K:\nkafka\src\NKafka\Config\SaslSettings.cs)

Критерий завершения:

- runtime support matrix соответствует documentation и config surface
- SCRAM не является скрытым "почти готовым" путём

#### Wave 6 concrete extraction plan

`Wave 6` фокусируется только на security/runtime honesty и на завершении SCRAM authentication path.
Session lifecycle, setup pipeline и pool topology из `Wave 2-5` считаются уже стабилизированными.

##### Target security responsibilities

- `SaslSettings` и cluster config должны принимать только те combinations, для которых есть честный runtime contract
- `KafkaConnector` должен выполнять ровно тот authentication flow, который соответствует выбранному mechanism
- unsupported mechanisms должны fail-fast до того, как connector войдёт в partially-authenticated session
- SCRAM должен использовать тот же setup/send/receive lifecycle, что и другие SASL paths, без special-case transport обходов

##### Target runtime support matrix

Для первого прохода `Wave 6` целевая matrix должна быть выражена явно:

- `PLAIN`
  - supported
  - production auth path implemented
- `OAUTHBEARER`
  - not supported in runtime for now
  - config visibility must not be treated as production-ready auth support
- `SCRAM-SHA-256`
  - supported after `Wave 6`
  - end-to-end runtime path and tests required
- `SCRAM-SHA-512`
  - supported after `Wave 6`
  - end-to-end runtime path and tests required
- `Kerberos/GSSAPI`
  - not supported in runtime for now
  - config validation must not present it as ready-to-use

##### Wave 6 implementation steps

1. Зафиксировать explicit support matrix in code and spec.
2. Синхронизировать `SaslSettings.Validate()` с реальной runtime support matrix.
3. Убрать misleading assumptions, где config surface принимает mechanism без end-to-end runtime path.
4. Выделить явный auth-provider/client selection path inside `KafkaConnector.Auth..cs`.
5. Подключить SCRAM client orchestration к стандартному `SaslHandshake -> token exchange` pipeline.
6. Убедиться, что auth failures остаются отдельной session failure category и не маскируются под обычный transport disconnect.
7. Добавить focused tests на:
   - `PLAIN`
   - `SCRAM-SHA-256`
   - `SCRAM-SHA-512`
   - unsupported mechanism/config combinations

##### Wave 6 non-goals

- не добавлять Kerberos/GSSAPI runtime path в этой волне
- не менять transport lifecycle rules из `Wave 2`
- не возвращаться к pool/topology responsibilities из `Wave 5`
- не расширять public config surface быстрее, чем появляется честная runtime support

##### Current implementation target

Для первого прохода `Wave 6` достаточно добиться следующего:

- `SaslSettings` validates only mechanisms with honest runtime status
- `KafkaConnector` has one explicit mechanism-selection path for auth setup
- SCRAM is integrated into the same session-establishment pipeline as the existing SASL flows
- tests confirm both supported and unsupported security combinations

##### Current implementation status

На текущем этапе перед началом `Wave 6` в коде наблюдается следующая картина:

- `KafkaConnector` already has a dedicated auth setup step via `AuthenticateSessionAsync(...)`
- runtime auth path is connected for:
  - `PLAIN`
  - `SCRAM-SHA-256`
  - `SCRAM-SHA-512`
- `KafkaConnector.Auth..cs` now uses one explicit auth-session contract for:
  - single-stage mechanisms
  - challenge/response mechanisms
- `SaslSettings.Validate()` is now aligned with the honest runtime matrix:
  - `PLAIN` is allowed when credentials are present
  - `SCRAM-SHA-256` and `SCRAM-SHA-512` are allowed when credentials are present
  - `OAUTHBEARER` is rejected as unsupported runtime behavior
  - `Kerberos/GSSAPI` is rejected as unsupported runtime behavior
- SCRAM primitives are connected to the standard `SaslHandshake -> SaslAuthenticate` pipeline without bypassing the connector request/response lifecycle
- `Kerberos/GSSAPI` has config surface only and must currently be treated as unsupported runtime behavior
- focused tests now cover:
  - `SaslSettings` support matrix
  - `ScramSaslClient` challenge/response exchange
  - connector setup path for `PLAIN`
  - explicit rejection of `OAUTHBEARER` until the provider emits real auth data
  - connector setup path for `SCRAM-SHA-256`
  - unsupported mechanism advertised by broker during handshake
  - explicit SASL authentication failure returned by broker

### Wave 7. Hardening, verification and spec sync

Цель:

- закрепить новую архитектуру тестами, документацией и проверкой регрессий

Задачи:

- добавить недостающие unit и integration tests
- перепроверить observability hooks around connection lifecycle
- обновить связанные spec-документы при изменении фактических контрактов
- при необходимости провести performance verification в `benchmarks/`

Критерий завершения:

- основные transport/session regressions прикрыты
- спецификации и код больше не расходятся по ключевым границам

##### Current implementation status

На текущем этапе `Wave 7` в коде уже выполнены следующие hardening pieces:

- focused verification now covers:
  - connector lifecycle cleanup and inflight invalidation
  - setup pipeline for `PLAIN` and `SCRAM-SHA-256`
  - explicit rejection of `OAUTHBEARER` until its runtime auth data path is implemented
  - negative SASL paths for unsupported broker-advertised mechanism and explicit authenticate failure
- connector observability has been tightened around lifecycle decisions:
  - state transitions are logged explicitly
  - negotiated API-version invalidation is logged when a session is torn down
  - fault handling now emits a dedicated warning with the connection-scope reason
  - SASL authentication start is logged with the selected mechanism
- focused verification now also checks the observability surface itself:
  - session setup emits expected state-transition logs
  - authenticated setup logs the selected SASL mechanism
  - session teardown logs negotiated API-version invalidation
  - faulted authentication emits the dedicated connection-fault warning
- integration test infrastructure is now less tightly coupled to plaintext-only local defaults:
  - integration cluster creation is centralized
  - bootstrap servers and security settings can be overridden through environment variables
  - future SASL/SSL scenarios can reuse the same test entrypoint instead of forking client bootstrap code
- the first opt-in integration-style security scenario now exists:
  - cluster describe can run against a non-plaintext broker configuration
  - the scenario stays gated behind explicit environment variables so ordinary local runs do not start failing on missing secure test infrastructure
- there is now also a `PLAIN`-specific opt-in integration entrypoint:
  - cluster describe can be exercised specifically against `SASL/PLAIN`
  - the scenario is isolated from generic security gating so `PLAIN` regressions can be diagnosed without conflating them with `SCRAM`
- there is also a SCRAM-specific opt-in integration entrypoint:
  - cluster describe can be exercised against `SCRAM-SHA-256` or `SCRAM-SHA-512`
  - the scenario is isolated from generic security gating so SCRAM failures are easier to diagnose against a secure broker setup
- `OAUTHBEARER` no longer has an opt-in integration entrypoint because the current provider remains a placeholder and must not be presented as a supported runtime mechanism

##### Integration security scenario inputs

The current integration-style security checks are opt-in and expect broker configuration to be supplied through environment variables.

Supported inputs:

- `NKAFKA_IT_ENABLE_SECURITY_SCENARIOS=true` enables security-specific integration tests
- `NKAFKA_IT_BOOTSTRAP_SERVERS` overrides the default `localhost:29091`
- `NKAFKA_IT_SECURITY_PROTOCOL` accepts `Ssl`, `SaslPlaintext` or `SaslSsl`
- `NKAFKA_IT_TRUST_SERVER_CERTIFICATE` controls whether self-signed certificates are accepted in integration environments
- `NKAFKA_IT_SASL_MECHANISM` accepts `Plain`, `ScramSha256` or `ScramSha512` for currently supported runtime scenarios
- `NKAFKA_IT_SASL_USERNAME` and `NKAFKA_IT_SASL_PASSWORD` provide credentials for SASL-based runs

When `NKAFKA_IT_ENABLE_SECURITY_SCENARIOS=true`:

- `NKAFKA_IT_SECURITY_PROTOCOL` becomes mandatory and may not remain `PlainText`
- SASL-based runs require explicit `NKAFKA_IT_SASL_MECHANISM`, `NKAFKA_IT_SASL_USERNAME` and `NKAFKA_IT_SASL_PASSWORD`
- missing security inputs now fail fast with a configuration error instead of silently falling back to placeholder credentials

Example `SCRAM-SHA-256` run:

```powershell
$env:NKAFKA_IT_ENABLE_SECURITY_SCENARIOS = "true"
$env:NKAFKA_IT_BOOTSTRAP_SERVERS = "localhost:29092"
$env:NKAFKA_IT_SECURITY_PROTOCOL = "SaslPlaintext"
$env:NKAFKA_IT_SASL_MECHANISM = "ScramSha256"
$env:NKAFKA_IT_SASL_USERNAME = "user"
$env:NKAFKA_IT_SASL_PASSWORD = "pencil"
dotnet test tests/NKafka.IntegrationTests/NKafka.IntegrationTests.csproj --no-restore -f net9.0 --filter "FullyQualifiedName~ClusterDescribeSecurityTests"
```

Recommended targeted filters:

- all security integration entrypoints:
  - `dotnet test tests/NKafka.IntegrationTests/NKafka.IntegrationTests.csproj --no-restore -f net9.0 --filter "Category=SecurityIntegration"`
- only `PLAIN`:
  - `dotnet test tests/NKafka.IntegrationTests/NKafka.IntegrationTests.csproj --no-restore -f net9.0 --filter "SecurityMechanism=PLAIN"`
- only `SCRAM`:
  - `dotnet test tests/NKafka.IntegrationTests/NKafka.IntegrationTests.csproj --no-restore -f net9.0 --filter "SecurityMechanism=SCRAM"`

Оставшиеся задачи `Wave 7`:

- добрать integration-style verification outside the focused mock-stream scenarios where it adds confidence
- провести финальную синхронизацию связанных spec-документов, если найдутся ещё расхождения по wording or support matrix

## Cross-cutting testing plan

Минимальный expected test plan по волнам:

- Wave 1: unit tests на pool selection policy и boundary contracts, если сигнатуры меняются
- Wave 2: unit tests на state transitions, reset, inflight invalidation, `SupportVersions` invalidation
- Wave 3: integration tests или focused tests на setup sequence `connect -> SSL -> ApiVersions -> SASL`
- Wave 4: regression tests на correlation, failed write, read loop shutdown, timeouts
- Wave 5: tests на seed/shared/dedicated connector lifecycle
- Wave 6: integration tests на `PLAIN`, SCRAM и unsupported combinations; `OAUTHBEARER` не должен считаться supported runtime path до появления реального provider implementation

## Risks and anti-goals during execution

Во время выполнения плана особенно важно избегать следующих ошибок:

- начинать с дробления `KafkaConnector` без фиксации верхних boundaries
- выносить transport ownership в `KafkaConnectorPool`
- сохранять старые `SupportVersions` после reconnect
- маскировать protocol parsing failures под обычный reconnect
- расширять config surface быстрее, чем появляется реальная runtime support
- делать рефакторинг сразу во многих файлах без промежуточных тестируемых остановок

## Stage 1 detailed agenda

Первый этап должен решить именно верхнеуровневые вопросы, а не писать новую transport-механику.

### Stage 1 deliverables

На выходе этапа должны появиться:

- матрица ответственности по слоям
- список текущих boundary violations в коде
- набор candidate internal contracts
- список открытых решений, без которых опасно переходить к state-machine refactoring

### Stage 1 responsibility matrix

`KafkaCluster`:

- владеет cluster config, metadata cache и metadata refresh policy
- выбирает broker по cluster semantics
- решает, когда нужен metadata refresh и когда transport error должен стать metadata hint
- запрашивает у pool shared или dedicated connector

`KafkaConnectorPool`:

- хранит seed endpoints и registry broker connectors
- выбирает shared connector по policy
- создаёт dedicated connector по явному запросу
- удаляет или помечает невалидные connectors по transport health signals

`KafkaConnector`:

- владеет одной physical broker session
- открывает TCP/SSL/SASL session
- выполняет `ApiVersions` negotiation
- ведёт request send path, correlation и response completion
- публикует connection-local health and capability state

`Internal session components`:

- framing and parsing
- inflight bookkeeping
- auth exchange implementation
- state transition mechanics

### Stage 1 current boundary violations to review

Предварительно стоит проверить и уточнить следующие зоны:

- `KafkaCluster` может быть слишком близко к текущему connector selection и metadata-fetch mechanics
- `KafkaConnectorPool` знает не только topology, но и часть lifecycle orchestration, что усложняет его роль
- `KafkaConnector` совмещает setup, send path, read loop и inflight completion без явного internal split
- TODO про возможный перенос response processing из connector в pool пока сформулирован слишком широко и может увести архитектуру не туда

## Open questions for Stage 1

Ниже собраны вопросы, которые нужно уточнить до начала активного рефакторинга.

### 1. Какой минимальный internal contract нужен между cluster и pool?

Нужно определить:

- должен ли `KafkaCluster` запрашивать "connector for node" или "routeable channel for operation kind"
- нужен ли cluster доступ к health/capability summary без выхода на `IKafkaConnector`
- где должна жить политика выбора dedicated vs shared connection: полностью в cluster или частично в pool

Почему это важно:

- без этого граница `KafkaCluster -> KafkaConnectorPool` останется слишком процедурной

### 2. Что считается transport signal, а что metadata decision?

Нужно определить:

- какие transport failures автоматически поднимают metadata invalidation hint
- может ли pool сам удалять broker mapping или только помечать connector как unusable
- должен ли cluster различать `faulted connector`, `stale metadata hint` и `unknown broker topology`

Почему это важно:

- иначе reconnect cleanup и metadata refresh будут продолжать смешиваться

### 3. Какой ownership у dedicated connectors?

Нужно определить:

- кто освобождает dedicated connector: owning client, pool или cluster
- должен ли dedicated connector регистрироваться в общем broker registry
- могут ли dedicated connectors участвовать в capability aggregation или только shared connectors

Почему это важно:

- dedicated lifecycle сильно влияет на consumer/coordinator и future transactional paths

### 4. Нужен ли отдельный internal component для response coordination?

Нужно определить:

- остаётся ли response processing полностью внутри connector
- выносится ли только completion/parsing coordination
- нужно ли это вообще до стабилизации state machine

Почему это важно:

- преждевременный вынос response logic может ухудшить, а не улучшить границы

### 5. Как публиковать `SupportVersions` наверх?

Нужно определить:

- нужна ли cluster-level aggregated capability snapshot
- можно ли строить её только по opened shared connectors
- как вести себя, если dedicated connector negotiated versions отличаются после reconnect или broker rollout

Почему это важно:

- `SupportVersions` уже фигурирует и в transport, и в compatibility/fallback semantics

### 6. Как выражать health state connector для pool без утечки stream internals?

Нужно определить:

- достаточно ли статусов `Open / Closed / Faulted / Connecting`
- нужны ли отдельные признаки `CanAcceptWrites`, `Negotiated`, `Authenticated`
- должен ли pool видеть число inflight requests и idle state, но не видеть stream object

Почему это важно:

- без этого pool либо будет слепым, либо снова начнёт зависеть от деталей connector implementation

### 7. Где должна жить policy повторного открытия connection?

Нужно определить:

- инициирует ли reconnect только `KafkaConnector`
- допускается ли lazy reconnect на первом write после reset
- где заканчивается transport retry и начинается higher-level retry policy

Почему это важно:

- reconnect semantics напрямую влияют на deterministic behavior inflight requests

## Stage 1 working decisions

Ниже зафиксированы рекомендуемые рабочие решения для начала 1 этапа. Это не окончательный wire contract, но достаточно конкретная стартовая позиция для следующих правок.

### 1. Contract between `KafkaCluster` and `KafkaConnectorPool`

Рекомендуемое решение:

- `KafkaCluster` выбирает semantic target: конкретный `nodeId`, либо service/seed path для bootstrap and metadata
- `KafkaCluster` также решает, нужен shared или dedicated connector
- `KafkaConnectorPool` не принимает решений уровня "какая операция куда должна идти", а только возвращает подходящий connector по уже выбранной стратегии

Что это означает для кода:

- текущий `TryGetConnector(nodeId, isDedicated, out connector)` концептуально близок к целевой границе
- `GetConnector()` выглядит слишком неявным и со временем должен быть заменён более явным service/bootstrap contract
- pool не должен получать knowledge about operation kind beyond routing mode, выбранный cluster layer

### 2. Transport signals vs metadata decisions

Рекомендуемое решение:

- connector и pool поднимают только transport hints
- metadata invalidation, topology refresh и broker-removal semantics остаются на стороне `KafkaCluster`

Примеры transport hints:

- connector faulted
- connection setup failed
- negotiated capabilities invalidated after reconnect
- no usable connector for known node

Что это означает для кода:

- pool может помечать connector unusable и исключать его из выбора
- окончательное решение о metadata refresh не должно жить в pool
- `KafkaCluster` должен различать transport fault и stale metadata как разные причины, даже если они иногда ведут к одному действию

### 3. Ownership model for dedicated connectors

Рекомендуемое решение:

- dedicated connector принадлежит owning client/session
- pool может создавать такой connector и при необходимости знать о его существовании для diagnostics/disposal, но не должен переиспользовать его как shared
- dedicated connectors не должны участвовать в обычном balancing shared traffic

Что это означает для кода:

- `ProvideDedicateConnector()` в [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs) соответствует направлению, но lifecycle ownership нужно сделать явнее
- dedicated connector не должен быть неявной частью cluster-wide connector selection policy

### 4. Response coordination extraction

Рекомендуемое решение:

- на 1 этапе не выносить response processing в `KafkaConnectorPool`
- сначала стабилизировать state machine и inflight semantics внутри connector
- если позже выносить механику, то только в отдельный internal helper/component с сохранением stream ownership за connector

Что это означает для кода:

- TODO в `KafkaConnector` про перенос response processing в pool пока не должен трактоваться как прямой план реализации
- premature extraction сейчас даст больше architectural noise, чем пользы

### 5. Publication of `SupportVersions`

Рекомендуемое решение:

- `SupportVersions` остаётся connection-scoped state
- cluster-level aggregated capability snapshot строится только по currently opened shared connectors
- dedicated connectors используют свои negotiated versions локально и по умолчанию не влияют на cluster aggregate

Что это означает для кода:

- текущий `MergeAllVersions()` в [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs) идейно близок к нужной задаче, но его нужно увязать с explicit invalidation rules
- после reconnect у connector capability state должен пересчитываться, а не silently reused

### 6. Minimal health surface for pool

Рекомендуемое решение:

- pool должен видеть только transport-safe health surface connector
- минимально достаточно:
  - lifecycle state
  - `IsDedicated`
  - current inflight count
  - признак, что connector готов принимать writes
  - endpoint and node identity

Что это означает для кода:

- pool не должен знать о `Stream`, SSL wrapping details или внутренней auth mechanics
- `ConnectorState` в текущем виде полезен, но на следующих этапах ему понадобится более точная state model

### 7. Reconnect ownership

Рекомендуемое решение:

- reconnect на transport-уровне остаётся ответственностью connector
- higher-level retry decision остаётся выше: в `KafkaCluster` и client workflows
- lazy reconnect допустим, если при этом:
  - старые inflight requests завершаются детерминированно
  - `SupportVersions` сбрасывается до новой negotiation
  - protocol-fatal failures не маскируются под обычный reconnect

Что это означает для кода:

- текущее поведение с `ReEstablishConnectionAsync()` внутри send path похоже на допустимое направление, но его нужно формализовать и очистить от неявных side effects

## Stage 1 code review focus

При прохождении первого этапа стоит в первую очередь перепроверить следующие участки:

- [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs): `GetConnectorForServiceRequests()`, `InternalRefreshMetadataAsync()`, `MergeAllVersions()`, `ProvideDedicateConnector()`
- [src/NKafka/Connection/IKafkaConnectorPool.cs](K:\nkafka\src\NKafka\Connection\IKafkaConnectorPool.cs): неявность `GetConnector()` и смешение discovery/policy concerns
- [src/NKafka/Connection/KafkaConnectorPool.cs](K:\nkafka\src\NKafka\Connection\KafkaConnectorPool.cs): seed/broker/shared/dedicated lifecycle и `TakeLeastLoaded()`
- [src/NKafka/Connection/IKafkaConnector.cs](K:\nkafka\src\NKafka\Connection\IKafkaConnector.cs): слишком широкая видимость connection-local capabilities наверх
- [src/NKafka/Connection/KafkaConnector.cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.cs): смешение `OpenAsync`, lazy reconnect и steady-state send path

## Stage 1 analysis results

Ниже зафиксированы результаты верхнеуровневого разбора текущего кода относительно целей 1 этапа.

### What already aligns with the target architecture

Текущее состояние уже содержит несколько полезных опор:

- `KafkaCluster` действительно остаётся основным owner metadata и cluster lifecycle
- `KafkaConnectorPool` уже отделён как самостоятельный слой выдачи connectors
- `KafkaConnector` уже является physical-session abstraction, а не просто socket wrapper
- distinction `shared` vs `dedicated` уже присутствует и может быть усилен, а не изобретён с нуля
- aggregated API capabilities уже считаются на cluster level через `MergeAllVersions()`

Это важно, потому что 1 этап можно делать как controlled boundary cleanup, а не как полную архитектурную перестройку.

### Boundary violations in `KafkaCluster`

#### 1. Cluster relies on an implicit pool routing contract

Проблема:

- `GetConnectorForServiceRequests()` использует `_connectorPool.GetConnector()` как fallback без явного различения seed/bootstrap path и ordinary shared broker routing

Почему это нарушение границы:

- cluster выбирает semantic target только частично
- финальное решение "какой именно тип service route сейчас допустим" скрыто в pool implementation

Риск:

- bootstrap semantics и steady-state service routing сцеплены через один неявный метод

Рекомендуемое направление:

- заменить смысл `GetConnector()` на более явный internal contract уровня service/bootstrap connector selection

#### 2. Cluster aggregates connector-local capabilities напрямую через connector enumeration

Проблема:

- `MergeAllVersions()` обходит `_connectorPool.GetAllOpenedConnectors()` и читает `connector.SupportVersions`

Почему это нарушение границы:

- cluster знает о connector-local capability storage слишком напрямую
- aggregated capability model зависит от деталей того, какие connectors pool решил держать открытыми

Риск:

- сложно формализовать, должны ли dedicated connectors участвовать в aggregate
- reconnect invalidation semantics пока не выражена явно

Рекомендуемое направление:

- сохранить aggregation на стороне cluster, но сузить и формализовать capability snapshot contract от pool/connectors

#### 3. Cluster owns dedicated-connector acquisition but not its lifecycle contract

Проблема:

- `ProvideDedicateConnector()` выдаёт dedicated connector, но ownership semantics дальше не выражена

Почему это нарушение границы:

- решение "кому принадлежит dedicated connector после выдачи" не зафиксировано в контракте

Риск:

- дальше будет трудно отделить consumer/coordinator lifetime от pool registry lifetime

Рекомендуемое направление:

- на уровне 1 этапа хотя бы документально закрепить ownership за owning client/session

### Boundary violations in `IKafkaConnectorPool` and `KafkaConnectorPool`

#### 1. `GetConnector()` is too implicit

Проблема:

- интерфейс `IKafkaConnectorPool` содержит `GetConnector()` без явного указания, выбирается ли seed connector, random broker connector или service connector

Почему это нарушение границы:

- contract скрывает policy decisions, которые важны для cluster layer

Риск:

- на уровне вызова невозможно понять, идёт ли речь о bootstrap path или normal service path

Рекомендуемое направление:

- заменить его на явно названный internal contract или хотя бы разделить семантику на bootstrap/service selection

#### 2. Pool mixes registry and lifecycle orchestration

Проблема:

- `AddOrUpdateConnectorsAsync()` не только обновляет topology registry, но и открывает connectors, ловит ошибки, удаляет dead connections и чистит registry

Почему это нарушение границы:

- pool одновременно хранит topology, создаёт connectors и исполняет connection-liveness cleanup

Риск:

- дальше трудно будет отделить topology update от transport health handling

Рекомендуемое направление:

- на 1 этапе пока не дробить aggressively, но выделить эти подзадачи хотя бы логически и по методам

#### 3. Pool still contains seed-to-broker migration policy details

Проблема:

- `UpdateConnectors()` сам решает, использовать ли seed connector как broker connector, либо создать новый

Почему это нарушение границы:

- seed lifecycle и broker lifecycle пока живут в одном mutation path

Риск:

- bootstrap topology cleanup остаётся сложным для reasoning и тестирования

Рекомендуемое направление:

- выделить seed-to-broker promotion как отдельную осознанную policy

#### 4. Dedicated connectors are created ad hoc and remain loosely modeled

Проблема:

- `TryDedicateConnector()` всегда создаёт новый dedicated connector, но интерфейс не выражает ownership, disposal и relationship to broker registry

Почему это нарушение границы:

- dedicated connection model есть в коде, но не завершена как contract

Риск:

- dedicated path будет разрастаться разрозненно по consumer and transactional scenarios

Рекомендуемое направление:

- не менять behaviour резко, но формализовать dedicated creation contract и жизненный цикл

### Boundary violations in `IKafkaConnector` and `KafkaConnector`

#### 1. Connector exposes too much connection-local state upward

Проблема:

- `IKafkaConnector` наружу отдаёт mutable/low-level capability surface: `NodeId`, `SupportVersions`, `ConnectorState`, inflight count

Почему это нарушение границы:

- higher layers работают почти с raw connector internals, а не с устойчивым health/capability contract

Риск:

- любое изменение lifecycle или capability publication будет протекать через весь стек

Рекомендуемое направление:

- на 1 этапе не ломать интерфейс полностью, но определить целевой minimal health surface и future capability snapshot contract

#### 2. `OpenAsync()` and `SendAsync()` overlap on reconnect responsibilities

Проблема:

- `OpenAsync()` вызывает `ReEstablishConnectionAsync()`, но `SendAsync()` почти для каждого запроса тоже делает `ReEstablishConnectionAsync()`

Почему это нарушение границы:

- setup lifecycle и steady-state request path уже смешаны до state-machine refactoring

Риск:

- трудно определить, какой слой владеет reopen semantics и когда `SupportVersions` считается валидным

Рекомендуемое направление:

- зафиксировать это как главный объект следующей волны, но пока не разносить логику без формальной state model

#### 3. Connector contains a pool-level architectural TODO

Проблема:

- в `KafkaConnector` зашит TODO о переносе response processing в `KafkaConnectorPool`

Почему это нарушение границы:

- само направление TODO конфликтует с целевым правилом: pool не должен становиться transport owner

Риск:

- такой TODO подталкивает к неверной архитектурной развязке

Рекомендуемое направление:

- переписать архитектурный intent: возможен вынос в internal response coordinator, но не в pool как новый владелец stream/session

#### 4. Idle-close timer embeds orchestration policy in connector

Проблема:

- `_closeConnectionAfterTimeout` и `ResetConnection()` зашивают часть lifecycle/orchestration policy внутрь connector

Почему это нарушение границы:

- connector здесь не только session owner, но и policy executor по idle management

Риск:

- future pool-level lifecycle cleanup будет конфликтовать с локальными timer-driven resets

Рекомендуемое направление:

- не удалять сразу, но явно пометить как policy concern, который позже надо согласовать с state machine и pool lifecycle model

### Stage 1 conclusions

По итогам разбора 1 этапа можно сделать несколько практических выводов:

- базовое разделение `Cluster / Pool / Connector` уже существует и его лучше усиливать, а не переписывать
- главный architectural smell на верхнем уровне - неявные контракты, а не отсутствие слоёв как таковых
- самым проблемным контрактом сейчас является `KafkaCluster -> IKafkaConnectorPool`, потому что `GetConnector()` скрывает слишком много policy
- вторым по важности является publication of connector state and capabilities upward without a stable snapshot contract
- `KafkaConnector` пока рано дробить механически; сначала нужно закрепить, что именно higher layers имеют право о нём знать

## Stage 1 minimal refactoring backlog

Ниже предложен минимальный backlog, который двигает 1 этап вперёд без раннего входа в глубокий transport refactoring.

### Backlog item 1. Make pool selection contracts explicit

Цель:

- убрать неявность вокруг `IKafkaConnectorPool.GetConnector()`

Минимальный результат:

- либо переименовать/разделить API pool на bootstrap/service semantics
- либо добавить промежуточный internal abstraction, который явно кодирует selection intent

Не цель:

- менять низкоуровневую send/receive механику

### Backlog item 2. Freeze dedicated connector ownership semantics

Цель:

- документально и контрактно закрепить, что dedicated connector принадлежит owning client/session

Минимальный результат:

- прояснить disposal expectations
- решить, должен ли pool только создавать dedicated connector или ещё и отслеживать его для diagnostics/disposal

Не цель:

- немедленно переписывать consumer coordination

### Backlog item 3. Define minimal connector health surface

Цель:

- определить, что именно pool и cluster могут легально читать из connector

Минимальный результат:

- согласовать future contract для lifecycle state, inflight count, write-readiness и capability snapshot

Не цель:

- сразу прятать все свойства `IKafkaConnector`

### Backlog item 4. Reframe response-processing TODO

Цель:

- убрать misleading architectural direction про перенос response processing в pool

Минимальный результат:

- зафиксировать в коде или spec, что допустим вынос только в internal coordination component без переноса stream ownership в pool

Не цель:

- прямо сейчас выносить response coordination

### Backlog item 5. Separate topology update from connector liveness cleanup logically

Цель:

- сделать `KafkaConnectorPool.AddOrUpdateConnectorsAsync()` менее смешанным по ролям

Минимальный результат:

- хотя бы логическое разделение на:
  - topology sync
  - connector open attempt
  - dead connector cleanup

Не цель:

- глубокая перестройка lifecycle пока без state machine

## Explicit pool contract proposal

Ниже предложен явный internal contract для `KafkaCluster -> IKafkaConnectorPool`, который должен заменить текущую неявную комбинацию `TryGetConnector(nodeId, isDedicated, out connector)` и `GetConnector()`.

### Design intent

Контракт должен выражать не "дай какой-нибудь connector", а конкретное routing intent:

- нужен shared connector для конкретного broker
- нужен dedicated connector для конкретного broker
- нужен любой доступный shared broker connector
- нужен bootstrap connector из seed list

Такой контракт делает выбор маршрута явным на стороне `KafkaCluster`, а pool отвечает только за исполнение выбранной стратегии.

### Proposed shape

Рекомендуемая форма API:

```csharp
internal interface IKafkaConnectorPool : IDisposable, IAsyncDisposable
{
    bool TryGetSharedConnector(int nodeId, out IKafkaConnector connector);

    bool TryCreateDedicatedConnector(int nodeId, out IKafkaConnector connector);

    bool TryGetAnySharedBrokerConnector(out IKafkaConnector connector);

    bool TryGetBootstrapConnector(out IKafkaConnector connector);

    IEnumerable<IKafkaConnector> GetOpenedSharedConnectors();

    ValueTask AddOrUpdateConnectorsAsync(IEnumerable<Node> nodes, CancellationToken token);
}
```

### Why methods are better than a generic selector enum

На этом этапе лучше использовать отдельные методы, а не единый selector object или enum-based request, по следующим причинам:

- текущих routing intents мало и они хорошо различимы
- call sites в `KafkaCluster` и tests станут читаемее сразу
- это минимальный шаг, который не требует новой orchestration abstraction поверх существующего кода

Если позже появятся дополнительные routing modes, их можно будет свернуть в selector object, но для 1 этапа это преждевременно.

### Semantics of each method

#### `TryGetSharedConnector(int nodeId, out IKafkaConnector connector)`

Семантика:

- вернуть shared connector для известного broker node
- dedicated connectors не участвуют в выборе
- pool сам выбирает least-loaded shared connector для этого node

Использовать для:

- ordinary broker-targeted cluster requests
- controller-targeted requests после того, как cluster уже выбрал `controllerId`

#### `TryCreateDedicatedConnector(int nodeId, out IKafkaConnector connector)`

Семантика:

- создать новый dedicated connector для заданного broker node
- connector не должен попадать в shared balancing path
- ownership после выдачи принадлежит owning client/session

Использовать для:

- coordinator-bound consumer path
- future long-lived affinity-sensitive flows

#### `TryGetAnySharedBrokerConnector(out IKafkaConnector connector)`

Семантика:

- вернуть любой пригодный shared connector среди известных brokers
- использовать только когда cluster уже решил, что operation не требует конкретного node и metadata уже загружена

Использовать для:

- service requests, которые допускают любой broker после bootstrap phase

#### `TryGetBootstrapConnector(out IKafkaConnector connector)`

Семантика:

- вернуть connector из seed list
- использовать только как bootstrap fallback до появления usable broker routing

Использовать для:

- initial metadata fetch
- fallback path, когда cluster ещё не имеет usable broker mapping

#### `GetOpenedSharedConnectors()`

Семантика:

- вернуть только открытые shared connectors
- dedicated connectors не участвуют в cluster-level capability aggregation по умолчанию

Использовать для:

- cluster-level aggregated `SupportVersions`
- observability and diagnostics at cluster scope

### Mapping from current contract to proposed contract

Текущее состояние:

- `TryGetConnector(nodeId, false, out connector)` -> `TryGetSharedConnector(nodeId, out connector)`
- `TryGetConnector(nodeId, true, out connector)` -> `TryCreateDedicatedConnector(nodeId, out connector)`
- `GetConnector()` -> либо `TryGetAnySharedBrokerConnector(out connector)`, либо `TryGetBootstrapConnector(out connector)` в зависимости от semantic intent
- `GetAllOpenedConnectors()` -> `GetOpenedSharedConnectors()`

### Expected usage in `KafkaCluster`

`SendAsync(message, nodeId, token)`:

```csharp
if (_connectorPool.TryGetSharedConnector(nodeId, out var connector))
{
    return connector.SendAsync<TRequestMessage, TResponseMessage>(message, false, token);
}
```

`ProvideDedicateConnector(nodeId)`:

```csharp
if (_connectorPool.TryCreateDedicatedConnector(nodeId, out var connector))
{
    return connector;
}
```

`GetConnectorForServiceRequests()`:

```csharp
if (_controllerId != Node.NoNode.Id &&
    _connectorPool.TryGetSharedConnector(_controllerId, out var controllerConnector))
{
    return controllerConnector;
}

if (_connectorPool.TryGetAnySharedBrokerConnector(out var brokerConnector))
{
    return brokerConnector;
}

if (_connectorPool.TryGetBootstrapConnector(out var bootstrapConnector))
{
    return bootstrapConnector;
}

throw new ConnectorNotFoundException(...);
```

### Ownership rules preserved by this contract

Контракт специально сохраняет целевое разделение ответственности:

- `KafkaCluster` выбирает semantic routing intent
- `KafkaConnectorPool` исполняет connector selection policy
- `KafkaConnector` остаётся physical connection/session abstraction

Pool по этому контракту:

- не принимает metadata decisions
- не знает operation semantics beyond explicit method chosen by cluster
- не становится transport owner for response processing

### Migration strategy for the contract

Минимальная безопасная последовательность внедрения:

1. добавить новые методы в `IKafkaConnectorPool` и `KafkaConnectorPool` поверх текущей логики
2. перевести `KafkaCluster` на новые explicit methods
3. обновить tests
4. удалить старые `GetConnector()` и `TryGetConnector(nodeId, isDedicated, ...)`

### Out of scope for this contract change

Этот шаг сам по себе не должен:

- менять `IKafkaConnector.SendAsync()` semantics
- менять reconnect behavior
- менять state model connector
- менять auth/setup flow

## Routing table for upper layers

Ниже зафиксирована рабочая routing table для верхнего уровня. Её задача - убрать неявность вокруг того, какой слой и в каком сценарии должен запрашивать bootstrap, shared или dedicated connector.

| Scenario | Caller | Target knowledge | Connector kind | Acquisition path | Notes |
|---|---|---|---|---|---|
| Initial metadata bootstrap | `KafkaCluster` | topology unknown | bootstrap | `TryGetBootstrapConnector` | Используется только до первого usable metadata |
| Metadata refresh after topology known | `KafkaCluster` | topology known, specific broker not required | shared | `TryGetAnySharedBrokerConnector` | Обычный service path после bootstrap |
| Controller-required admin/service request | `KafkaCluster` | `controllerId` known | shared | `TryGetSharedConnector(controllerId, out connector)` | Без fallback в bootstrap, если controller обязателен |
| Generic service/admin request to any broker | `KafkaCluster` | topology known | shared | `TryGetAnySharedBrokerConnector` | Для операций без broker affinity |
| Broker-targeted request | `KafkaCluster` or clients via cluster | `nodeId` known | shared | `TryGetSharedConnector(nodeId, out connector)` | Обычный targeted path |
| Consumer discovery (`FindCoordinator`, initial metadata, leader discovery) | consumer via `KafkaCluster` | unknown or partially known | shared or bootstrap | `IKafkaCluster.SendAsync(message, token)` | Consumer сам pool не выбирает |
| Consumer fetch/list-offset to known broker | consumer via `KafkaCluster` | `nodeId` known | shared | `IKafkaCluster.SendAsync(message, nodeId, token)` | Разовый broker-targeted запрос |
| Consumer coordinator session (`JoinGroup`, `Heartbeat`, `SyncGroup`, `OffsetCommit` via coordinator) | consumer | `coordinator.NodeId` known | dedicated | `ProvideDedicateConnector(nodeId)` | Long-lived affinity channel |
| Producer metadata/service request | producer via `KafkaCluster` | specific broker not required | shared | `IKafkaCluster.SendAsync(message, token)` | Обычный cluster service path |
| Producer send to partition leader | producer via `KafkaCluster` | leader `nodeId` known | shared | `IKafkaCluster.SendAsync(message, nodeId, token)` | Dedicated обычно не нужен |
| Admin request to specific controller or broker | admin via `KafkaCluster` | target known | shared | `IKafkaCluster.SendAsync(message, nodeId, token)` or controller path | Зависит от semantics API |
| Future transactional/coordinator-affine flow | producer/admin internal flow | coordinator known | dedicated | `ProvideDedicateConnector(nodeId)` | Только если session affinity реально нужна |

### Routing rules derived from the table

Из таблицы следуют несколько обязательных правил:

- bootstrap connector запрашивает только `KafkaCluster`, и только пока cluster routing ещё не инициализирован metadata-driven topology
- `producer`, `consumer` и `admin` не должны напрямую выбирать bootstrap path через pool
- если известен `nodeId` и нужна разовая broker-targeted операция, должен использоваться shared connector
- если нужен долгоживущий channel с connection affinity, должен использоваться dedicated connector
- controller-required path не должен тихо fallback-иться в bootstrap после завершения bootstrap phase

### Implication for `KafkaCluster`

`KafkaCluster` должен быть явным владельцем routing phase decision:

- topology unknown: bootstrap routing
- topology known and broker not fixed: any shared broker routing
- target broker known: shared broker routing by `nodeId`
- coordinator/session affinity required: dedicated connector provisioning

Это означает, что верхний слой должен принимать решение о routing phase на основании cluster state, а не по косвенным признакам внутри pool.

## API sketch for explicit routing contracts

Ниже приведён рекомендуемый набор сигнатур для следующей волны изменений. Это ещё не обязательный final form, но достаточно конкретный контракт, чтобы на него можно было опираться в коде и тестах.

### `IKafkaConnectorPool`

Рекомендуемые сигнатуры:

```csharp
internal interface IKafkaConnectorPool : IDisposable, IAsyncDisposable
{
    bool TryGetSharedConnector(int nodeId, out IKafkaConnector connector);

    bool TryCreateDedicatedConnector(int nodeId, out IKafkaConnector connector);

    bool TryGetAnySharedBrokerConnector(out IKafkaConnector connector);

    bool TryGetBootstrapConnector(out IKafkaConnector connector);

    IEnumerable<IKafkaConnector> GetOpenedSharedConnectors();

    ValueTask AddOrUpdateConnectorsAsync(IEnumerable<Node> nodes, CancellationToken token);
}
```

Назначение методов:

- `TryGetSharedConnector(int nodeId, out IKafkaConnector connector)`:
  вернуть shared connector для конкретного broker node
- `TryCreateDedicatedConnector(int nodeId, out IKafkaConnector connector)`:
  создать новый dedicated connector для owning client/session
- `TryGetAnySharedBrokerConnector(out IKafkaConnector connector)`:
  вернуть любой пригодный shared connector после загрузки topology
- `TryGetBootstrapConnector(out IKafkaConnector connector)`:
  вернуть connector из seed list для bootstrap phase
- `GetOpenedSharedConnectors()`:
  вернуть открытые shared connectors для cluster-level aggregation and diagnostics

### `IKafkaCluster`

Публично-внутренний контракт cluster layer можно сохранить близким к текущему, но сделать его семантически более явным.

Рекомендуемые сигнатуры:

```csharp
internal interface IKafkaCluster : IDisposable, IAsyncDisposable
{
    Task<TResponseMessage> SendAsync<TRequestMessage, TResponseMessage>(
        TRequestMessage message,
        CancellationToken token)
        where TResponseMessage : class, IResponseMessage
        where TRequestMessage : class, IRequestMessage;

    Task<TResponseMessage> SendAsync<TRequestMessage, TResponseMessage>(
        TRequestMessage message,
        int nodeId,
        CancellationToken token)
        where TResponseMessage : class, IResponseMessage
        where TRequestMessage : class, IRequestMessage;

    IKafkaConnector ProvideDedicatedConnector(int nodeId);
}
```

Смысл этих сигнатур:

- `SendAsync(message, token)`:
  cluster-level discovery/service path
- `SendAsync(message, nodeId, token)`:
  broker-targeted shared request path
- `ProvideDedicatedConnector(nodeId)`:
  выдача dedicated connector для long-lived affinity-sensitive workflows

### Internal helper methods in `KafkaCluster`

Чтобы cluster state decision не размазывался по call sites, рекомендуется в `KafkaCluster` ввести набор явных private helpers.

Минимальный набор:

```csharp
private bool HasUsableBrokerTopology { get; }

private bool HasKnownController { get; }

private IKafkaConnector GetConnectorForServiceRequests(bool controllerRequired = false);

private IKafkaConnector GetConnectorForBootstrapOrAnyBroker();

private IKafkaConnector GetConnectorForKnownBroker(int nodeId);
```

Рекомендуемая семантика:

- `HasUsableBrokerTopology`:
  cluster уже может работать в metadata-driven routing mode
- `HasKnownController`:
  controller routing может выполняться без bootstrap fallback
- `GetConnectorForServiceRequests(bool controllerRequired = false)`:
  общий entrypoint для service/admin/discovery path
- `GetConnectorForBootstrapOrAnyBroker()`:
  явный helper для switch между bootstrap phase и any-shared-broker phase
- `GetConnectorForKnownBroker(int nodeId)`:
  central place for broker-targeted shared connector acquisition

### Recommended implementation shape for `KafkaCluster`

Пример целевой формы helper methods:

```csharp
private bool HasUsableBrokerTopology => _nodes.Count > 0 && Brokers.Count > 0;

private bool HasKnownController => _controllerId != Node.NoNode.Id;

private IKafkaConnector GetConnectorForBootstrapOrAnyBroker()
{
    if (!HasUsableBrokerTopology)
    {
        if (_connectorPool.TryGetBootstrapConnector(out var bootstrapConnector))
        {
            return bootstrapConnector;
        }

        throw new ConnectorNotFoundException("No bootstrap connector is available.");
    }

    if (_connectorPool.TryGetAnySharedBrokerConnector(out var brokerConnector))
    {
        return brokerConnector;
    }

    throw new ConnectorNotFoundException("No shared broker connector is available.");
}

private IKafkaConnector GetConnectorForKnownBroker(int nodeId)
{
    if (_connectorPool.TryGetSharedConnector(nodeId, out var connector))
    {
        return connector;
    }

    throw new ConnectorNotFoundException($"Connector for broker {nodeId} not found.");
}

private IKafkaConnector GetConnectorForServiceRequests(bool controllerRequired = false)
{
    if (controllerRequired)
    {
        if (!HasKnownController)
        {
            throw new ClusterKafkaException(ExceptionMessages.NoController);
        }

        return GetConnectorForKnownBroker(_controllerId);
    }

    if (HasKnownController && _connectorPool.TryGetSharedConnector(_controllerId, out var controllerConnector))
    {
        return controllerConnector;
    }

    return GetConnectorForBootstrapOrAnyBroker();
}
```

### Naming recommendations

Чтобы не тащить дальше двусмысленность, рекомендуется зафиксировать следующие имена:

- `ProvideDedicatedConnector`, а не `ProvideDedicateConnector`
- `GetOpenedSharedConnectors`, а не `GetAllOpenedConnectors`
- `TryCreateDedicatedConnector`, а не `TryGetConnector(nodeId, true, ...)`
- `TryGetAnySharedBrokerConnector`, а не `GetConnector`

### Compatibility notes for the migration

Чтобы переход был безопасным, лучше внедрять сигнатуры так:

1. добавить новые методы рядом со старыми
2. перевести `KafkaCluster` на новые explicit methods
3. обновить tests и call sites
4. удалить старые ambiguous methods

На этом этапе не требуется:

- менять `IKafkaConnector.SendAsync(...)`
- менять public API producer/consumer/admin
- вводить новый routing object, если explicit methods уже покрывают сценарии

## Implementation sequence for the explicit contract

Ниже зафиксирована рекомендуемая последовательность внедрения explicit routing contract.

### Step 1. Spec and contract baseline

Цель:

- зафиксировать целевые сигнатуры и routing semantics до изменения кода

Что должно быть сделано:

- explicit pool contract записан в этой спеке
- routing table для верхнего уровня записана в этой спеке
- API sketch для `IKafkaConnectorPool` и `IKafkaCluster` записан в этой спеке
- naming decisions зафиксированы: `ProvideDedicatedConnector`, `TryGetAnySharedBrokerConnector`, `GetOpenedSharedConnectors`

Критерий завершения:

- дальнейшее внедрение может ссылаться на эту спецификацию как на baseline
- новые сигнатуры и routing semantics больше не обсуждаются как неформальная идея

### Step 2. Add new connector-pool methods without removing old ones

Цель:

- ввести новые explicit methods в кодовую базу без резкого перехода

Что должно быть сделано:

- новые методы добавлены в `IKafkaConnectorPool`
- `KafkaConnectorPool` реализует их поверх текущей логики
- старые `GetConnector()` и `TryGetConnector(nodeId, isDedicated, ...)` пока остаются

Критерий завершения:

- код компилируется
- новые методы доступны для поэтапного перевода call sites

Статус:

- `Done`

Примечание по реализации:

- новые explicit methods уже добавлены в `IKafkaConnectorPool`
- `KafkaConnectorPool` реализует их как thin wrappers над текущей логикой
- старые ambiguous methods пока сохранены для безопасной миграции

### Step 3. Migrate `KafkaCluster` to explicit routing methods

Цель:

- убрать из `KafkaCluster` зависимость от ambiguous pool methods

Что должно быть сделано:

- `SendAsync(..., nodeId, ...)` использует `TryGetSharedConnector`
- service routing uses explicit controller/any-broker/bootstrap path
- `MergeAllVersions()` uses `GetOpenedSharedConnectors()`
- dedicated acquisition migrates to `TryCreateDedicatedConnector`

Критерий завершения:

- cluster layer больше не зависит от `GetConnector()`
- routing intent читается из кода явно

Статус:

- `Done`

Примечание по реализации:

- broker-targeted routing переведён на `TryGetSharedConnector`
- service routing now uses explicit `controller -> any shared broker -> bootstrap` path
- `MergeAllVersions()` uses `GetOpenedSharedConnectors()`
- dedicated acquisition in `KafkaCluster` migrated to `TryCreateDedicatedConnector`

### Step 4. Rename cluster dedicated-connector API

Цель:

- устранить naming inconsistency в cluster contract

Что должно быть сделано:

- `ProvideDedicateConnector` renamed to `ProvideDedicatedConnector`
- `IKafkaCluster`, `KafkaCluster` and consumer call sites updated

Критерий завершения:

- dedicated connector API naming согласовано в cluster layer

Статус:

- `Done`

Примечание по реализации:

- `ProvideDedicateConnector` renamed to `ProvideDedicatedConnector`
- interface, implementation and consumer call site already updated

### Step 5. Update tests

Цель:

- перевести unit tests на новые explicit methods без изменения смысловой проверки

Что должно быть сделано:

- updated tests for cluster routing
- updated tests for connector-pool selection
- updated client tests using dedicated/shared acquisition paths

Критерий завершения:

- test suite использует новые названия и методы

Статус:

- `Done`

Примечание по реализации:

- unit tests for cluster, client and connector-pool routing migrated to explicit methods
- legacy test call sites for `GetConnector()`, `TryGetConnector(...)`, `GetAllOpenedConnectors()` and `ProvideDedicateConnector` removed
- verification remains partially limited by an existing `KafkaCluster.DisposeAsync()` failure in two cluster tests

### Step 6. Remove old ambiguous methods

Цель:

- завершить миграцию и убрать старые неявные контракты

Что должно быть сделано:

- removed `IKafkaConnectorPool.GetConnector()`
- removed `IKafkaConnectorPool.TryGetConnector(int nodeId, bool isDedicated, ...)`
- removed old implementation paths in `KafkaConnectorPool`

Критерий завершения:

- в кодовой базе остаётся только explicit routing contract

Статус:

- `Done`

Примечание по реализации:

- old pool methods `GetConnector()`, `TryGetConnector(...)` and `GetAllOpenedConnectors()` removed from interface and implementation
- only explicit routing methods remain in `IKafkaConnectorPool`

### Step 7. Start cluster helper cleanup

Цель:

- после стабилизации сигнатур подготовить чистую routing-state model в `KafkaCluster`

Что должно быть сделано:

- add `HasUsableBrokerTopology`
- add `HasKnownController`
- centralize bootstrap/any-broker/known-broker routing in private helpers

Критерий завершения:

- `KafkaCluster` routing logic больше не размазана по нескольким местам

Статус:

- `Done`

Примечание по реализации:

- `KafkaCluster` now has explicit routing-state helpers `HasUsableBrokerTopology` and `HasKnownController`
- broker-targeted acquisition centralized in `GetConnectorForKnownBroker(int nodeId)`
- bootstrap vs any-broker fallback centralized in `GetConnectorForBootstrapOrAnyBroker()`
- `GetConnectorForServiceRequests()` now composes these helpers instead of carrying all routing branches inline

## Recommended immediate next step

После принятия этого плана следующий шаг должен быть таким:

1. превратить вопросы Stage 1 в согласованную матрицу решений
2. пройтись по [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs), [src/NKafka/Connection/KafkaConnectorPool.cs](K:\nkafka\src\NKafka\Connection\KafkaConnectorPool.cs) и [src/NKafka/Connection/KafkaConnector.cs](K:\nkafka\src\NKafka\Connection\KafkaConnector.cs)
3. отметить конкретные boundary violations и кандидатные точки первых минимальных правок

## Completion criteria for this plan document

Сам документ можно считать достаточным стартом, если:

- он покрывает путь от верхнеуровневых boundaries до тестового закрепления
- этапы идут в безопасном порядке
- отдельным блоком выделены вопросы, которые нужно закрыть до глубокого рефакторинга
- дальнейшая работа может ссылаться на него как на execution plan, а не только как на intent note
