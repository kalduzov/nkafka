#!/usr/bin/env sh

set -eu

TARGET_FRAMEWORK="${TARGET_FRAMEWORK:-net9.0}"
KEEP_ENVIRONMENT="${KEEP_ENVIRONMENT:-false}"
KAFKA_VERSIONS="${KAFKA_VERSIONS:-3.7.1 3.8.0 3.9.1}"

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
REPOSITORY_ROOT="$(CDPATH= cd -- "$SCRIPT_DIR/../../../.." && pwd)"
COMPOSE_FILE="$SCRIPT_DIR/docker-compose.yml"
BOOTSTRAP_SERVERS="localhost:29092"

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
  KAFKA_EXTERNAL_PORT="29092" \
  KAFKA_ADVERTISED_HOST="localhost" \
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

invoke_api_versions_test_run() {
  kafka_version="$1"
  project_path="$2"
  max_attempts="${3:-5}"

  # The E2E harness selects concrete scenarios from environment so one script can
  # drive the same broker-backed flow across multiple Kafka version lines.
  attempt=1
  while [ "$attempt" -le "$max_attempts" ]; do
    if NKAFKA_E2E_ENABLED="true" \
      NKAFKA_E2E_TOPOLOGY_MODE="kraft" \
      NKAFKA_E2E_SECURITY_PROFILE="plaintext" \
      NKAFKA_E2E_KAFKA_VERSION="$kafka_version" \
      NKAFKA_E2E_BOOTSTRAP_SERVERS="$BOOTSTRAP_SERVERS" \
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
  project_name="nkafka-e2e-kraft-plaintext-$version_token"
  container_name="nkafka-e2e-kraft-plaintext-$version_token"
  project_path="$(get_project_path_for_kafka_version "$kafka_version")"

  CURRENT_KAFKA_VERSION="$kafka_version"
  CURRENT_CONTAINER_NAME="$container_name"

  printf '\n=== Kafka %s / ApiVersions E2E ===\n' "$kafka_version"

  invoke_compose "$project_name" down -v --remove-orphans >/dev/null 2>&1 || true

  cleanup() {
    if [ "$KEEP_ENVIRONMENT" != "true" ]; then
      invoke_compose "$project_name" down -v --remove-orphans
    fi
  }

  trap cleanup EXIT INT TERM

  invoke_compose "$project_name" up -d
  wait_kafka_ready "localhost" "29092" 120
  invoke_api_versions_test_run "$kafka_version" "$project_path"

  trap - EXIT INT TERM
  cleanup
done
