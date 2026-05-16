#!/usr/bin/fish

set base_dir "src"
# 1. Define o vetor (array) de caminhos e arquivos
set target_files \
    $base_dir/Controllers/*.cs \
    $base_dir/Data/*.cs \
    $base_dir/DTOs/*.cs \
    $base_dir/Models/*.cs \
    $base_dir/Properties/* \
    $base_dir/appsettings.Development.json \
    $base_dir/Program.cs \

# 2. Itera sobre o vetor
for file in $target_files
    # Combina os echos para deixar o código mais curto
    set_color cyan
    echo -e "\n\nFile: $file"
    set_color normal
    
    cat $file
end