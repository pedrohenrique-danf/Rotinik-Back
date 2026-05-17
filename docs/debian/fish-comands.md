# Scripts de Apoio - fish 🐟

## Preparação

```bash
chmod +x *.fish
```

## Mostrar Arvore de Pastas
Exibe a árvore de diretórios do projeto, (ignora automaticamente pastas de build e configurações como bin, obj, .git, etc).

```bash
# Mostra a árvore da pasta atual
fish docs/scripts/show_tree.fish

# Mostra a árvore de uma pasta específica (ex: src)
fish docs/scripts/show_tree.fish src
```

## Mostrar Código por Arquivo
Lê os arquivos do diretório escolhido e os exibe no terminal formatados em blocos de Markdown.

```bash
# Extrai todo o código da pasta atual
fish docs/scripts/show_code.fish

# Extrai o código de uma pasta específica (ex: src)
fish docs/scripts/show_code.fish src
```
