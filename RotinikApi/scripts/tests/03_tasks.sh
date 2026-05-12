#!/bin/bash
# =============================================================================
#   scripts/tests/03_tasks.sh
#   Tests: FMRT_4 (create task), FMRT_5 (edit), FMRT_6 (delete),
#          FMRT_10 (frequency), FMRT_11 (type).
# =============================================================================

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/../lib/common.sh"

EMAIL="task_$(date +%s)@rotinik.com"
PASSWORD="Password123"

header "TASK TESTS — FMRT_4 · FMRT_5 · FMRT_6 · FMRT_10 · FMRT_11"

# ── Setup ─────────────────────────────────────────────────────────────────────
section "Setup · Create user & login"
http_call POST "/User" \
  -d "{\"name\":\"Task Tester\",\"email\":\"$EMAIL\",\"phone\":\"+55 11 99999-9999\",\"password\":\"$PASSWORD\"}"
[ "$HTTP_CODE" = "201" ] || abort "User signup failed (HTTP $HTTP_CODE)"
USER_ID=$(json_field "$BODY" "id")
ok "User created (ID: $USER_ID)"

http_call POST "/User/login" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}"
[ "$HTTP_CODE" = "200" ] || abort "Login failed (HTTP $HTTP_CODE)"
TOKEN=$(json_field "$BODY" "token")
[ -n "$TOKEN" ] || abort "No token in login response"
ok "Logged in — token acquired"

# ── FMRT_4 · FMRT_10 · FMRT_11 — Create task ────────────────────────────────
section "FMRT_4/10/11 · POST /api/Task — Create task (Daily, Important, 30 min)"
http_call POST "/Task" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"Read a Book Chapter","description":"Daily reading habit","estimatedMinutes":30,"frequency":0,"type":2}'

# frequency: 0=Daily, 1=Monthly, 2=Yearly
# type:      0=Normal, 1=Moderate, 2=Important, 3=Urgent

if [ "$HTTP_CODE" = "201" ]; then
  ok "Task created (HTTP 201)"
  TASK_ID=$(json_field "$BODY" "id")
  FREQ=$(json_field "$BODY" "frequencyLabel")
  TYPE=$(json_field "$BODY" "typeLabel")
  ok "Task ID: $TASK_ID | Frequency: $FREQ | Type: $TYPE"
else
  abort "Create task failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_4 — Validation: missing required fields ──────────────────────────────
section "FMRT_4 · POST /api/Task — Invalid: empty title (expect 400)"
http_call POST "/Task" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"","estimatedMinutes":30,"frequency":0,"type":2}'
if [ "$HTTP_CODE" = "400" ]; then
  ok "Validation correctly rejected empty title (HTTP 400)"
else
  fail "Expected 400 for empty title, got HTTP $HTTP_CODE"
fi

# ── FMRT_4 — Validation: estimatedMinutes out of range ───────────────────────
section "FMRT_4 · POST /api/Task — Invalid: 0 minutes (expect 400)"
http_call POST "/Task" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"Bad Task","estimatedMinutes":0,"frequency":0,"type":0}'
if [ "$HTTP_CODE" = "400" ]; then
  ok "Validation correctly rejected 0 minutes (HTTP 400)"
else
  fail "Expected 400 for 0 minutes, got HTTP $HTTP_CODE"
fi

# ── FMRT_10/11 — Create more tasks with different frequencies/types ───────────
section "FMRT_10/11 · POST /api/Task — Monthly + Urgent task"
http_call POST "/Task" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"Monthly Budget Review","description":"Track spending and savings","estimatedMinutes":60,"frequency":1,"type":3}'

if [ "$HTTP_CODE" = "201" ]; then
  TASK_MONTHLY_ID=$(json_field "$BODY" "id")
  FREQ=$(json_field "$BODY" "frequencyLabel")
  TYPE=$(json_field "$BODY" "typeLabel")
  ok "Monthly/Urgent task created (ID: $TASK_MONTHLY_ID | $FREQ | $TYPE)"
else
  fail "Create monthly task failed (HTTP $HTTP_CODE)"
fi

section "FMRT_10 · POST /api/Task — Yearly + Normal task"
http_call POST "/Task" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"Annual Health Checkup","estimatedMinutes":120,"frequency":2,"type":0}'
if [ "$HTTP_CODE" = "201" ]; then
  TASK_YEARLY_ID=$(json_field "$BODY" "id")
  FREQ=$(json_field "$BODY" "frequencyLabel")
  ok "Yearly task created (ID: $TASK_YEARLY_ID | $FREQ)"
else
  fail "Create yearly task failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_4 — List tasks ───────────────────────────────────────────────────────
section "FMRT_4 · GET /api/Task — List all user tasks"
http_call GET "/Task" -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "200" ]; then
  ok "Task list returned (HTTP 200)"
else
  fail "List tasks failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_4 — Get by ID ───────────────────────────────────────────────────────
section "FMRT_4 · GET /api/Task/$TASK_ID — Get task by ID"
http_call GET "/Task/$TASK_ID" -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "200" ]; then
  TITLE=$(json_field "$BODY" "title")
  ok "Task retrieved: '$TITLE' (HTTP 200)"
else
  fail "Get task by ID failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_5 — Update task ──────────────────────────────────────────────────────
section "FMRT_5 · PUT /api/Task/$TASK_ID — Update title and type"
http_call PUT "/Task/$TASK_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"Read Two Book Chapters","type":3}'

if [ "$HTTP_CODE" = "200" ]; then
  UPDATED_TITLE=$(json_field "$BODY" "title")
  UPDATED_TYPE=$(json_field "$BODY" "typeLabel")
  ok "Task updated (HTTP 200)"
  ok "New title: '$UPDATED_TITLE' | New type: $UPDATED_TYPE"
else
  fail "Update task failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_5 — Cannot access another user's task ───────────────────────────────
section "FMRT_5 · GET /api/Task/999999 — Non-existent task (expect 404)"
http_call GET "/Task/999999" -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "404" ]; then
  ok "Non-existent task returns 404 (HTTP 404)"
else
  fail "Expected 404, got HTTP $HTTP_CODE"
fi

# ── FMRT_6 — Delete task ──────────────────────────────────────────────────────
section "FMRT_6 · DELETE /api/Task/$TASK_ID — Delete task"
http_call DELETE "/Task/$TASK_ID" -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "204" ]; then
  ok "Task deleted (HTTP 204)"
else
  fail "Delete task failed (HTTP $HTTP_CODE)"
fi

section "FMRT_6 · GET /api/Task/$TASK_ID — Should be 404 after delete"
http_call GET "/Task/$TASK_ID" -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "404" ]; then
  ok "Deleted task correctly returns 404"
else
  fail "Expected 404 after delete, got HTTP $HTTP_CODE"
fi

# ── Cleanup ───────────────────────────────────────────────────────────────────
section "Cleanup · Delete remaining tasks and user"
for TID in "$TASK_MONTHLY_ID" "$TASK_YEARLY_ID"; do
  if [ -n "$TID" ]; then
    http_call DELETE "/Task/$TID" -H "Authorization: Bearer $TOKEN"
    [ "$HTTP_CODE" = "204" ] && ok "Deleted task $TID" || fail "Failed to delete task $TID"
  fi
done

http_call DELETE "/User/$USER_ID" -H "Authorization: Bearer $TOKEN"
[ "$HTTP_CODE" = "204" ] && ok "Test user deleted" || fail "Cleanup user failed (HTTP $HTTP_CODE)"

footer
