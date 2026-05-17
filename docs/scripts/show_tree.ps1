param (
    [string]$Path = "."
)

# Padrão de pastas a serem ignoradas
$excludePattern = "^(Migrations|artifacts|obj|bin|docs|\.git)$"

function Print-Tree {
    param (
        [string]$CurrentPath,
        [string]$Prefix = ""
    )

    # Busca arquivos e pastas, filtrando os que não queremos
    $items = Get-ChildItem -Path $CurrentPath -ErrorAction SilentlyContinue | Where-Object { $_.Name -notmatch $excludePattern }
    $total = $items.Count
    $currentIndex = 0

    foreach ($item in $items) {
        $currentIndex++
        $isLast = ($currentIndex -eq $total)

        if ($isLast) {
            Write-Output "${Prefix}└── $($item.Name)"
            if ($item.PSIsContainer) {
                Print-Tree -CurrentPath $item.FullName -Prefix "${Prefix}    "
            }
        } else {
            Write-Output "${Prefix}├── $($item.Name)"
            if ($item.PSIsContainer) {
                Print-Tree -CurrentPath $item.FullName -Prefix "${Prefix}│   "
            }
        }
    }
}

# Resolve o caminho para exibir o diretório raiz corretamente
$resolvedPath = (Resolve-Path -Path $Path).Path
Write-Output $resolvedPath
Print-Tree -CurrentPath $Path