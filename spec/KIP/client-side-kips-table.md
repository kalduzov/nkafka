# Client-side Kafka KIPs Table for NKafka

## Scope

This table is a structured companion to `client-side-kips.md`.

Columns:

- `KIP`: official KIP link with title
- `Kafka since`: the Kafka release where the KIP became available or actionable for clients
- `Area`: protocol, consumer, producer, admin, or security
- `Priority`: suggested priority for `NKafka`
- `NKafka status`: rough current assessment based on repository references only
- `librdkafka`: useful comparison point from the official `librdkafka` support matrix when available

Status meanings:

- `Referenced`: the repository already references the KIP in code or message specs
- `Likely partial`: signs of protocol awareness exist, but full feature support is not verified
- `Unknown`: not enough local evidence yet
- `Roadmap`: relevant, but no clear local implementation evidence yet

## Modern client-side KIPs for Kafka >= 2.0

| KIP | Kafka since | Area | Priority | NKafka status | librdkafka |
|---|---|---|---|---|---|
| [KIP-110: Add Codec for ZStandard Compression](https://cwiki.apache.org/confluence/display/KAFKA/KIP-110%3A%2BAdd%2BCodec%2Bfor%2BZStandard%2BCompression) | 2.1.0 | Protocol | P0 | Referenced | Supported |
| [KIP-117: Add a public AdminClient API for Kafka admin operations](https://cwiki.apache.org/confluence/display/KAFKA/KIP-117%3A%2BAdd%2Ba%2Bpublic%2BAdminClient%2BAPI%2Bfor%2BKafka%2Badmin%2Boperations) | 0.11.0.0 | Admin | P0 | Referenced | Supported |
| [KIP-140: Add administrative RPCs for adding, deleting, and listing ACLs](https://cwiki.apache.org/confluence/display/KAFKA/KIP-140%3A%2BAdd%2Badministrative%2BRPCs%2Bfor%2Badding%2C%2Bdeleting%2C%2Band%2Blisting%2BACLs) | 0.11.0.0 | Admin | P1 | Unknown | Supported |
| [KIP-255: OAuth Authentication via SASL/OAUTHBEARER](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=75968876) | 2.0.0 | Security | P1 | Likely partial | Supported |
| [KIP-320: Allow fetchers to detect and handle log truncation](https://cwiki.apache.org/confluence/display/KAFKA/KIP-320%3A%2BAllow%2Bfetchers%2Bto%2Bdetect%2Band%2Bhandle%2Blog%2Btruncation) | 2.1.0 broker / 2.3.0 client | Consumer | P1 | Referenced | Supported |
| [KIP-359: Verify leader epoch in produce requests](https://cwiki.apache.org/confluence/display/KAFKA/KIP-359%3A%2BVerify%2Bleader%2Bepoch%2Bin%2Bproduce%2Brequests) | 2.8.0 (WIP in index) | Producer | P1 | Unknown | Not supported |
| [KIP-368: Allow SASL Connections to Periodically Re-Authenticate](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=89068981) | 2.2.0 | Security | P1 | Unknown | Supported |
| [KIP-392: Allow consumers to fetch from closest replica](https://cwiki.apache.org/confluence/display/KAFKA/KIP-392%3A%2BAllow%2Bconsumers%2Bto%2Bfetch%2Bfrom%2Bclosest%2Breplica) | 2.4.0 | Consumer | P1 | Unknown | Supported |
| [KIP-396: Add Reset/List Offsets Operations to AdminClient](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=97551484&src=contextnavchildmode) | 2.5.0 | Admin | P1 | Unknown | Supported |
| [KIP-429: Kafka Consumer Incremental Rebalance Protocol](https://cwiki.apache.org/confluence/display/KAFKA/KIP-429%3A%2BKafka%2BConsumer%2BIncremental%2BRebalance%2BProtocol) | 2.4.0 | Consumer | P0 | Roadmap | Supported |
| [KIP-482: The Kafka Protocol should Support Optional Tagged Fields](https://cwiki.apache.org/confluence/display/KAFKA/KIP-482%3A%2BThe%2BKafka%2BProtocol%2Bshould%2BSupport%2BOptional%2BTagged%2BFields) | 2.4.0 | Protocol | P0 | Likely partial | Partially supported |
| [KIP-511: Collect and Expose Client's Name and Version in the Brokers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-511%3A%2BCollect%2Band%2BExpose%2BClient%27s%2BName%2Band%2BVersion%2Bin%2Bthe%2BBrokers) | 2.4.0 protocol / 2.5.0 metric | Protocol | P0 | Referenced | Supported |
| [KIP-516: Topic Identifiers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-516%3A%2BTopic%2BIdentifiers) | 2.8.0 | Protocol | P0 | Referenced | Partially supported |
| [KIP-519: Make SSL context/engine configuration extensible](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=128650952) | 2.6.0 | Security | P1 | Unknown | Supported |
| [KIP-651: Support PEM format for SSL certificates and private key](https://cwiki.apache.org/confluence/display/KAFKA/KIP-651%2B-%2BSupport%2BPEM%2Bformat%2Bfor%2BSSL%2Bcertificates%2Band%2Bprivate%2Bkey) | 2.7.0 | Security | P1 | Unknown | Supported |
| [KIP-699: Update FindCoordinator to resolve multiple Coordinators at a time](https://cwiki.apache.org/confluence/display/KAFKA/KIP-699%3A%2BUpdate%2BFindCoordinator%2Bto%2Bresolve%2Bmultiple%2BCoordinators%2Bat%2Ba%2Btime) | 3.0.0 | Consumer/Admin | P0 | Referenced | Supported |
| [KIP-700: Add Describe Cluster API](https://cwiki.apache.org/confluence/display/KAFKA/KIP-700%3A%2BAdd%2BDescribe%2BCluster%2BAPI) | 2.8.0 | Admin | P1 | Referenced | Supported |
| [KIP-714: Client metrics and observability](https://cwiki.apache.org/confluence/display/KAFKA/KIP-714%3A%2BClient%2Bmetrics%2Band%2Bobservability) | 3.7.0 | Protocol/Telemetry | P1 | Unknown | Supported |
| [KIP-768: Extend SASL/OAUTHBEARER with Support for OIDC](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=186877575) | 3.1.0 | Security | P1 | Unknown | Supported |
| [KIP-848: The Next Generation of the Consumer Rebalance Protocol](https://cwiki.apache.org/confluence/x/HhD1D) | 3.7.0 EA / 4.0.0 GA | Consumer | P0 | Referenced | Supported |
| [KIP-890: Transactions Server-Side Defense](https://cwiki.apache.org/confluence/display/KAFKA/KIP-890%3A%2BTransactions%2BServer-Side%2BDefense) | 3.7.0 part 1 / 4.0.0 part 2 | Producer/Transactions | P1 | Referenced | n/a |
| [KIP-899: Allow producer and consumer clients to rebootstrap](https://cwiki.apache.org/confluence/display/KAFKA/KIP-899%3A%2BAllow%2Bproducer%2Band%2Bconsumer%2Bclients%2Bto%2Brebootstrap) | 3.8.0 | Protocol | P0 | Roadmap | Supported |
| [KIP-932: Queues for Kafka](https://cwiki.apache.org/confluence/display/KAFKA/KIP-932%3A%2BQueues%2Bfor%2BKafka) | 4.0.0 EA / 4.1.0 preview | Consumer | P2 | Referenced | n/a |
| [KIP-951: Leader discovery optimizations for the client](https://cwiki.apache.org/confluence/display/KAFKA/KIP-951%3A%2BLeader%2Bdiscovery%2Boptimizations%2Bfor%2Bthe%2Bclient) | 3.7.0 | Protocol | P0 | Referenced | Supported |
| [KIP-1043: Administration of groups](https://cwiki.apache.org/confluence/x/XoowEg) | 4.0.0 | Admin/Groups | P1 | Referenced | n/a |
| [KIP-1082: Require Client-Generated IDs over the ConsumerGroupHeartbeat RPC](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1082%3A%2BRequire%2BClient-Generated%2BIDs%2Bover%2Bthe%2BConsumerGroupHeartbeat%2BRPC) | 4.0.0 | Consumer | P1 | Referenced | Supported |
| [KIP-1102: Enable clients to rebootstrap based on timeout or error code](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1102%3A%2BEnable%2Bclients%2Bto%2Brebootstrap%2Bbased%2Bon%2Btimeout%2Bor%2Berror%2Bcode) | 4.0.0 | Protocol | P0 | Roadmap | Supported |
| [KIP-1139: Add support for OAuth jwt-bearer grant type](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1139%3A%2BAdd%2Bsupport%2Bfor%2BOAuth%2Bjwt-bearer%2Bgrant%2Btype) | 4.1.0 | Security | P1 | Unknown | Supported |
| [KIP-1274: Deprecate and remove support for Classic rebalance protocol in KafkaConsumer](https://cwiki.apache.org/confluence/display/KAFKA/KIP-1274%3A%2BDeprecate%2Band%2Bremove%2Bsupport%2Bfor%2BClassic%2Brebalance%2Bprotocol%2Bin%2BKafkaConsumer) | 4.3.0 target / 5.0+ phases | Consumer | P2 | Roadmap | n/a |

## Legacy KIPs explicitly filtered out from the main roadmap

| KIP | Kafka since | Why excluded from the modern roadmap |
|---|---|---|
| [KIP-35: Retrieving protocol version](https://cwiki.apache.org/confluence/display/KAFKA/KIP-35%2B-%2BRetrieving%2Bprotocol%2Bversion) | 0.10.0.0 | Pre-2.0 baseline capability |
| [KIP-74: Add Fetch Response Size Limit in Bytes](https://cwiki.apache.org/confluence/display/KAFKA/KIP-74%3A%2BAdd%2BFetch%2BResponse%2BSize%2BLimit%2Bin%2BBytes) | 0.10.1.0 | Pre-2.0 baseline capability |
| [KIP-82: Add Record Headers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-82%2B-%2BAdd%2BRecord%2BHeaders) | 0.11.0.0 | Pre-2.0 baseline capability |
| [KIP-84: Support SASL SCRAM mechanisms](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=67636971) | 0.10.2.0 | Pre-2.0 baseline capability |
| [KIP-85: Dynamic JAAS configuration for Kafka clients](https://cwiki.apache.org/confluence/display/KAFKA/KIP-85%3A%2BDynamic%2BJAAS%2Bconfiguration%2Bfor%2BKafka%2Bclients) | 0.10.2.0 | Pre-2.0 baseline capability |
| [KIP-86: Configurable SASL callback handlers](https://cwiki.apache.org/confluence/display/KAFKA/KIP-86%3A%2BConfigurable%2BSASL%2Bcallback%2Bhandlers) | 2.0.0 | Security baseline, but intentionally excluded per the previous filtering decision |

## Sources

- [Kafka Improvement Proposals index](https://cwiki.apache.org/confluence/pages/viewpage.action?pageId=293046882)
- [librdkafka INTRODUCTION](https://docs.confluent.io/platform/current/clients/librdkafka/html/md_INTRODUCTION.html)
