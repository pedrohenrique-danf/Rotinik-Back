#!/usr/bin/env fish

function get_markdown_lang -a ext
    switch "$ext"
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
    
    set ext (string split -r -m1 . "$file")[-1]
    set lang (get_markdown_lang "$ext")
    
    printf "```%s\n" "$lang"
    cat "$file"
    printf "\n```\n\n"
end

function print_all_code -a target_dir
    set target_files (string match -v -r '/(Migrations|artifacts|obj|bin|docs|\.git)/' $target_dir/**/*)

    for file in $target_files
        if test -f "$file"
            print_file_content "$file"
        end
    end
end

function main
    set base_dir "."
    if test (count $argv) -gt 0
        set base_dir $argv[1]
    end

    if not test -d "$base_dir"
        echo "Error: The directory '$base_dir' does not exist."
        exit 1
    end

    print_all_code "$base_dir"
end

main $argv