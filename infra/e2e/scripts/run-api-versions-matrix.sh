#!/usr/bin/env sh

set -eu

PROFILE_DIRECTORY="$1"
TOPOLOGY_MODE="$2"
SECURITY_PROFILE="$3"
TARGET_FRAMEWORK="${TARGET_FRAMEWORK:-net9.0}"
KEEP_ENVIRONMENT="${KEEP_ENVIRONMENT:-false}"
KAFKA_VERSIONS="${KAFKA_VERSIONS:-3.7.1 3.8.0 3.9.1}"
BOOTSTRAP_HOST="${BOOTSTRAP_HOST:-localhost}"
BOOTSTRAP_PORT="${BOOTSTRAP_PORT:-29092}"
SASL_MECHANISM="${SASL_MECHANISM:-}"
SASL_USERNAME="${SASL_USERNAME:-test}"
SASL_PASSWORD="${SASL_PASSWORD:-test}"
SSL_STORE_PASSWORD="${SSL_STORE_PASSWORD:-changeit}"
SCRAM_BOOTSTRAP_MECHANISM="${SCRAM_BOOTSTRAP_MECHANISM:-}"
INTERNAL_BOOTSTRAP_PORT="${INTERNAL_BOOTSTRAP_PORT:-9094}"
REQUIRES_SSL_ARTIFACTS="${REQUIRES_SSL_ARTIFACTS:-false}"

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
REPOSITORY_ROOT="$(CDPATH= cd -- "$PROFILE_DIRECTORY/../../../.." && pwd)"
COMPOSE_FILE="$PROFILE_DIRECTORY/docker-compose.yml"
BOOTSTRAP_SERVERS="$BOOTSTRAP_HOST:$BOOTSTRAP_PORT"

get_project_path_for_kafka_version() {
  kafka_version="$1"

  case "$kafka_version" in
    3.7.*) printf '%s\n' "$REPOSITORY_ROOT/tests/integration/NKafka.IntegrationTests.Kafka_3_7/NKafka.IntegrationTests.Kafka_3_7.csproj" ;;
    3.8.*) printf '%s\n' "$REPOSITORY_ROOT/tests/integration/NKafka.IntegrationTests.Kafka_3_8/NKafka.IntegrationTests.Kafka_3_8.csproj" ;;
    3.9.*) printf '%s\n' "$REPOSITORY_ROOT/tests/integration/NKafka.IntegrationTests.Kafka_3_9/NKafka.IntegrationTests.Kafka_3_9.csproj" ;;
    *)
      printf '%s\n' "No version-specific E2E assembly is configured for Kafka version '$kafka_version'." >&2
      exit 1
      ;;
  esac
}

invoke_compose() {
  project_name="$1"
  shift

  KAFKA_VERSION="$CURRENT_KAFKA_VERSION" \
  KAFKA_CONTAINER_NAME="$CURRENT_CONTAINER_NAME" \
  KAFKA_EXTERNAL_PORT="$BOOTSTRAP_PORT" \
  KAFKA_ADVERTISED_HOST="$BOOTSTRAP_HOST" \
  SASL_USERNAME="$SASL_USERNAME" \
  SASL_PASSWORD="$SASL_PASSWORD" \
  SSL_STORE_PASSWORD="$SSL_STORE_PASSWORD" \
  ZOOKEEPER_CONTAINER_NAME="$CURRENT_ZOOKEEPER_CONTAINER_NAME" \
  ZOOKEEPER_EXTERNAL_PORT="22181" \
    docker compose --project-name "$project_name" -f "$COMPOSE_FILE" "$@"
}

wait_kafka_ready() {
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
      printf '%s\n' "Kafka broker did not become ready on $host:$port within $timeout_seconds seconds." >&2
      exit 1
    fi

    sleep 3
  done
}

initialize_scram_credentials() {
  kafka_container_name="$1"
  mechanism="$2"
  user_name="$3"
  password="$4"
  bootstrap_port="$5"
  max_attempts="${6:-10}"
  scram_config="$mechanism=[password=$password]"

  attempt=1
  while [ "$attempt" -le "$max_attempts" ]; do
    if docker exec "$kafka_container_name" /opt/kafka/bin/kafka-configs.sh \
      --bootstrap-server "localhost:$bootstrap_port" \
      --alter \
      --add-config "$scram_config" \
      --entity-type users \
      --entity-name "$user_name"; then
      return 0
    fi

    if [ "$attempt" -eq "$max_attempts" ]; then
      printf '%s\n' "Failed to bootstrap SCRAM credentials for container '$kafka_container_name'." >&2
      exit 1
    fi

    attempt=$((attempt + 1))
    sleep 3
  done
}

invoke_api_versions_test_run() {
  kafka_version="$1"
  project_path="$2"
  max_attempts="${3:-5}"

  # The E2E harness selects concrete scenarios from environment so one script can
  # drive the same broker-backed flow across multiple Kafka version lines.
  attempt=1
  while [ "$attempt" -le "$max_attempts" ]; do
    if NKAFKA_E2E_ENABLED="true" \
      NKAFKA_E2E_TOPOLOGY_MODE="$TOPOLOGY_MODE" \
      NKAFKA_E2E_SECURITY_PROFILE="$SECURITY_PROFILE" \
      NKAFKA_E2E_KAFKA_VERSION="$kafka_version" \
      NKAFKA_E2E_BOOTSTRAP_SERVERS="$BOOTSTRAP_SERVERS" \
      NKAFKA_E2E_SASL_MECHANISM="$SASL_MECHANISM" \
      NKAFKA_E2E_SASL_USERNAME="$SASL_USERNAME" \
      NKAFKA_E2E_SASL_PASSWORD="$SASL_PASSWORD" \
      NKAFKA_E2E_TRUST_SERVER_CERTIFICATE="$REQUIRES_SSL_ARTIFACTS" \
        dotnet test "$project_path" -f "$TARGET_FRAMEWORK" --filter "FullyQualifiedName~ApiVersionsE2ETests"; then
      return 0
    fi

    if [ "$attempt" -eq "$max_attempts" ]; then
      printf '%s\n' "ApiVersions E2E tests failed for Kafka version '$kafka_version' after $max_attempts attempts." >&2
      exit 1
    fi

    attempt=$((attempt + 1))
    sleep 5
  done
}

for kafka_version in $KAFKA_VERSIONS; do
  version_token="$(printf '%s' "$kafka_version" | tr '.' '-')"
  security_token="$(printf '%s' "$SECURITY_PROFILE" | tr '/' '-')"
  project_name="nkafka-e2e-$TOPOLOGY_MODE-$security_token-$version_token"
  container_name="nkafka-e2e-$TOPOLOGY_MODE-$security_token-$version_token"
  zookeeper_container_name="nkafka-e2e-$TOPOLOGY_MODE-$security_token-zookeeper-$version_token"
  project_path="$(get_project_path_for_kafka_version "$kafka_version")"

  CURRENT_KAFKA_VERSION="$kafka_version"
  CURRENT_CONTAINER_NAME="$container_name"
  CURRENT_ZOOKEEPER_CONTAINER_NAME="$zookeeper_container_name"

  if [ "$REQUIRES_SSL_ARTIFACTS" = "true" ]; then
    "$SCRIPT_DIR/ensure-ssl-certs.sh" "$PROFILE_DIRECTORY" "$SSL_STORE_PASSWORD"
  fi

  printf '\n=== Kafka %s / %s / %s / ApiVersions E2E ===\n' "$kafka_version" "$TOPOLOGY_MODE" "$SECURITY_PROFILE"

  invoke_compose "$project_name" down -v --remove-orphans >/dev/null 2>&1 || true

  cleanup() {
    if [ "$KEEP_ENVIRONMENT" != "true" ]; then
      invoke_compose "$project_name" down -v --remove-orphans
    fi
  }

  trap cleanup EXIT INT TERM

  invoke_compose "$project_name" up -d
  wait_kafka_ready "$BOOTSTRAP_HOST" "$BOOTSTRAP_PORT" 120

  if [ -n "$SCRAM_BOOTSTRAP_MECHANISM" ]; then
    initialize_scram_credentials "$container_name" "$SCRAM_BOOTSTRAP_MECHANISM" "$SASL_USERNAME" "$SASL_PASSWORD" "$INTERNAL_BOOTSTRAP_PORT"
  fi

  invoke_api_versions_test_run "$kafka_version" "$project_path"

  trap - EXIT INT TERM
  cleanup
done
