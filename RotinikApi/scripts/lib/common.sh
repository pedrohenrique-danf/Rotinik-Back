#!/bin/bash
# =============================================================================
#   scripts/lib/common.sh
#   Shared utilities: colors, print helpers, HTTP call wrapper, token extractor.
#   Source this file at the top of every test script.
# =============================================================================

# ── Color palette ─────────────────────────────────────────────────────────────
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BOLD='\033[1m'
DIM='\033[2m'
NC='\033[0m'   # Reset

# ── Base URL (override with env var if needed) ────────────────────────────────
API_URL="${API_URL:-http://localhost:5025/api}"

# ── Pretty-printer (jq if available, cat otherwise) ──────────────────────────
JQ_CMD="cat"
command -v jq &>/dev/null && JQ_CMD="jq ."

# ── Pass/fail counters (accumulated per script) ───────────────────────────────
PASS=0
FAIL=0

# ── Printing helpers ──────────────────────────────────────────────────────────
ok()      { echo -e "${GREEN}  ✓ $1${NC}";      PASS=$((PASS + 1)); }
fail()    { echo -e "${RED}  ✗ $1${NC}";        FAIL=$((FAIL + 1)); }
info()    { echo -e "\n${CYAN}▶ $1${NC}"; }
section() { echo -e "\n${YELLOW}${BOLD}── $1 ──${NC}${DIM}"; }
header()  {
  local title="$1"
  echo ""
  echo -e "${YELLOW}${BOLD}╔══════════════════════════════════════════════════════════╗${NC}"
  printf "${YELLOW}${BOLD}║  %-56s  ║${NC}\n" "$title"
  echo -e "${YELLOW}${BOLD}╚══════════════════════════════════════════════════════════╝${NC}"
  echo ""
}
footer()  {
  echo ""
  echo -e "${YELLOW}${BOLD}╔══════════════════════════════════════════════════════════╗${NC}"
  printf "${YELLOW}${BOLD}║  Results: ${GREEN}%d passed${YELLOW}  ${RED}%d failed${NC}${YELLOW}${BOLD}%-36s║${NC}\n" "$PASS" "$FAIL" ""
  echo -e "${YELLOW}${BOLD}╚══════════════════════════════════════════════════════════╝${NC}"
  echo ""
}
abort()   { echo -e "${RED}${BOLD}  ✗ FATAL: $1 — aborting.${NC}\n"; footer; exit 1; }

# ── HTTP call: prints body + returns HTTP code in HTTP_CODE ───────────────────
#   Usage: http_call METHOD PATH [-H "..."] [-d "..."]
#          Body is pretty-printed; HTTP_CODE is set as global.
http_call() {
  local method="$1"; shift
  local path="$1";   shift
  local response
  response=$(curl -s -w "\n%{http_code}" -X "$method" "$API_URL$path" \
    -H "Content-Type: application/json" "$@")

  HTTP_CODE=$(echo "$response" | tail -1)
  BODY=$(echo "$response" | head -n -1)

  echo -e "${DIM}$(echo "$BODY" | $JQ_CMD)${NC}"
}

# ── Extract a JSON field value (no jq required) ───────────────────────────────
#   Usage: json_field BODY "fieldName"
#   Returns the first match only (prevents picking up nested array values).
json_field() {
  local body="$1"
  local field="$2"
  echo "$body" | grep -o "\"$field\":[^,}]*" | head -1 | sed "s/\"$field\"://" | tr -d '"' | xargs
}
