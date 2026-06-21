#!/usr/bin/env sh

set -eu

SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
SHARED_SCRIPT="$SCRIPT_DIR/../../scripts/run-api-versions-matrix.sh"
export SASL_MECHANISM=ScramSha256
export REQUIRES_SSL_ARTIFACTS=true
export SCRAM_BOOTSTRAP_MECHANISM='SCRAM-SHA-256'

exec "$SHARED_SCRIPT" "$SCRIPT_DIR" "kraft" "sasl-ssl-scram256"

