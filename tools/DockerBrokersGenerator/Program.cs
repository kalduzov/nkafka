/*
  SharpLab tools in Run mode:
    • value.Inspect()
    • Inspect.Heap(object)
    • Inspect.Stack(value)
    • Inspect.MemoryGraph(value1, value2, …)
*/

const string template = @"  bc_broker-{0}:
    image: 'confluentinc/cp-kafka:${{CONFLUENT_PLATFORM_VERSION:-7.0.0}}'
    hostname: bc_broker-{0}
    restart: always
    container_name: bc_broker-{0}
    depends_on:
      - bc_zookeeper-1
      - bc_zookeeper-2
      - bc_zookeeper-3
    ports:
      - '{1}:{1}'
    environment:
      KAFKA_BROKER_ID: '{0}'
      KAFKA_BROKER_RACK: '{0}'
      KAFKA_ZOOKEEPER_CONNECT: 'bc_zookeeper-1:2181,bc_zookeeper-2:2181,bc_zookeeper-3:2181'
      KAFKA_LISTENERS: 'LISTENER_INTERNAL://:9091,LISTENER_LOCAL://:{1}'
      KAFKA_ADVERTISED_LISTENERS: 'LISTENER_INTERNAL://bc_broker-{0}:9091,LISTENER_LOCAL://localhost:{1}'
      KAFKA_INTER_BROKER_LISTENER_NAME: 'LISTENER_INTERNAL'
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: '${{KAFKA_LISTENER_SECURITY_PROTOCOL_MAP}}'
      KAFKA_DEFAULT_REPLICATION_FACTOR: '2'
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 2
      KAFKA_ZOOKEEPER_SESSION_TIMEOUT_MS: '3000'
      KAFKA_ZOOKEEPER_CONNECTION_TIMEOUT_MS: '3000'
      KAFKA_TRANSACTION_STATE_LOG_MIN_ISR: 1
      KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR: 1
      KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS: 0
    networks:
      - bc_kafka-network";

using var file = File.CreateText("brokers");

for (var i = 6; i < 100; i++)
{
    var port = 29095 + i;
    file.WriteLine(template, i, port);
    file.WriteLine();
}