# Спецификация cluster-first клиентской архитектуры NKafka

## Назначение документа

Этот документ фиксирует целевую клиентскую архитектуру `NKafka`, в которой центральной точкой входа является `Cluster`, а не отдельные `Producer`,
`Consumer` или `Admin` клиенты.

Документ нужен, чтобы:

- зафиксировать, почему `NKafka` строится от понятия cluster runtime
- определить boundaries между `Cluster` и role-specific clients
- описать, какой state должен быть shared, а какой role-local
- задать правила для shared и dedicated connections
- уменьшить дублирование metadata, routing и connection state между client roles

Этот документ следует читать вместе с:

- [Base spec](K:\nkafka\spec\index.md)
- [Technical requirements](K:\nkafka\spec\technical-requirements.md)
- [Protocol interaction spec](K:\nkafka\spec\protocol-interaction.md)
- [Network interaction spec](K:\nkafka\spec\network-interaction.md)
- [Code map](K:\nkafka\spec\code-map.md)
- [Code gaps](K:\nkafka\spec\code-gaps.md)

## Scope

В scope этого документа входят:

- верхнеуровневая client architecture
- роль `Cluster` как shared runtime context
- responsibilities `Producer`, `Consumer` и `Admin`
- state ownership model
- shared vs dedicated connection policy на уровне клиентской архитектуры
- configuration model для cluster-wide и role-specific settings
- lifecycle rules для cluster и дочерних clients

Не входят в scope:

- wire protocol details
- low-level transport implementation
- broker-side semantics
- детальная реализация transactional, rebalance или admin workflows
- message generation и protocol contracts как таковые

## Почему нужна cluster-first модель

Классическая модель Kafka client libraries обычно строится вокруг отдельных client roles:

- producer
- consumer
- admin client

На практике у этих ролей есть большой общий инфраструктурный слой:

- bootstrap servers и broker discovery
- metadata по cluster, topics и partitions
- leader lookup
- negotiated API versions
- security configuration и session setup
- shared routing decisions
- observability и diagnostics context

Если каждая роль владеет своим полным стеком независимо, это ведёт к повторению:

- физических соединений
- metadata state
- broker capability state
- background refresh logic
- connection/auth setup

Для `NKafka` более естественна модель, где этот общий слой принадлежит `Cluster`, а `Producer`, `Consumer` и `Admin` используют его как shared runtime
foundation.

## Основной принцип

`Cluster` является главным runtime-контекстом Kafka-клиента.

`Producer`, `Consumer` и `Admin` рассматриваются как role-specific façades над общими cluster services, а не как полностью независимые клиенты с
собственным дублирующимся инфраструктурным стеком.

Из этого следуют базовые правила:

- cluster-wide state должен жить на уровне `Cluster`
- role-specific workflow state должен жить в соответствующем client facade
- общие соединения и metadata не должны дублироваться без явной причины
- отдельные соединения создаются только там, где это оправдано runtime semantics

## Sources of truth

Для client-architecture решений приоритет источников такой:

1. [AGENTS.md](K:\nkafka\AGENTS.md)
2. [spec/index.md](K:\nkafka\spec\index.md)
3. [spec/technical-requirements.md](K:\nkafka\spec\technical-requirements.md)
4. Этот документ
5. [spec/network-interaction.md](K:\nkafka\spec\network-interaction.md)
6. [spec/protocol-interaction.md](K:\nkafka\spec\protocol-interaction.md)
7. Код и тесты

Если код временно расходится с этой моделью, код рассматривается как текущее состояние, а этот документ как target architecture для дальнейшего
выравнивания.

## Layer model

### Layer 1. Cluster runtime

Основной код:

- [src/NKafka/KafkaCluster.cs](K:\nkafka\src\NKafka\KafkaCluster.cs)
- [src/NKafka/IKafkaCluster.cs](K:\nkafka\src\NKafka\IKafkaCluster.cs)

Отвечает за:

- bootstrap configuration
- cluster metadata и broker topology
- cluster-wide API capability knowledge
- routing-level decisions
- connector pool ownership
- shared runtime lifecycle
- factory methods для client roles

### Layer 2. Runtime services

Основной код:

- [src/NKafka/Connection](K:\nkafka\src\NKafka\Connection)

Отвечает за:

- shared и dedicated connectors
- connection establishment
- request routing primitives
- connection-scoped support versions
- transport/session execution

### Layer 3. Role-specific clients

Основной код:

- [src/NKafka/Clients/Producer](K:\nkafka\src\NKafka\Clients\Producer)
- [src/NKafka/Clients/Consumer](K:\nkafka\src\NKafka\Clients\Consumer)
- [src/NKafka/Clients/Admin](K:\nkafka\src\NKafka\Clients\Admin)

Отвечает за:

- producer workflow semantics
- consumer workflow semantics
- admin workflow semantics

Role-specific clients не должны повторно владеть cluster-wide networking и metadata state.

## Responsibilities

### `Cluster`

`Cluster` должен:

- хранить базовую конфигурацию подключения
- владеть connector pool и общим runtime lifecycle
- управлять metadata refresh и topology knowledge
- агрегировать broker capabilities
- выбирать, когда использовать shared connector, а когда dedicated
- создавать `Producer`, `Consumer` и `Admin` поверх общего контекста

`Cluster` не должен:

- содержать детальную producer batching logic
- содержать consumer rebalance state machine
- содержать admin request mapping details
- превращаться в god-object, который реализует все client workflows напрямую

### `Producer`

`Producer` должен:

- управлять produce pipeline
- владеть batching, partitioning и delivery behavior
- хранить transactional/idempotent state, если он есть
- использовать shared cluster services для metadata и routing

`Producer` не должен:

- владеть своим отдельным cluster metadata cache по умолчанию
- открывать собственный полностью независимый набор service connections без необходимости

### `Consumer`

`Consumer` должен:

- управлять subscription model
- владеть fetch, group coordination и assignment lifecycle
- хранить consumer-local session state
- использовать cluster metadata и runtime services без дублирования cluster-wide knowledge

`Consumer` не должен:

- повторно реализовывать cluster discovery
- владеть своей отдельной глобальной картиной broker topology

### `Admin`

`Admin` должен:

- предоставлять orchestration над admin APIs
- использовать существующий cluster runtime и metadata knowledge
- переиспользовать shared service connections там, где это допустимо

`Admin` не должен:

- поднимать отдельный cluster stack только ради admin request path
- дублировать metadata refresh model

## State ownership model

### Cluster-wide state

На уровне `Cluster` должен жить следующий state:

- bootstrap servers
- broker registry
- controller knowledge
- topic metadata
- partition metadata
- topic-to-id и id-to-topic mappings
- aggregated API compatibility knowledge
- shared diagnostics context
- shared connection registry

### Role-local state

На уровне role-specific clients должен жить следующий state:

- producer accumulator/batches/delivery tasks
- producer transaction state
- consumer subscription
- consumer assignment state
- consumer offset tracking
- consumer coordinator session state
- admin operation-local request/response orchestration state

### Rule of ownership

Если state отвечает на вопрос "что знает процесс о cluster в целом", он должен принадлежать `Cluster`.

Если state отвечает на вопрос "в каком workflow-состоянии находится конкретный producer/consumer/admin instance", он должен принадлежать
соответствующему role-specific client.

## Connection policy

### Shared connections

Shared connections подходят для:

- bootstrap and metadata requests
- admin requests
- producer service traffic, если нет причины изолировать его
- other non-session-affine service requests

### Dedicated connections

Dedicated connections подходят для:

- long-lived consumer fetch traffic
- coordinator-sensitive consumer sessions
- workflows, где connection affinity влияет на correctness или isolation
- future role-specific high-sensitivity paths, если это подтвердят runtime requirements

### Selection rule

Политика соединений должна определяться не названием client role, а operational semantics конкретного path.

Это означает:

- не каждый consumer request обязан идти по dedicated connection
- не каждый producer request обязан идти по shared connection
- решение принимается по требованиям к affinity, isolation, latency и correctness

## Lifecycle model

### Cluster lifecycle

`Cluster` должен:

1. Инициализировать shared runtime
2. Открывать bootstrap connectivity
3. Получать metadata и broker capabilities
4. Поддерживать shared state в актуальном виде
5. Создавать role-specific clients без повторной инициализации всего стека
6. Централизованно завершать shared runtime

### Role client lifecycle

`Producer`, `Consumer` и `Admin` должны:

- создаваться поверх уже существующего cluster context
- владеть только своим локальным workflow lifecycle
- освобождать собственные локальные ресурсы при dispose/close
- не уничтожать shared cluster runtime только потому, что завершился один role-specific client

### Ownership rule for dispose

Dispose role-specific client должен:

- завершить local workflow resources
- освободить dedicated resources, если они принадлежат только ему
- уведомить cluster о завершении локального instance при необходимости

Dispose role-specific client не должен:

- закрывать весь `Cluster`
- разрушать shared connectors, которые могут использовать другие clients
- инвалидировать cluster-wide metadata без явной причины

## Configuration model

### ClusterConfig

`ClusterConfig` должен задавать общий baseline для:

- bootstrap servers
- security settings
- request timeout defaults
- metadata policy
- shared transport/runtime options
- client-wide observability defaults

### Role-specific config

`ProducerConfig`, `ConsumerConfig` и admin-specific options должны задавать только те настройки, которые действительно относятся к их workflow
semantics.

Они могут:

- переопределять часть cluster defaults
- добавлять role-local behavior settings

Они не должны:

- дублировать весь cluster transport stack как отдельную независимую сущность
- требовать повторной полной инициализации cluster runtime

## Public API implications

Из этой модели следуют правила для публичного API:

- основной entry point библиотеки должен быть cluster-oriented
- создание producer/consumer/admin должно происходить через cluster context
- public API не должен создавать впечатление, что каждый role client является полностью независимым mini-cluster
- cluster-wide operations и knowledge должны быть доступны через cluster abstraction

При этом:

- role-specific APIs должны оставаться удобными для .NET-разработчика
- cluster-first model не должна заставлять пользователя вручную управлять transport internals

## Design constraints

Cluster-first architecture должна сохранять:

- predictable resource usage
- minimal unnecessary connections
- separation of responsibilities
- AOT-friendly design
- testability
- explicit lifecycle ownership

Особое ограничение:

cluster-first model не должна превращаться в универсальный объект, который трудно развивать и тестировать.

Поэтому любые новые responsibilities должны сначала классифицироваться как:

- cluster-wide infrastructure responsibility
- runtime service responsibility
- role-specific workflow responsibility

## Non-goals

Эта архитектура не означает:

- что все соединения обязаны стать shared
- что consumer-specific isolation больше не нужна
- что producer, consumer и admin должны потерять свои отдельные API
- что `Cluster` должен вобрать в себя все бизнес-методы library surface

Цель не в устранении client roles, а в устранении лишнего дублирования общего runtime слоя.

## Architecture review checklist

При добавлении новой возможности нужно проверить:

1. Это cluster-wide state или role-local state?
2. Это shared runtime concern или role-specific workflow concern?
3. Нужен ли dedicated connection по runtime semantics, или достаточно shared path?
4. Не дублируется ли metadata/routing/auth state без необходимости?
5. Не начинает ли `Cluster` брать на себя business logic конкретного client role?

## Completion criteria

Client architecture можно считать приведённой к этой модели, когда:

- cluster-wide state действительно централизован на уровне `Cluster`
- role-specific clients не дублируют общий runtime stack без явной причины
- dispose semantics отражают ownership boundaries
- shared/dedicated connection policy определяется runtime requirements
- public API отражает cluster-first model без misleading independence between roles

## Related evolution directions

Эта спецификация естественно связана с:

- уточнением connection ownership в network layer
- доведением public API до честного отражения runtime readiness
- выравниванием lifecycle semantics между cluster и child clients
- последующим выделением feature-ready и placeholder client surfaces
