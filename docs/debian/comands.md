```sh
eza --tree -I "artifacts|bin|obj"
```

```sh
set base_dir "src"

# 1. Pega TODOS os .cs e .json recursivamente, mas ignora a pasta Migrations
set target_files (string match -v "*Migrations*" $base_dir/**/*.cs $base_dir/**/*.json)

# 2. Itera sobre a lista limpa
for file in $target_files
    if test -f "$file"
        set_color cyan
        printf "\n\nFile: %s\n\n" "$file"
        set_color normal
        
        cat "$file"
    end
end
```
