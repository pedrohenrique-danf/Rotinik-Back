mostrar estrutura de pastas:
```sh
eza --tree -I "artifacts|bin|obj"
```

mostrar `src/`:
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

mostrar `test/`:
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

mostrar tudo:
```sh
set base_dir "."

set target_files (string match -v -r '/(Migrations|artifacts|obj|bin|docs)/' $base_dir/**/*.cs $base_dir/**/*.json)

for file in $target_files
    if test -f "$file"
        printf "File: %s\n" "$file"
        
        set ext (string split -r -m1 . "$file")[-1]
        
        if test "$ext" = "cs"
            printf '```csharp\n'
        else if test "$ext" = "json"
            printf '```json\n'
        else
            printf '
```text\n'
        end
        
        cat "$file"
        printf '\n```\n\n'
    end
end
```