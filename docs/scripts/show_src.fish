#!/usr/bin/fish

set base_dir "src"
set target_files (string match -v "*Migrations*" $base_dir/**/*.cs $base_dir/**/*.json)

for file in $target_files
    if test -f "$file"
        printf "\nFile: %s\n" "$file"
        printf '```csharp\n'
        cat "$file"
        printf '\n```\n'
    end
end