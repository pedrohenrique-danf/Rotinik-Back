# Checklist de Migração para Produção - Rotinik

Este documento resume as alterações críticas necessárias na camada `Core` antes de implantar a aplicação em ambiente de produção.

---

## 1. Segurança de CORS (Cross-Origin Resource Sharing)
* **Onde está:** `ApplicationBuilderExtensions.cs` -> `app.UseCors("AllowAll");`
* **O que mudar:** Atualmente, qualquer site pode fazer requisições para a sua API. Em produção, isso expõe o sistema a ataques.
* **Ação:** * Configure uma política restritiva no `ServiceCollectionExtensions.cs` que leia os domínios permitidos (ex: seu frontend) a partir do `appsettings.Production.json`.
  * Só use o `"AllowAll"` se o ambiente for de Desenvolvimento.

## 2. Ciclo de Vida do Banco de Dados
* **Onde está:** `ApplicationBuilderExtensions.cs` -> `db.Database.EnsureCreated();`
* **O que mudar:** O `EnsureCreated` ignora as migrações do Entity Framework Core e cria o banco direto. Se você alterar o modelo e rodar isso em produção, ele não conseguirá atualizar o banco sem apagar os dados existentes.
* **Ação:**
  * Substitua por `db.Database.Migrate();` para que o EF Core aplique as novas migrações incrementalmente e de forma segura.

## 3. Proteção da Chave Secreta do JWT
* **Onde está:** `ServiceCollectionExtensions.cs` -> `var jwtSecret = jwtSettings?.Secret ?? "TemporaryKeySoEFCoreMigrationDoesNotBreak!";`
* **O que mudar:** Se houver qualquer falha ao carregar as configurações em produção, o sistema usará essa string estática como chave secreta, tornando seus tokens JWT fáceis de serem forjados.
* **Ação:**
  * Remova o fallback ou adicione uma verificação rigorosa: se `jwtSettings.Secret` for nulo ou vazio em produção, force a aplicação a estourar uma exceção e travar a inicialização (*Fail-Fast*). Injete a chave real via Variáveis de Ambiente do servidor.

## 4. Isolamento de Ambientes (Scalar e OpenAPI)
* **Onde está:** `ApplicationBuilderExtensions.cs` -> Bloco `if (app.Environment.IsDevelopment())`
* **O que mudar:** Garantir que ferramentas de teste e documentação não subam para produção.
* **Ação:**
  * A estrutura atual já está correta (protegida pelo `IsDevelopment`). Apenas certifique-se de que a variável de ambiente `ASPNETCORE_ENVIRONMENT` esteja estritamente configurada como `Production` no seu servidor de hospedagem.

## 5. Centralização de Logs e Monitoramento
* **Onde está:** `GlobalExceptionMiddleware.cs` -> `_logger.LogError(...)`
* **O que mudar:** Logs salvos apenas no console do container/servidor somem facilmente e são difíceis de analisar em escala.
* **Ação:**
  * Configure um agregador de logs estruturados (como Serilog integrado ao Seq, Application Insights, Datadog ou AWS CloudWatch) para monitorar erros 500 em tempo real e gerar alertas automatizados.