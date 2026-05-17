# Scripts de Apoio - PowerShell 🪟

## Preparação

No Windows, pode ser necessário liberar a permissão de execução de scripts no seu usuário caso seja a primeira vez.

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

## Mostrar Arvore de Pastas

Exibe a árvore de diretórios do projeto, ignorando automaticamente pastas de build e configurações (como bin, obj, .git, etc).

Como usar:

```powershell
# Mostra a árvore da pasta atual
.\docs\scripts\show_tree.ps1

# Mostra a árvore de uma pasta específica (ex: src)
.\docs\scripts\show_tree.ps1 src
```

## Mostrar Código por Arquivo

Lê os arquivos do diretório escolhido e os exibe no terminal formatados em blocos de Markdown, com o syntax highlighting correto. Ideal para copiar e colar em documentações ou enviar para IAs.

Como usar:

```powershell
# Extrai todo o código da pasta atual
.\docs\scripts\show_code.ps1

# Extrai o código de uma pasta específica (ex: src)
.\docs\scripts\show_code.ps1 src
```
