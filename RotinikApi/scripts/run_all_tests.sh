#!/bin/bash
# =============================================================================
#   scripts/run_all_tests.sh
#   Master test runner — executes all test suites in order and prints a
#   consolidated summary across all suites.
#
#   Usage:
#     ./scripts/run_all_tests.sh            # run all tests
#     ./scripts/run_all_tests.sh 02 03      # run only specific suites by prefix
#     API_URL=http://localhost:5000/api ./scripts/run_all_tests.sh
# =============================================================================

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TESTS_DIR="$SCRIPT_DIR/tests"

# ── Colors (standalone, not sourcing lib to keep runner self-contained) ───────
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
BOLD='\033[1m'
NC='\033[0m'

TOTAL_PASS=0
TOTAL_FAIL=0
SUITE_RESULTS=()

# ── Collect test files ────────────────────────────────────────────────────────
if [ $# -gt 0 ]; then
  # Filter by prefix arguments (e.g. "02" "03")
  SUITES=()
  for prefix in "$@"; do
    while IFS= read -r file; do
      SUITES+=("$file")
    done < <(find "$TESTS_DIR" -name "${prefix}*.sh" | sort)
  done
else
  mapfile -t SUITES < <(find "$TESTS_DIR" -name "*.sh" | sort)
fi

if [ ${#SUITES[@]} -eq 0 ]; then
  echo -e "${RED}No test files found in $TESTS_DIR${NC}"
  exit 1
fi

# ── Banner ────────────────────────────────────────────────────────────────────
echo ""
echo -e "${YELLOW}${BOLD}╔══════════════════════════════════════════════════════════╗${NC}"
echo -e "${YELLOW}${BOLD}║          ROTINIK API — FULL TEST SUITE RUNNER            ║${NC}"
echo -e "${YELLOW}${BOLD}╚══════════════════════════════════════════════════════════╝${NC}"
echo -e "${CYAN}  API: ${API_URL:-http://localhost:5025/api}${NC}"
echo -e "${CYAN}  Suites: ${#SUITES[@]}${NC}"
echo ""

# ── Run each suite ────────────────────────────────────────────────────────────
for suite in "${SUITES[@]}"; do
  name=$(basename "$suite")
  echo -e "${CYAN}${BOLD}┌─ Running: $name ─────────────────────────────────────────${NC}"

  # Capture output and extract pass/fail from footer line
  output=$(bash "$suite" 2>&1)
  exit_code=$?

  echo "$output"

  # Extract counters from footer line "Results: N passed  M failed"
  suite_pass=$(echo "$output" | grep -o '[0-9]* passed' | grep -o '[0-9]*')
  suite_fail=$(echo "$output" | grep -o '[0-9]* failed' | grep -o '[0-9]*')
  suite_pass="${suite_pass:-0}"
  suite_fail="${suite_fail:-0}"

  TOTAL_PASS=$((TOTAL_PASS + suite_pass))
  TOTAL_FAIL=$((TOTAL_FAIL + suite_fail))

  if [ "$suite_fail" = "0" ]; then
    STATUS_ICON="${GREEN}${BOLD}✓ PASSED${NC}"
  else
    STATUS_ICON="${RED}${BOLD}✗ FAILED${NC}"
  fi
  SUITE_RESULTS+=("$(printf "  %-38s %s  (%d✓ %d✗)" "$name" "$(echo -e "$STATUS_ICON")" "$suite_pass" "$suite_fail")")

  echo -e "${CYAN}${BOLD}└──────────────────────────────────────────────────────────${NC}"
  echo ""
done

# ── Consolidated summary ──────────────────────────────────────────────────────
echo -e "${YELLOW}${BOLD}╔══════════════════════════════════════════════════════════╗${NC}"
echo -e "${YELLOW}${BOLD}║                   CONSOLIDATED RESULTS                  ║${NC}"
echo -e "${YELLOW}${BOLD}╠══════════════════════════════════════════════════════════╣${NC}"
for result in "${SUITE_RESULTS[@]}"; do
  printf "${YELLOW}${BOLD}║${NC}%s${YELLOW}${BOLD}║${NC}\n" "$result"
done
echo -e "${YELLOW}${BOLD}╠══════════════════════════════════════════════════════════╣${NC}"
printf "${YELLOW}${BOLD}║  Total: ${GREEN}%d passed${NC}${YELLOW}${BOLD}  ${RED}%d failed${NC}${YELLOW}${BOLD}%-30s║${NC}\n" \
  "$TOTAL_PASS" "$TOTAL_FAIL" ""
echo -e "${YELLOW}${BOLD}╚══════════════════════════════════════════════════════════╝${NC}"
echo ""

[ "$TOTAL_FAIL" -eq 0 ] && exit 0 || exit 1
