#!/usr/bin/env bash
set -euo pipefail

# TODO: Externalize host mapping if running outside Docker Desktop/Linux.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_BASE_URL="${API_BASE_URL:-http://host.docker.internal:5000}"
TIMESTAMP="$(date +%Y%m%d%H%M%S)"
SUMMARY_NAME="steady_state-${TIMESTAMP}.json"
SUMMARY_FILE="reports/${SUMMARY_NAME}"
SUMMARY_CONTAINER_PATH="/src/${SUMMARY_FILE}"

mkdir -p "${SCRIPT_DIR}/reports"

docker run --rm \
  -u "$(id -u):$(id -g)" \
  -v "${SCRIPT_DIR}:/src" \
  -w /src \
  grafana/k6 run \
  -e API_BASE_URL="${API_BASE_URL}" \
  --summary-export "${SUMMARY_CONTAINER_PATH}" \
  scenarios/steady_state.js

echo "k6 steady_state summary saved to ${SUMMARY_FILE}" >&2
