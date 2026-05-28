# Client-side Kafka KIPs for NKafka

## Scope

This list includes only KIPs that affect implementation of a Kafka client library:

- wire protocol and message schema
- producer behavior
- consumer and group management behavior
- admin client behavior
- client-side security and authentication

This list intentionally excludes:

- broker-only KIPs
- KRaft/controller internals without client impact
- Kafka Connect-only KIPs
- Kafka Streams-only KIPs
- storage and replication internals that do not change client behavior or protocol usage

Additional filter for this document:

- exclude KIPs that are only relevant to pre-2.0 Kafka protocol eras
- keep only KIPs that were introduced, became relevant, or remain actionable for Kafka `>= 2.0.0`

## Modern client-side KIPs for Kafka >= 2.0

### Core protocol and serialization

- [KIP-110: Add Codec for ZStandard Compression](https://cwiki.apache.org/confluence/display/KAFKA/KIP-110%3A%2BAdd%2BCodec%2Bfor%2BZStandard%2BCompression)
- [KIP-482: The Kafka Protocol should Support Optional Tagged Fields](https://cwiki.apache.org/confluence/display/KAFKA/KIP-482%3A%2BThe%2BKafka%2BProtocol%2Bshould%2BSupport%2BOptional%2BTagged%2BFields)
- [KIP-511: Collect and Expose Client's Name and Version in the Brokers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-511%3A%2BCollect%2Band%2BExpose%2BClient%27s%2BName%2Band%2BVersion%2Bin%2Bthe%2BBrokers)
- [KIP-516: Topic Identifiers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-516%3A%2BTopic%2BIdentifiers)
- [KIP-714: Client metrics and observability](https://cwiki.apache.org/confluence/display/KAFKA/KIP-714%3A%2BClient%2Bmetrics%2Band%2Bobservability)
- [KIP-899: Allow producer and consumer clients to rebootstrap](https://cwiki.apache.org/confluence/display/KAFKA/KIP-899%3A%2BAllow%2Bproducer%2Band%2Bconsumer%2Bclients%2Bto%2Brebootstrap)
- [KIP-951: Leader discovery optimizations for the client](https://cwiki.apache.org/confluence/display/KAFKA/KIP-951%3A%2BLeader%2Bdiscovery%2Boptimizations%2Bfor%2Bthe%2Bclient)
- [KIP-1102: Enable clients to rebootstrap based on timeout or error code](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1102%3A%2BEnable%2Bclients%2Bto%2Brebootstrap%2Bbased%2Bon%2Btimeout%2Bor%2Berror%2Bcode)

### Consumer and group management

- [KIP-320: Allow fetchers to detect and handle log truncation](https://cwiki.apache.org/confluence/display/KAFKA/KIP-320%3A%2BAllow%2Bfetchers%2Bto%2Bdetect%2Band%2Bhandle%2Blog%2Btruncation)
- [KIP-392: Allow consumers to fetch from closest replica](https://cwiki.apache.org/confluence/display/KAFKA/KIP-392%3A%2BAllow%2Bconsumers%2Bto%2Bfetch%2Bfrom%2Bclosest%2Breplica)
- [KIP-429: Kafka Consumer Incremental Rebalance Protocol](https://cwiki.apache.org/confluence/display/KAFKA/KIP-429%3A%2BKafka%2BConsumer%2BIncremental%2BRebalance%2BProtocol)
- [KIP-699: Update FindCoordinator to resolve multiple Coordinators at a time](https://cwiki.apache.org/confluence/display/KAFKA/KIP-699%3A%2BUpdate%2BFindCoordinator%2Bto%2Bresolve%2Bmultiple%2BCoordinators%2Bat%2Ba%2Btime)
- [KIP-848: The Next Generation of the Consumer Rebalance Protocol](https://cwiki.apache.org/confluence/x/HhD1D)
- [KIP-932: Queues for Kafka](https://cwiki.apache.org/confluence/display/KAFKA/KIP-932%3A%2BQueues%2Bfor%2BKafka)
- [KIP-1043: Administration of groups](https://cwiki.apache.org/confluence/x/XoowEg)
- [KIP-1082: Require Client-Generated IDs over the ConsumerGroupHeartbeat RPC](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1082%3A%2BRequire%2BClient-Generated%2BIDs%2Bover%2Bthe%2BConsumerGroupHeartbeat%2BRPC)
- [KIP-1274: Deprecate and remove support for Classic rebalance protocol in KafkaConsumer](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1274%3A%2BDeprecate%2Band%2Bremove%2Bsupport%2Bfor%2BClassic%2Brebalance%2Bprotocol%2Bin%2BKafkaConsumer)

### Producer and transactions

- [KIP-359: Verify leader epoch in produce requests](https://cwiki.apache.org/confluence/display/KAFKA/KIP-359%3A%2BVerify%2Bleader%2Bepoch%2Bin%2Bproduce%2Brequests)
- [KIP-890: Transactions Server-Side Defense](https://cwiki.apache.org/confluence/display/KAFKA/KIP-890%3A%2BTransactions%2BServer-Side%2BDefense)

### Admin client

- [KIP-117: Add a public AdminClient API for Kafka admin operations](https://cwiki.apache.org/confluence/display/KAFKA/KIP-117%3A%2BAdd%2Ba%2Bpublic%2BAdminClient%2BAPI%2Bfor%2BKafka%2Badmin%2Boperations)
- [KIP-140: Add administrative RPCs for adding, deleting, and listing ACLs](https://cwiki.apache.org/confluence/display/KAFKA/KIP-140%3A%2BAdd%2Badministrative%2BRPCs%2Bfor%2Badding%2C%2Bdeleting%2C%2Band%2Blisting%2BACLs)
- [KIP-396: Add Reset/List Offsets Operations to AdminClient](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=97551484&src=contextnavchildmode)
- [KIP-700: Add Describe Cluster API](https://cwiki.apache.org/confluence/display/KAFKA/KIP-700%3A%2BAdd%2BDescribe%2BCluster%2BAPI)

### Security and authentication

- [KIP-255: OAuth Authentication via SASL/OAUTHBEARER](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=75968876)
- [KIP-368: Allow SASL Connections to Periodically Re-Authenticate](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=89068981)
- [KIP-519: Make SSL context/engine configuration extensible](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=128650952)
- [KIP-651: Support PEM format for SSL certificates and private key](https://cwiki.apache.org/confluence/display/KAFKA/KIP-651%2B-%2BSupport%2BPEM%2Bformat%2Bfor%2BSSL%2Bcertificates%2Band%2Bprivate%2Bkey)
- [KIP-768: Extend SASL/OAUTHBEARER with Support for OIDC](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=186877575)
- [KIP-1139: Add support for OAuth jwt-bearer grant type](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1139%3A%2BAdd%2Bsupport%2Bfor%2BOAuth%2Bjwt-bearer%2Bgrant%2Btype)

## Priority for NKafka

### P0: mandatory for a modern general-purpose client

- [KIP-110: Add Codec for ZStandard Compression](https://cwiki.apache.org/confluence/display/KAFKA/KIP-110%3A%2BAdd%2BCodec%2Bfor%2BZStandard%2BCompression)
- [KIP-117: Add a public AdminClient API for Kafka admin operations](https://cwiki.apache.org/confluence/display/KAFKA/KIP-117%3A%2BAdd%2Ba%2Bpublic%2BAdminClient%2BAPI%2Bfor%2BKafka%2Badmin%2Boperations)
- [KIP-482: The Kafka Protocol should Support Optional Tagged Fields](https://cwiki.apache.org/confluence/display/KAFKA/KIP-482%3A%2BThe%2BKafka%2BProtocol%2Bshould%2BSupport%2BOptional%2BTagged%2BFields)
- [KIP-511: Collect and Expose Client's Name and Version in the Brokers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-511%3A%2BCollect%2Band%2BExpose%2BClient%27s%2BName%2Band%2BVersion%2Bin%2Bthe%2BBrokers)
- [KIP-516: Topic Identifiers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-516%3A%2BTopic%2BIdentifiers)
- [KIP-429: Kafka Consumer Incremental Rebalance Protocol](https://cwiki.apache.org/confluence/display/KAFKA/KIP-429%3A%2BKafka%2BConsumer%2BIncremental%2BRebalance%2BProtocol)
- [KIP-699: Update FindCoordinator to resolve multiple Coordinators at a time](https://cwiki.apache.org/confluence/display/KAFKA/KIP-699%3A%2BUpdate%2BFindCoordinator%2Bto%2Bresolve%2Bmultiple%2BCoordinators%2Bat%2Ba%2Btime)
- [KIP-848: The Next Generation of the Consumer Rebalance Protocol](https://cwiki.apache.org/confluence/x/HhD1D)
- [KIP-899: Allow producer and consumer clients to rebootstrap](https://cwiki.apache.org/confluence/display/KAFKA/KIP-899%3A%2BAllow%2Bproducer%2Band%2Bconsumer%2Bclients%2Bto%2Brebootstrap)
- [KIP-951: Leader discovery optimizations for the client](https://cwiki.apache.org/confluence/display/KAFKA/KIP-951%3A%2BLeader%2Bdiscovery%2Boptimizations%2Bfor%2Bthe%2Bclient)
- [KIP-1102: Enable clients to rebootstrap based on timeout or error code](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1102%3A%2BEnable%2Bclients%2Bto%2Brebootstrap%2Bbased%2Bon%2Btimeout%2Bor%2Berror%2Bcode)

### P1: important depending on feature coverage

- [KIP-140: Add administrative RPCs for adding, deleting, and listing ACLs](https://cwiki.apache.org/confluence/display/KAFKA/KIP-140%3A%2BAdd%2Badministrative%2BRPCs%2Bfor%2Badding%2C%2Bdeleting%2C%2Band%2Blisting%2BACLs)
- [KIP-255: OAuth Authentication via SASL/OAUTHBEARER](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=75968876)
- [KIP-320: Allow fetchers to detect and handle log truncation](https://cwiki.apache.org/confluence/display/KAFKA/KIP-320%3A%2BAllow%2Bfetchers%2Bto%2Bdetect%2Band%2Bhandle%2Blog%2Btruncation)
- [KIP-359: Verify leader epoch in produce requests](https://cwiki.apache.org/confluence/display/KAFKA/KIP-359%3A%2BVerify%2Bleader%2Bepoch%2Bin%2Bproduce%2Brequests)
- [KIP-368: Allow SASL Connections to Periodically Re-Authenticate](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=89068981)
- [KIP-392: Allow consumers to fetch from closest replica](https://cwiki.apache.org/confluence/display/KAFKA/KIP-392%3A%2BAllow%2Bconsumers%2Bto%2Bfetch%2Bfrom%2Bclosest%2Breplica)
- [KIP-396: Add Reset/List Offsets Operations to AdminClient](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=97551484&src=contextnavchildmode)
- [KIP-519: Make SSL context/engine configuration extensible](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=128650952)
- [KIP-651: Support PEM format for SSL certificates and private key](https://cwiki.apache.org/confluence/display/KAFKA/KIP-651%2B-%2BSupport%2BPEM%2Bformat%2Bfor%2BSSL%2Bcertificates%2Band%2Bprivate%2Bkey)
- [KIP-700: Add Describe Cluster API](https://cwiki.apache.org/confluence/display/KAFKA/KIP-700%3A%2BAdd%2BDescribe%2BCluster%2BAPI)
- [KIP-714: Client metrics and observability](https://cwiki.apache.org/confluence/display/KAFKA/KIP-714%3A%2BClient%2Bmetrics%2Band%2Bobservability)
- [KIP-768: Extend SASL/OAUTHBEARER with Support for OIDC](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=186877575)
- [KIP-890: Transactions Server-Side Defense](https://cwiki.apache.org/confluence/display/KAFKA/KIP-890%3A%2BTransactions%2BServer-Side%2BDefense)
- [KIP-1043: Administration of groups](https://cwiki.apache.org/confluence/x/XoowEg)
- [KIP-1082: Require Client-Generated IDs over the ConsumerGroupHeartbeat RPC](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1082%3A%2BRequire%2BClient-Generated%2BIDs%2Bover%2Bthe%2BConsumerGroupHeartbeat%2BRPC)
- [KIP-1139: Add support for OAuth jwt-bearer grant type](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1139%3A%2BAdd%2Bsupport%2Bfor%2BOAuth%2Bjwt-bearer%2Bgrant%2Btype)

### P2: optional / roadmap features

- [KIP-932: Queues for Kafka](https://cwiki.apache.org/confluence/display/KAFKA/KIP-932%3A%2BQueues%2Bfor%2BKafka)
- [KIP-1274: Deprecate and remove support for Classic rebalance protocol in KafkaConsumer](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1274%3A%2BDeprecate%2Band%2Bremove%2Bsupport%2Bfor%2BClassic%2Brebalance%2Bprotocol%2Bin%2BKafkaConsumer)

## Notes for current NKafka codebase

The current repository already directly references or is affected by these KIPs in message specs or code:

- [KIP-110: Add Codec for ZStandard Compression](https://cwiki.apache.org/confluence/display/KAFKA/KIP-110%3A%2BAdd%2BCodec%2Bfor%2BZStandard%2BCompression)
- [KIP-320: Allow fetchers to detect and handle log truncation](https://cwiki.apache.org/confluence/display/KAFKA/KIP-320%3A%2BAllow%2Bfetchers%2Bto%2Bdetect%2Band%2Bhandle%2Blog%2Btruncation)
- [KIP-511: Collect and Expose Client's Name and Version in the Brokers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-511%3A%2BCollect%2Band%2BExpose%2BClient%27s%2BName%2Band%2BVersion%2Bin%2Bthe%2BBrokers)
- [KIP-516: Topic Identifiers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-516%3A%2BTopic%2BIdentifiers)
- [KIP-699: Update FindCoordinator to resolve multiple Coordinators at a time](https://cwiki.apache.org/confluence/display/KAFKA/KIP-699%3A%2BUpdate%2BFindCoordinator%2Bto%2Bresolve%2Bmultiple%2BCoordinators%2Bat%2Ba%2Btime)
- [KIP-700: Add Describe Cluster API](https://cwiki.apache.org/confluence/display/KAFKA/KIP-700%3A%2BAdd%2BDescribe%2BCluster%2BAPI)
- [KIP-848: The Next Generation of the Consumer Rebalance Protocol](https://cwiki.apache.org/confluence/x/HhD1D)
- [KIP-890: Transactions Server-Side Defense](https://cwiki.apache.org/confluence/display/KAFKA/KIP-890%3A%2BTransactions%2BServer-Side%2BDefense)
- [KIP-932: Queues for Kafka](https://cwiki.apache.org/confluence/display/KAFKA/KIP-932%3A%2BQueues%2Bfor%2BKafka)
- [KIP-951: Leader discovery optimizations for the client](https://cwiki.apache.org/confluence/display/KAFKA/KIP-951%3A%2BLeader%2Bdiscovery%2Boptimizations%2Bfor%2Bthe%2Bclient)
- [KIP-1043: Administration of groups](https://cwiki.apache.org/confluence/x/XoowEg)
- [KIP-1082: Require Client-Generated IDs over the ConsumerGroupHeartbeat RPC](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1082%3A%2BRequire%2BClient-Generated%2BIDs%2Bover%2Bthe%2BConsumerGroupHeartbeat%2BRPC)

## Cross-check with librdkafka

`librdkafka` is a useful comparison point because its official documentation maintains a client-facing KIP support matrix with statuses such as `Supported`, `Partially supported`, and `Not supported`.

Relevant source:

- [librdkafka INTRODUCTION](https://docs.confluent.io/platform/current/clients/librdkafka/html/md_INTRODUCTION.html)

The `librdkafka` matrix strongly confirms that the following KIPs are client-side concerns:

- [KIP-110: Add Codec for ZStandard Compression](https://cwiki.apache.org/confluence/display/KAFKA/KIP-110%3A%2BAdd%2BCodec%2Bfor%2BZStandard%2BCompression)
- [KIP-117: Add a public AdminClient API for Kafka admin operations](https://cwiki.apache.org/confluence/display/KAFKA/KIP-117%3A%2BAdd%2Ba%2Bpublic%2BAdminClient%2BAPI%2Bfor%2BKafka%2Badmin%2Boperations)
- [KIP-140: Add administrative RPCs for adding, deleting, and listing ACLs](https://cwiki.apache.org/confluence/display/KAFKA/KIP-140%3A%2BAdd%2Badministrative%2BRPCs%2Bfor%2Badding%2C%2Bdeleting%2C%2Band%2Blisting%2BACLs)
- [KIP-255: OAuth Authentication via SASL/OAUTHBEARER](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=75968876)
- [KIP-320: Allow fetchers to detect and handle log truncation](https://cwiki.apache.org/confluence/display/KAFKA/KIP-320%3A%2BAllow%2Bfetchers%2Bto%2Bdetect%2Band%2Bhandle%2Blog%2Btruncation)
- [KIP-359: Verify leader epoch in produce requests](https://cwiki.apache.org/confluence/display/KAFKA/KIP-359%3A%2BVerify%2Bleader%2Bepoch%2Bin%2Bproduce%2Brequests)
- [KIP-368: Allow SASL Connections to Periodically Re-Authenticate](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=89068981)
- [KIP-392: Allow consumers to fetch from closest replica](https://cwiki.apache.org/confluence/display/KAFKA/KIP-392%3A%2BAllow%2Bconsumers%2Bto%2Bfetch%2Bfrom%2Bclosest%2Breplica)
- [KIP-396: Add Reset/List Offsets Operations to AdminClient](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=97551484&src=contextnavchildmode)
- [KIP-429: Kafka Consumer Incremental Rebalance Protocol](https://cwiki.apache.org/confluence/display/KAFKA/KIP-429%3A%2BKafka%2BConsumer%2BIncremental%2BRebalance%2BProtocol)
- [KIP-482: The Kafka Protocol should Support Optional Tagged Fields](https://cwiki.apache.org/confluence/display/KAFKA/KIP-482%3A%2BThe%2BKafka%2BProtocol%2Bshould%2BSupport%2BOptional%2BTagged%2BFields)
- [KIP-511: Collect and Expose Client's Name and Version in the Brokers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-511%3A%2BCollect%2Band%2BExpose%2BClient%27s%2BName%2Band%2BVersion%2Bin%2Bthe%2BBrokers)
- [KIP-516: Topic Identifiers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-516%3A%2BTopic%2BIdentifiers)
- [KIP-519: Make SSL context/engine configuration extensible](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=128650952)
- [KIP-651: Support PEM format for SSL certificates and private key](https://cwiki.apache.org/confluence/display/KAFKA/KIP-651%2B-%2BSupport%2BPEM%2Bformat%2Bfor%2BSSL%2Bcertificates%2Band%2Bprivate%2Bkey)
- [KIP-714: Client metrics and observability](https://cwiki.apache.org/confluence/display/KAFKA/KIP-714%3A%2BClient%2Bmetrics%2Band%2Bobservability)
- [KIP-768: Extend SASL/OAUTHBEARER with Support for OIDC](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=186877575)
- [KIP-848: The Next Generation of the Consumer Rebalance Protocol](https://cwiki.apache.org/confluence/x/HhD1D)
- [KIP-899: Allow producer and consumer clients to rebootstrap](https://cwiki.apache.org/confluence/display/KAFKA/KIP-899%3A%2BAllow%2Bproducer%2Band%2Bconsumer%2Bclients%2Bto%2Brebootstrap)
- [KIP-951: Leader discovery optimizations for the client](https://cwiki.apache.org/confluence/display/KAFKA/KIP-951%3A%2BLeader%2Bdiscovery%2Boptimizations%2Bfor%2Bthe%2Bclient)
- [KIP-1082: Require Client-Generated IDs over the ConsumerGroupHeartbeat RPC](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1082%3A%2BRequire%2BClient-Generated%2BIDs%2Bover%2Bthe%2BConsumerGroupHeartbeat%2BRPC)
- [KIP-1102: Enable clients to rebootstrap based on timeout or error code](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1102%3A%2BEnable%2Bclients%2Bto%2Brebootstrap%2Bbased%2Bon%2Btimeout%2Bor%2Berror%2Bcode)
- [KIP-1139: Add support for OAuth jwt-bearer grant type](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1139%3A%2BAdd%2Bsupport%2Bfor%2BOAuth%2Bjwt-bearer%2Bgrant%2Btype)

Useful comparison notes from `librdkafka`:

- [KIP-848: The Next Generation of the Consumer Rebalance Protocol](https://cwiki.apache.org/confluence/x/HhD1D) is marked supported and production-ready in recent releases.
- [KIP-951: Leader discovery optimizations for the client](https://cwiki.apache.org/confluence/display/KAFKA/KIP-951%3A%2BLeader%2Bdiscovery%2Boptimizations%2Bfor%2Bthe%2Bclient) is marked supported.
- [KIP-511: Collect and Expose Client's Name and Version in the Brokers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-511%3A%2BCollect%2Band%2BExpose%2BClient%27s%2BName%2Band%2BVersion%2Bin%2Bthe%2BBrokers) is marked supported.
- [KIP-482: The Kafka Protocol should Support Optional Tagged Fields](https://cwiki.apache.org/confluence/display/KAFKA/KIP-482%3A%2BThe%2BKafka%2BProtocol%2Bshould%2BSupport%2BOptional%2BTagged%2BFields) is only partially supported there.
- [KIP-516: Topic Identifiers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-516%3A%2BTopic%2BIdentifiers) is only partially supported there.
- [KIP-359: Verify leader epoch in produce requests](https://cwiki.apache.org/confluence/display/KAFKA/KIP-359%3A%2BVerify%2Bleader%2Bepoch%2Bin%2Bproduce%2Brequests) is marked not supported there.

This is helpful for `NKafka` because it distinguishes:

- clearly client-owned KIPs
- KIPs that are protocol-visible but costly or uncommon to implement fully
- KIPs that mature client libraries still treat as partial or optional

## Explicitly filtered out as pre-2.0 era

These may still be historically relevant, but they should not drive the modern `NKafka` roadmap:

- [KIP-35: Retrieving protocol version](https://cwiki.apache.org/confluence/display/KAFKA/KIP-35%2B-%2BRetrieving%2Bprotocol%2Bversion)
- [KIP-74: Add Fetch Response Size Limit in Bytes](https://cwiki.apache.org/confluence/display/KAFKA/KIP-74%3A%2BAdd%2BFetch%2BResponse%2BSize%2BLimit%2Bin%2BBytes)
- [KIP-82: Add Record Headers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-82%2B-%2BAdd%2BRecord%2BHeaders)
- [KIP-84: Support SASL SCRAM mechanisms](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=67636971)
- [KIP-85: Dynamic JAAS configuration for Kafka clients](https://cwiki.apache.org/confluence/display/KAFKA/KIP-85%3A%2BDynamic%2BJAAS%2Bconfiguration%2Bfor%2BKafka%2Bclients)
- [KIP-86: Configurable SASL callback handlers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-86%3A%2BConfigurable%2BSASL%2Bcallback%2Bhandlers)

Reason:

- they were introduced before Kafka `2.0.0`
- they belong to the older baseline client/protocol evolution
- for a modern gap analysis they should be treated as legacy compatibility background rather than primary roadmap items

## Official references

- [Kafka Improvement Proposals index](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=293046882)
- [librdkafka INTRODUCTION](https://docs.confluent.io/platform/current/clients/librdkafka/html/md_INTRODUCTION.html)
