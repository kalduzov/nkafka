#!/usr/bin/env sh

set -eu

TOPIC_NAME="${1:-test_topic}"
KAFKA_VERSION="${KAFKA_VERSION:-3.9.1}"
KAFKA_ADVERTISED_HOST="${KAFKA_ADVERTISED_HOST:-localhost}"
SSL_STORE_PASSWORD="${SSL_STORE_PASSWORD:-changeit}"
KAFKA_CLUSTER_ID="${KAFKA_CLUSTER_ID:-4dYfr59lTFyBKgAAfge3lg}"
PROJECT_NAME="nkafka-e2e-kraft-ssl-3broker"
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
COMPOSE_FILE="$SCRIPT_DIR/docker-compose.yml"
TOPIC_REPLICA_ASSIGNMENT="1:2:3,2:3:1,3:1:2,1:3:2,2:1:3,3:2:1,1:2:3,2:3:1,3:1:2"
TRUSTSTORE_PATH="$SCRIPT_DIR/certs/client.truststore.p12"

invoke_compose() {
  KAFKA_VERSION="$KAFKA_VERSION" \
  KAFKA_ADVERTISED_HOST="$KAFKA_ADVERTISED_HOST" \
  KAFKA_CLUSTER_ID="$KAFKA_CLUSTER_ID" \
  SSL_STORE_PASSWORD="$SSL_STORE_PASSWORD" \
  BROKER_1_EXTERNAL_PORT="29091" \
  BROKER_2_EXTERNAL_PORT="29092" \
  BROKER_3_EXTERNAL_PORT="29093" \
  BROKER_1_CONTAINER_NAME="nkafka-e2e-kraft-ssl-3broker-broker-1" \
  BROKER_2_CONTAINER_NAME="nkafka-e2e-kraft-ssl-3broker-broker-2" \
  BROKER_3_CONTAINER_NAME="nkafka-e2e-kraft-ssl-3broker-broker-3" \
    docker compose --project-name "$PROJECT_NAME" -f "$COMPOSE_FILE" "$@"
}

wait_port_ready() {
  host="$1"
  port="$2"
  timeout_seconds="${3:-120}"
  start_time="$(date +%s)"

  while :; do
    if python - "$host" "$port" <<'PY'
import socket
import sys

host = sys.argv[1]
port = int(sys.argv[2])

with socket.create_connection((host, port), timeout=2):
    pass
PY
    then
      return 0
    fi

    current_time="$(date +%s)"
    elapsed=$((current_time - start_time))
    if [ "$elapsed" -ge "$timeout_seconds" ]; then
      printf '%s\n' "Broker did not become ready on $host:$port within $timeout_seconds seconds." >&2
      exit 1
    fi

    sleep 3
  done
}

wait_admin_ready() {
  container_name="$1"
  timeout_seconds="${2:-120}"
  start_time="$(date +%s)"

  while :; do
    if docker exec "$container_name" /opt/kafka/bin/kafka-topics.sh \
      --bootstrap-server kafka-1:9094 \
      --list >/dev/null 2>/dev/null; then
      return 0
    fi

    current_time="$(date +%s)"
    elapsed=$((current_time - start_time))
    if [ "$elapsed" -ge "$timeout_seconds" ]; then
      printf '%s\n' "Kafka admin path did not become ready inside container '$container_name' within $timeout_seconds seconds." >&2
      exit 1
    fi

    sleep 3
  done
}

ensure_test_topic() {
  container_name="$1"
  topic_name="$2"
  max_attempts="${3:-20}"

  attempt=1
  while [ "$attempt" -le "$max_attempts" ]; do
    if docker exec "$container_name" /opt/kafka/bin/kafka-topics.sh \
      --bootstrap-server kafka-1:9094 \
      --describe \
      --topic "$topic_name" >/tmp/nkafka-topic-description.txt 2>/dev/null; then
      :
    else
      if ! docker exec "$container_name" /opt/kafka/bin/kafka-topics.sh \
        --bootstrap-server kafka-1:9094 \
        --create \
        --topic "$topic_name" \
        --replica-assignment "$TOPIC_REPLICA_ASSIGNMENT"; then
        if [ "$attempt" -eq "$max_attempts" ]; then
          printf '%s\n' "Failed to create topic '$topic_name'." >&2
          exit 1
        fi

        attempt=$((attempt + 1))
        sleep 3
        continue
      fi

      docker exec "$container_name" /opt/kafka/bin/kafka-topics.sh \
        --bootstrap-server kafka-1:9094 \
        --describe \
        --topic "$topic_name" >/tmp/nkafka-topic-description.txt
    fi

    partition_count="$(grep -c 'Partition:' /tmp/nkafka-topic-description.txt || true)"
    if [ "$partition_count" -ne 9 ]; then
      if [ "$attempt" -eq "$max_attempts" ]; then
        printf '%s\n' "Topic '$topic_name' does not have 9 partitions." >&2
        exit 1
      fi

      attempt=$((attempt + 1))
      sleep 3
      continue
    fi

    for broker_id in 1 2 3; do
      leader_count="$(grep -Ec "Leader:[[:space:]]+$broker_id([[:space:]]|$)" /tmp/nkafka-topic-description.txt || true)"
      if [ "$leader_count" -ne 3 ]; then
        if [ "$attempt" -eq "$max_attempts" ]; then
          printf '%s\n' "Topic '$topic_name' does not have 3 leader partitions on broker '$broker_id'." >&2
          exit 1
        fi

        attempt=$((attempt + 1))
        sleep 3
        continue 2
      fi
    done

    rm -f /tmp/nkafka-topic-description.txt
    return 0
  done
}

"$SCRIPT_DIR/../../scripts/ensure-ssl-certs.sh" "$SCRIPT_DIR" "$SSL_STORE_PASSWORD"

invoke_compose up -d

wait_port_ready "$KAFKA_ADVERTISED_HOST" 29091
wait_port_ready "$KAFKA_ADVERTISED_HOST" 29092
wait_port_ready "$KAFKA_ADVERTISED_HOST" 29093

wait_admin_ready "nkafka-e2e-kraft-ssl-3broker-broker-1"
ensure_test_topic "nkafka-e2e-kraft-ssl-3broker-broker-1" "$TOPIC_NAME"

printf '\nKRaft SSL 3-broker environment is ready.\n'
printf 'Topic: %s\n' "$TOPIC_NAME"
printf 'Bootstrap servers: %s\n' "${KAFKA_ADVERTISED_HOST}:29091,${KAFKA_ADVERTISED_HOST}:29092,${KAFKA_ADVERTISED_HOST}:29093"
printf 'Client truststore: %s\n' "$TRUSTSTORE_PATH"
printf 'SSL truststore password: %s\n' "$SSL_STORE_PASSWORD"
printf 'SSL truststore type: %s\n' "PKCS12"
