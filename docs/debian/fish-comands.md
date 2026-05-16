mostrar estrutura de pastas:
```sh
eza --tree -I "artifacts|bin|obj"
```

mostrar código fonte (`src/`) completo:
```sh
set base_dir "src"
set target_files (string match -v "*Migrations*" $base_dir/**/*.cs $base_dir/**/*.json)

for file in $target_files
    if test -f "$file"
        printf "File: %s\n" "$file"
        printf '```csharp\n'
        cat "$file"
        printf '\n```\n'
    end
end
```

mostrar código dos testes (`test/`) completo:
```sh
set base_dir "tests"

for file in $base_dir/**/*
    if test -f "$file"
        printf "File: %s\n" "$file"
        printf '```csharp\n'
        cat "$file"
        printf '\n```\n'
    end
end
```
