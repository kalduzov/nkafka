#!/usr/bin/env sh

set -eu

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
COMPOSE_FILE="$SCRIPT_DIR/docker-compose.yml"
PROJECT_NAME="nkafka-e2e-kraft-ssl-3broker"

docker compose --project-name "$PROJECT_NAME" -f "$COMPOSE_FILE" down -v --remove-orphans
