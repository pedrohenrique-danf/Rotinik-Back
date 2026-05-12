#!/bin/bash
# =============================================================================
#   scripts/tests/02_routines.sh
#   Tests: FMRT_1 (create), FMRT_2 (update), FMRT_3 (delete),
#          FMRT_13 (list templates, clone template).
#   Requires a valid API running at $API_URL.
# =============================================================================

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/../lib/common.sh"

EMAIL="routine_$(date +%s)@rotinik.com"
PASSWORD="Password123"

header "ROUTINE TESTS — FMRT_1 · FMRT_2 · FMRT_3 · FMRT_13"

# ── Setup: create user and get token ──────────────────────────────────────────
section "Setup · Create user & login"
http_call POST "/User" \
  -d "{\"name\":\"Routine Tester\",\"email\":\"$EMAIL\",\"phone\":\"+55 11 99999-9999\",\"password\":\"$PASSWORD\"}"
[ "$HTTP_CODE" = "201" ] || abort "User signup failed (HTTP $HTTP_CODE)"
USER_ID=$(json_field "$BODY" "id")
ok "User created (ID: $USER_ID)"

http_call POST "/User/login" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}"
[ "$HTTP_CODE" = "200" ] || abort "Login failed (HTTP $HTTP_CODE)"
TOKEN=$(json_field "$BODY" "token")
[ -n "$TOKEN" ] || abort "No token in login response"
ok "Logged in — token acquired"
AUTH="-H \"Authorization: Bearer $TOKEN\""

# ── FMRT_1 — Create routine ───────────────────────────────────────────────────
section "FMRT_1 · POST /api/Routine — Create a routine"
http_call POST "/Routine" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"name":"My Morning Routine","description":"A fresh start every day","theme":"Morning"}'

if [ "$HTTP_CODE" = "201" ]; then
  ok "Routine created (HTTP 201)"
  ROUTINE_ID=$(json_field "$BODY" "id")
  ok "Routine ID: $ROUTINE_ID"
else
  abort "Create routine failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_1 — Unauthenticated create should be blocked ────────────────────────
section "FMRT_1 · POST /api/Routine (no token) — Should be 401"
STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X POST "$API_URL/Routine" \
  -H "Content-Type: application/json" \
  -d '{"name":"Ghost Routine"}')
if [ "$STATUS" = "401" ]; then
  ok "Unauthenticated create correctly blocked (HTTP 401)"
else
  fail "Expected 401 but got HTTP $STATUS"
fi

# ── FMRT_1 — List routines ────────────────────────────────────────────────────
section "FMRT_1 · GET /api/Routine — List user routines"
http_call GET "/Routine" -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "200" ]; then
  ok "Routine list returned (HTTP 200)"
else
  fail "Listing routines failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_1 — Get by ID ───────────────────────────────────────────────────────
section "FMRT_1 · GET /api/Routine/$ROUTINE_ID — Get routine detail"
http_call GET "/Routine/$ROUTINE_ID" -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "200" ]; then
  ok "Routine detail returned (HTTP 200)"
  NAME=$(json_field "$BODY" "name")
  ok "Name: $NAME"
else
  fail "Get routine by ID failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_2 — Update routine ───────────────────────────────────────────────────
section "FMRT_2 · PUT /api/Routine/$ROUTINE_ID — Update routine"
http_call PUT "/Routine/$ROUTINE_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"name":"My Updated Morning Routine","theme":"Morning & Meditation"}'

if [ "$HTTP_CODE" = "200" ]; then
  ok "Routine updated (HTTP 200)"
  UPDATED_NAME=$(json_field "$BODY" "name")
  ok "New name: $UPDATED_NAME"
else
  fail "Update routine failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_13 — List templates ─────────────────────────────────────────────────
section "FMRT_13 · GET /api/Routine/templates — List pre-made templates"
http_call GET "/Routine/templates" -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "200" ]; then
  ok "Templates returned (HTTP 200)"
else
  fail "List templates failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_13 — Clone template (template ID 1 = Morning Routine) ───────────────
section "FMRT_13 · POST /api/Routine/templates/1/clone — Clone Morning template"
http_call POST "/Routine/templates/1/clone" \
  -H "Authorization: Bearer $TOKEN"

if [ "$HTTP_CODE" = "201" ]; then
  ok "Template cloned (HTTP 201)"
  CLONED_ID=$(json_field "$BODY" "id")
  ok "Cloned routine ID: $CLONED_ID"
else
  fail "Clone template failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_3 — Delete routine ───────────────────────────────────────────────────
section "FMRT_3 · DELETE /api/Routine/$ROUTINE_ID — Delete routine"
http_call DELETE "/Routine/$ROUTINE_ID" -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "204" ]; then
  ok "Routine deleted (HTTP 204)"
else
  fail "Delete routine failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_3 — Verify deletion ─────────────────────────────────────────────────
section "FMRT_3 · GET /api/Routine/$ROUTINE_ID — Should be 404 after delete"
http_call GET "/Routine/$ROUTINE_ID" -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "404" ]; then
  ok "Deleted routine returns 404 (HTTP 404)"
else
  fail "Expected 404 but got HTTP $HTTP_CODE"
fi

# ── Cleanup ───────────────────────────────────────────────────────────────────
section "Cleanup · DELETE /api/User/$USER_ID"
http_call DELETE "/User/$USER_ID" -H "Authorization: Bearer $TOKEN"
[ "$HTTP_CODE" = "204" ] && ok "Test user deleted" || fail "Cleanup failed (HTTP $HTTP_CODE)"

footer
