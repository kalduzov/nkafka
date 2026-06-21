#!/usr/bin/env sh

set -eu

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
SHARED_SCRIPT="$SCRIPT_DIR/../../scripts/run-api-versions-matrix.sh"
export SASL_MECHANISM=OAuthBearer

exec "$SHARED_SCRIPT" "$SCRIPT_DIR" "kraft" "sasl-oauthbearer"

