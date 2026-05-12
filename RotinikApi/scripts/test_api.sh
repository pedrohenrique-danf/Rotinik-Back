#!/bin/bash

# ============================================================
#   ROTINIK API — AUTOMATED TEST SCRIPT
#   Covers: Signup, Login (JWT), GET /me, Authenticated DELETE
# ============================================================

API_URL="http://localhost:5025/api"
EMAIL="testuser_$(date +%s)@rotinik.com"
PASSWORD="Password123"

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

ok()   { echo -e "${GREEN}  ✓ $1${NC}"; }
fail() { echo -e "${RED}  ✗ $1${NC}"; }
info() { echo -e "${CYAN}$1${NC}"; }

# jq for pretty-print, or cat as fallback
JQ_CMD="cat"
command -v jq &> /dev/null && JQ_CMD="jq ."

echo ""
echo -e "${YELLOW}============================================================${NC}"
echo -e "${YELLOW}       AUTOMATED TESTS — ROTINIK API                        ${NC}"
echo -e "${YELLOW}============================================================${NC}"
echo ""

# ------------------------------------------------------------------
# 1. USER SIGNUP
# ------------------------------------------------------------------
info "[1/5] Creating test user (POST /api/User)..."
CREATE_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/User" \
  -H "Content-Type: application/json" \
  -d "{\"name\": \"Test Bash JWT\", \"email\": \"$EMAIL\", \"phone\": \"+55 11 99999-9999\", \"password\": \"$PASSWORD\"}")

HTTP_CODE=$(echo "$CREATE_RESPONSE" | tail -1)
BODY=$(echo "$CREATE_RESPONSE" | head -n -1)
echo "$BODY" | $JQ_CMD

if [ "$HTTP_CODE" = "201" ]; then
  ok "User created successfully (HTTP $HTTP_CODE)"
  USER_ID=$(echo "$BODY" | grep -o '"id":[0-9]*' | grep -o '[0-9]*')
else
  fail "Failed to create user (HTTP $HTTP_CODE)"
  echo "Aborting tests."; exit 1
fi

# ------------------------------------------------------------------
# 2. ATTEMPT TO ACCESS PROTECTED ROUTE WITHOUT TOKEN (should be 401)
# ------------------------------------------------------------------
info "\n[2/5] Testing protected route WITHOUT token (GET /api/User) — expected: 401..."
STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X GET "$API_URL/User")
if [ "$STATUS" = "401" ]; then
  ok "Blocked correctly without token (HTTP 401)"
else
  fail "PROBLEM: Route should be protected, but returned HTTP $STATUS"
fi

# ------------------------------------------------------------------
# 3. LOGIN AND CAPTURE JWT TOKEN
# ------------------------------------------------------------------
info "\n[3/5] Logging in and capturing JWT token (POST /api/User/login)..."
LOGIN_RESPONSE=$(curl -s -X POST "$API_URL/User/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\": \"$EMAIL\", \"password\": \"$PASSWORD\"}")

echo "$LOGIN_RESPONSE" | $JQ_CMD

TOKEN=$(echo "$LOGIN_RESPONSE" | grep -o '"token":"[^"]*"' | sed 's/"token":"//;s/"//')

if [ -n "$TOKEN" ]; then
  ok "JWT token captured! (${TOKEN:0:40}...)"
else
  fail "Failed to capture JWT token. Aborting."
  exit 1
fi

# ------------------------------------------------------------------
# 4. ACCESS /ME ROUTE WITH TOKEN
# ------------------------------------------------------------------
info "\n[4/5] Accessing protected route /me with token (GET /api/User/me)..."
ME_RESPONSE=$(curl -s -w "\n%{http_code}" -X GET "$API_URL/User/me" \
  -H "Authorization: Bearer $TOKEN")

HTTP_CODE=$(echo "$ME_RESPONSE" | tail -1)
BODY=$(echo "$ME_RESPONSE" | head -n -1)
echo "$BODY" | $JQ_CMD

if [ "$HTTP_CODE" = "200" ]; then
  ok "Authenticated user data returned successfully (HTTP $HTTP_CODE)"
else
  fail "Failed to access /me (HTTP $HTTP_CODE)"
fi

# ------------------------------------------------------------------
# 5. DELETE USER WITH TOKEN (cleanup)
# ------------------------------------------------------------------
info "\n[5/5] Deleting test user (DELETE /api/User/$USER_ID) with authentication..."
DEL_STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X DELETE "$API_URL/User/$USER_ID" \
  -H "Authorization: Bearer $TOKEN")

if [ "$DEL_STATUS" = "204" ]; then
  ok "User deleted successfully (HTTP $DEL_STATUS)"
else
  fail "Failed to delete user (HTTP $DEL_STATUS)"
fi

echo ""
echo -e "${YELLOW}============================================================${NC}"
echo -e "${YELLOW}               TESTS COMPLETED                             ${NC}"
echo -e "${YELLOW}============================================================${NC}"
echo ""

