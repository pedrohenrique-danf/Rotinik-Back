#!/usr/bin/env fish

set dir "."
if test (count $argv) -gt 0
    set dir $argv[1]
end

eza --tree -I "artifacts|bin|obj|.git|Migrations" $dir