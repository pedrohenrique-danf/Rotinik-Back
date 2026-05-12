#!/bin/bash

# ============================================================
#   ROTINIK API — SCRIPT DE TESTES AUTOMATIZADOS
#   Cobre: Cadastro, Login (JWT), GET /me, DELETE autenticado
# ============================================================

API_URL="http://localhost:5025/api"
EMAIL="testuser_$(date +%s)@rotinik.com"
SENHA="Password123"

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

ok()   { echo -e "${GREEN}  ✓ $1${NC}"; }
fail() { echo -e "${RED}  ✗ $1${NC}"; }
info() { echo -e "${CYAN}$1${NC}"; }

# jq para pretty-print, ou cat como fallback
JQ_CMD="cat"
command -v jq &> /dev/null && JQ_CMD="jq ."

echo ""
echo -e "${YELLOW}============================================================${NC}"
echo -e "${YELLOW}       TESTES AUTOMATIZADOS — ROTINIK API                   ${NC}"
echo -e "${YELLOW}============================================================${NC}"
echo ""

# ------------------------------------------------------------------
# 1. CADASTRO DE USUÁRIO
# ------------------------------------------------------------------
info "[1/5] Criando usuário de teste (POST /api/usuario)..."
CREATE_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/Usuario" \
  -H "Content-Type: application/json" \
  -d "{\"nome\": \"Teste Bash JWT\", \"email\": \"$EMAIL\", \"telefone\": \"+55 11 99999-9999\", \"senha\": \"$SENHA\"}")

HTTP_CODE=$(echo "$CREATE_RESPONSE" | tail -1)
BODY=$(echo "$CREATE_RESPONSE" | head -n -1)
echo "$BODY" | $JQ_CMD

if [ "$HTTP_CODE" = "201" ]; then
  ok "Usuário criado com sucesso (HTTP $HTTP_CODE)"
  USER_ID=$(echo "$BODY" | grep -o '"id":[0-9]*' | grep -o '[0-9]*')
else
  fail "Falha ao criar usuário (HTTP $HTTP_CODE)"
  echo "Abortando testes."; exit 1
fi

# ------------------------------------------------------------------
# 2. TENTATIVA DE ACESSAR ROTA PROTEGIDA SEM TOKEN (deve ser 401)
# ------------------------------------------------------------------
info "\n[2/5] Testando rota protegida SEM token (GET /api/usuario) — esperado: 401..."
STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X GET "$API_URL/Usuario")
if [ "$STATUS" = "401" ]; then
  ok "Bloqueado corretamente sem token (HTTP 401)"
else
  fail "PROBLEMA: Rota deveria ser protegida, mas retornou HTTP $STATUS"
fi

# ------------------------------------------------------------------
# 3. LOGIN E CAPTURA DO TOKEN JWT
# ------------------------------------------------------------------
info "\n[3/5] Fazendo login e capturando token JWT (POST /api/usuario/login)..."
LOGIN_RESPONSE=$(curl -s -X POST "$API_URL/Usuario/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\": \"$EMAIL\", \"senha\": \"$SENHA\"}")

echo "$LOGIN_RESPONSE" | $JQ_CMD

TOKEN=$(echo "$LOGIN_RESPONSE" | grep -o '"token":"[^"]*"' | sed 's/"token":"//;s/"//')

if [ -n "$TOKEN" ]; then
  ok "Token JWT capturado! (${TOKEN:0:40}...)"
else
  fail "Falha ao capturar token JWT. Abortando."
  exit 1
fi

# ------------------------------------------------------------------
# 4. ACESSAR ROTA /ME COM TOKEN
# ------------------------------------------------------------------
info "\n[4/5] Acessando rota protegida /me com token (GET /api/usuario/me)..."
ME_RESPONSE=$(curl -s -w "\n%{http_code}" -X GET "$API_URL/Usuario/me" \
  -H "Authorization: Bearer $TOKEN")

HTTP_CODE=$(echo "$ME_RESPONSE" | tail -1)
BODY=$(echo "$ME_RESPONSE" | head -n -1)
echo "$BODY" | $JQ_CMD

if [ "$HTTP_CODE" = "200" ]; then
  ok "Dados do usuário autenticado retornados com sucesso (HTTP $HTTP_CODE)"
else
  fail "Falha ao acessar /me (HTTP $HTTP_CODE)"
fi

# ------------------------------------------------------------------
# 5. EXCLUIR USUÁRIO COM TOKEN (limpeza)
# ------------------------------------------------------------------
info "\n[5/5] Excluindo usuário de teste (DELETE /api/usuario/$USER_ID) com autenticação..."
DEL_STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X DELETE "$API_URL/Usuario/$USER_ID" \
  -H "Authorization: Bearer $TOKEN")

if [ "$DEL_STATUS" = "204" ]; then
  ok "Usuário excluído com sucesso (HTTP $DEL_STATUS)"
else
  fail "Falha ao excluir usuário (HTTP $DEL_STATUS)"
fi

echo ""
echo -e "${YELLOW}============================================================${NC}"
echo -e "${YELLOW}               TESTES CONCLUÍDOS                           ${NC}"
echo -e "${YELLOW}============================================================${NC}"
echo ""

