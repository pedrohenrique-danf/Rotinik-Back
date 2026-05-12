#!/bin/bash
# =============================================================================
#   scripts/run_tests.sh
#   Master test runner — executes all test suites in order and prints a
#   consolidated summary across all suites.
#
#   Usage:
#     ./scripts/run_tests.sh            # run all tests (quiet mode)
#     ./scripts/run_tests.sh 02 03      # run only specific suites by prefix
#     API_URL=http://localhost:5000/api ./scripts/run_tests.sh
#     VERBOSE=1 ./scripts/run_tests.sh  # show full HTTP bodies + section headers
# =============================================================================

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TESTS_DIR="$SCRIPT_DIR/tests"

# ── Source common libraries ───────────────────────────────────────────────────
source "$SCRIPT_DIR/lib/common.sh"

TOTAL_PASS=0
TOTAL_FAIL=0
SUITE_RESULTS=()

# ── Collect test files ────────────────────────────────────────────────────────
if [ $# -gt 0 ]; then
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
echo ""

# ── Run each suite ────────────────────────────────────────────────────────────
# QUIET=1: suppress HTTP bodies, section headers, per-file banners.
# Override with VERBOSE=1 env var to see full output.
QUIET="${VERBOSE:-0}"
[[ "$VERBOSE" = "1" ]] && QUIET=0 || QUIET=1

for suite in "${SUITES[@]}"; do
  name=$(basename "$suite")
  label=$(basename "$name" .sh | sed 's/^[0-9]*_//' | sed 's/_/ /g' | tr '[:lower:]' '[:upper:]')
  echo -e "${CYAN}${BOLD}  ▶ $label${NC}"

  # Reset per-file counters
  FILE_PASS=0
  FILE_FAIL=0

  source "$suite"

  TOTAL_PASS=$((TOTAL_PASS + FILE_PASS))
  TOTAL_FAIL=$((TOTAL_FAIL + FILE_FAIL))

  if [ "$FILE_FAIL" = "0" ]; then
    STATUS_ICON="${GREEN}${BOLD}✓ PASSED${NC}"
  else
    STATUS_ICON="${RED}${BOLD}✗ FAILED${NC}"
  fi
  SUITE_RESULTS+=("$(printf "  %-38s %s  (%d✓ %d✗)" "$name" "$(echo -e "$STATUS_ICON")" "$FILE_PASS" "$FILE_FAIL")")

  echo ""
done

# ── Consolidated summary ──────────────────────────────────────────────────────
echo -e "${YELLOW}${BOLD}╔══════════════════════════════════════════════════════════╗${NC}"
echo -e "${YELLOW}${BOLD}║                   CONSOLIDATED RESULTS                   ║${NC}"
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
