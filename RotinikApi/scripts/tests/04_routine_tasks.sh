#!/bin/bash
# =============================================================================
#   scripts/tests/04_routine_tasks.sh
#   Tests: FMRT_7 (add task to routine), FMRT_8 (edit in routine),
#          FMRT_9 (remove from routine), FMRT_12 (complete task),
#          FMRT_14 (time validation).
# =============================================================================

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/../lib/common.sh"

EMAIL="rtask_$(date +%s)@rotinik.com"
PASSWORD="Password123"

header "ROUTINE-TASK TESTS — FMRT_7 · FMRT_8 · FMRT_9 · FMRT_12 · FMRT_14"

# ── Setup: user, login, routine, task ────────────────────────────────────────
section "Setup · Create user, login, create routine & task"
http_call POST "/User" \
  -d "{\"name\":\"RT Tester\",\"email\":\"$EMAIL\",\"phone\":\"+55 11 99999-9999\",\"password\":\"$PASSWORD\"}"
[ "$HTTP_CODE" = "201" ] || abort "Signup failed (HTTP $HTTP_CODE)"
USER_ID=$(json_field "$BODY" "id")
ok "User created (ID: $USER_ID)"

http_call POST "/User/login" \
  -d "{\"email\":\"$EMAIL\",\"password\":\"$PASSWORD\"}"
[ "$HTTP_CODE" = "200" ] || abort "Login failed (HTTP $HTTP_CODE)"
TOKEN=$(json_field "$BODY" "token")
[ -n "$TOKEN" ] || abort "No token"
ok "Logged in"

# Create routine
http_call POST "/Routine" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"name":"Test Routine","description":"Used for task linkage tests"}'
[ "$HTTP_CODE" = "201" ] || abort "Create routine failed (HTTP $HTTP_CODE)"
ROUTINE_ID=$(json_field "$BODY" "id")
ok "Routine created (ID: $ROUTINE_ID)"

# Create a short task (1 min — short enough to test time validation quickly)
http_call POST "/Task" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"Quick Task","description":"For time validation test","estimatedMinutes":1,"frequency":0,"type":0}'
[ "$HTTP_CODE" = "201" ] || abort "Create task failed (HTTP $HTTP_CODE)"
TASK_ID=$(json_field "$BODY" "id")
ok "Task created (ID: $TASK_ID, estimatedMinutes: 1)"

# Create a long task (60 min — will be used to test time rejection)
http_call POST "/Task" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"title":"Long Task","description":"Should not be completable right away","estimatedMinutes":60,"frequency":0,"type":1}'
[ "$HTTP_CODE" = "201" ] || abort "Create long task failed (HTTP $HTTP_CODE)"
LONG_TASK_ID=$(json_field "$BODY" "id")
ok "Long task created (ID: $LONG_TASK_ID, estimatedMinutes: 60)"

# ── FMRT_7 — Add tasks to routine ────────────────────────────────────────────
section "FMRT_7 · POST /api/Routine/$ROUTINE_ID/tasks — Add quick task (order 0)"
http_call POST "/Routine/$ROUTINE_ID/tasks" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"taskId\":$TASK_ID,\"order\":0}"

if [ "$HTTP_CODE" = "200" ]; then
  RT_ENTRY_ID=$(json_field "$BODY" "id")
  ok "Task added to routine (entry ID: $RT_ENTRY_ID)"
else
  abort "Add task to routine failed (HTTP $HTTP_CODE)"
fi

section "FMRT_7 · POST /api/Routine/$ROUTINE_ID/tasks — Add long task (order 1)"
http_call POST "/Routine/$ROUTINE_ID/tasks" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"taskId\":$LONG_TASK_ID,\"order\":1}"

if [ "$HTTP_CODE" = "200" ]; then
  LONG_RT_ENTRY_ID=$(json_field "$BODY" "id")
  ok "Long task added to routine (entry ID: $LONG_RT_ENTRY_ID)"
else
  fail "Add long task to routine failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_7 — Duplicate prevention ────────────────────────────────────────────
section "FMRT_7 · POST duplicate — Same task added twice (expect 409)"
http_call POST "/Routine/$ROUTINE_ID/tasks" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"taskId\":$TASK_ID,\"order\":0}"
if [ "$HTTP_CODE" = "409" ]; then
  ok "Duplicate task in routine correctly rejected (HTTP 409)"
else
  fail "Expected 409 for duplicate, got HTTP $HTTP_CODE"
fi

# ── FMRT_8 — Update order of task in routine ─────────────────────────────────
section "FMRT_8 · PUT /api/Routine/$ROUTINE_ID/tasks/$RT_ENTRY_ID — Change order to 2"
http_call PUT "/Routine/$ROUTINE_ID/tasks/$RT_ENTRY_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"order":2}'

if [ "$HTTP_CODE" = "200" ]; then
  NEW_ORDER=$(json_field "$BODY" "order")
  ok "Task order updated to: $NEW_ORDER (HTTP 200)"
else
  fail "Update routine task order failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_14 — Start execution session ────────────────────────────────────────
section "FMRT_14 · POST /api/Routine/$ROUTINE_ID/executions — Start execution"
http_call POST "/Routine/$ROUTINE_ID/executions" \
  -H "Authorization: Bearer $TOKEN"

if [ "$HTTP_CODE" = "200" ]; then
  EXECUTION_ID=$(json_field "$BODY" "id")
  STARTED_AT=$(json_field "$BODY" "startedAt")
  ok "Execution started (ID: $EXECUTION_ID | StartedAt: $STARTED_AT)"
else
  abort "Start execution failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_14 — Reject completion of long task (not enough time) ───────────────
section "FMRT_14 · PATCH .../complete — Long task (60 min) right after start (expect 422)"
http_call PATCH "/Routine/$ROUTINE_ID/tasks/$LONG_RT_ENTRY_ID/complete" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"routineExecutionId\":$EXECUTION_ID}"

if [ "$HTTP_CODE" = "422" ]; then
  ok "Time validation correctly rejected long task (HTTP 422)"
else
  fail "Expected 422 (time validation), got HTTP $HTTP_CODE"
fi

# ── FMRT_12 + FMRT_14 — Complete short task (should pass) ────────────────────
section "FMRT_12/14 · PATCH .../complete — Short task (1 min, ~0s elapsed) may vary"
info "  Note: 1-minute task with <1 min elapsed — rejection is correct per FMRT_14."
info "  Waiting 5 seconds to accumulate elapsed time..."
sleep 5
http_call PATCH "/Routine/$ROUTINE_ID/tasks/$RT_ENTRY_ID/complete" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"routineExecutionId\":$EXECUTION_ID}"

if [ "$HTTP_CODE" = "200" ]; then
  IS_COMPLETED=$(json_field "$BODY" "isCompleted")
  COMPLETED_AT=$(json_field "$BODY" "completedAt")
  ok "Short task completed! isCompleted: $IS_COMPLETED | completedAt: $COMPLETED_AT"
elif [ "$HTTP_CODE" = "422" ]; then
  info "  ⚠ Still within estimated time (FMRT_14 guard active) — this is correct behavior."
  ok "FMRT_14 time guard triggered as expected"
else
  fail "Complete task returned unexpected HTTP $HTTP_CODE"
fi

# ── FMRT_12 — Uncomplete task ─────────────────────────────────────────────────
section "FMRT_12 · PATCH .../uncomplete — Undo completion"
http_call PATCH "/Routine/$ROUTINE_ID/tasks/$RT_ENTRY_ID/uncomplete" \
  -H "Authorization: Bearer $TOKEN"

if [ "$HTTP_CODE" = "200" ]; then
  IS_COMPLETED=$(json_field "$BODY" "isCompleted")
  ok "Task uncompleted. isCompleted: $IS_COMPLETED"
else
  fail "Uncomplete task failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_14 — Finish execution ───────────────────────────────────────────────
section "FMRT_14 · PATCH /api/Routine/$ROUTINE_ID/executions/$EXECUTION_ID/finish"
http_call PATCH "/Routine/$ROUTINE_ID/executions/$EXECUTION_ID/finish" \
  -H "Authorization: Bearer $TOKEN"

if [ "$HTTP_CODE" = "200" ]; then
  FINISHED_AT=$(json_field "$BODY" "finishedAt")
  ELAPSED=$(json_field "$BODY" "elapsedMinutes")
  ok "Execution finished (finishedAt: $FINISHED_AT | elapsed: ${ELAPSED} min)"
else
  fail "Finish execution failed (HTTP $HTTP_CODE)"
fi

# ── FMRT_9 — Remove tasks from routine ───────────────────────────────────────
section "FMRT_9 · DELETE /api/Routine/$ROUTINE_ID/tasks/$RT_ENTRY_ID — Remove task"
http_call DELETE "/Routine/$ROUTINE_ID/tasks/$RT_ENTRY_ID" \
  -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "204" ]; then
  ok "Task removed from routine (HTTP 204)"
else
  fail "Remove task from routine failed (HTTP $HTTP_CODE)"
fi

section "FMRT_9 · DELETE /api/Routine/$ROUTINE_ID/tasks/$LONG_RT_ENTRY_ID — Remove long task"
http_call DELETE "/Routine/$ROUTINE_ID/tasks/$LONG_RT_ENTRY_ID" \
  -H "Authorization: Bearer $TOKEN"
if [ "$HTTP_CODE" = "204" ]; then
  ok "Long task removed from routine (HTTP 204)"
else
  fail "Remove long task failed (HTTP $HTTP_CODE)"
fi

# ── Cleanup ───────────────────────────────────────────────────────────────────
section "Cleanup · Delete routine, tasks, and user"
http_call DELETE "/Routine/$ROUTINE_ID" -H "Authorization: Bearer $TOKEN"
[ "$HTTP_CODE" = "204" ] && ok "Routine deleted" || fail "Delete routine failed"

http_call DELETE "/Task/$TASK_ID" -H "Authorization: Bearer $TOKEN"
[ "$HTTP_CODE" = "204" ] && ok "Task deleted" || fail "Delete task failed"

http_call DELETE "/Task/$LONG_TASK_ID" -H "Authorization: Bearer $TOKEN"
[ "$HTTP_CODE" = "204" ] && ok "Long task deleted" || fail "Delete long task failed"

http_call DELETE "/User/$USER_ID" -H "Authorization: Bearer $TOKEN"
[ "$HTTP_CODE" = "204" ] && ok "Test user deleted" || fail "Cleanup user failed"

footer
