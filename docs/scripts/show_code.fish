#!/usr/bin/fish

function get_markdown_lang -a ext
    switch "$ext"
        case ts
            echo "typescript"
        case js
            echo "javascript"
        case html
            echo "html"
        case css
            echo "css"
        case scss
            echo "scss"
        case cs
            echo "csharp"
        case json
            echo "json"
        case md markdown
            echo "markdown"
        case sh fish
            echo "bash"
        case '*'
            echo "text"
    end
end

function print_file_content -a file
    printf "File: %s\n" "$file"
    
    set filename (basename "$file")
    set ext ""
    
    if string match -q "*.*" "$filename"
        set ext (string split -r -m1 . "$filename")[-1]
    end
    
    set lang (get_markdown_lang "$ext")
    
    printf "```%s\n" "$lang"
    cat "$file"
    printf "\n
```\n\n"
end

function process_target -a target
    if test -f "$target"
        print_file_content "$target"
        
    else if test -d "$target"
        set target_files (string match -v -r '/(Migrations|artifacts|obj|bin|docs|\.git|node_modules)/' $target/**/*)

        for file in $target_files
            if test -f "$file"
                print_file_content "$file"
            end
        end
    else
        echo "Error: The path '$target' does not exist or is invalid." >&2
    end
end

function main
    if test (count $argv) -eq 0
        process_target "."
    else
        for target in $argv
            process_target "$target"
        end
    end
end

main $argv