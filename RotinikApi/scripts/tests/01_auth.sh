#!/bin/bash
# =============================================================================
#   scripts/tests/01_auth.sh
#   Tests: User signup, protected route blocking, login (JWT), /me, cleanup.
#   Mirrors the existing test_api.sh but uses lib/common.sh helpers.
# =============================================================================

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/../lib/common.sh"

EMAIL="auth_$(date +%s)@rotinik.com"
PASSWORD="Password123"

header "AUTH TESTS — Signup · Login · JWT · /me"

# ── 1. Signup ─────────────────────────────────────────────────────────────────
section "1/5 · POST /api/User — Create test user"
http_call POST "/User" \
  -d "{\"name\":\"Auth Test User\",\"email\":\"$EMAIL\",\"phone\":\"+55 11 99999-9999\",\"password\":\"$PASSWORD\"}"

if [ "$HTTP_CODE" = "201" ]; then
  ok "User created (HTTP 201)"
  USER_ID=$(json_field "$BODY" "id")
else
  abort "Signup failed (HTTP $HTTP_CODE)"
fi

# ── 2. Unauthenticated access ─────────────────────────────────────────────────
section "2/5 · GET /api/User — Should be blocked (401)"
STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/User")
if [ "$STATUS" = "401" ]; then
  ok "Protected route correctly blocked (HTTP 401)"
else
  fail "Route should return 401 but returned HTTP $STATUS"
fi

# ── 3. Login ──────────────────────────────────────────────────────────────────
section "3/5 · POST /api/User/login — Get JWT"
http_call POST "/User/login" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}"

if [ "$HTTP_CODE" = "200" ]; then
  ok "Login successful (HTTP 200)"
else
  abort "Login failed (HTTP $HTTP_CODE)"
fi

TOKEN=$(json_field "$BODY" "token")
if [ -n "$TOKEN" ]; then
  ok "JWT token captured (${TOKEN:0:40}...)"
else
  abort "JWT token not found in response"
fi

# ── 4. GET /me with valid JWT ─────────────────────────────────────────────────
section "4/5 · GET /api/User/me — Authenticated profile"
http_call GET "/User/me" -H "Authorization: Bearer $TOKEN"

if [ "$HTTP_CODE" = "200" ]; then
  ok "Authenticated /me returned (HTTP 200)"
  RETURNED_EMAIL=$(json_field "$BODY" "email")
  if [ "$RETURNED_EMAIL" = "$EMAIL" ]; then
    ok "Email matches logged-in user"
  else
    fail "Email mismatch: expected '$EMAIL', got '$RETURNED_EMAIL'"
  fi
else
  fail "GET /me failed (HTTP $HTTP_CODE)"
fi

# ── 5. Cleanup — Delete test user ─────────────────────────────────────────────
section "5/5 · DELETE /api/User/$USER_ID — Cleanup"
http_call DELETE "/User/$USER_ID" -H "Authorization: Bearer $TOKEN"

if [ "$HTTP_CODE" = "204" ]; then
  ok "Test user deleted (HTTP 204)"
else
  fail "Cleanup failed (HTTP $HTTP_CODE)"
fi

footer
