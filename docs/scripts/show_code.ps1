param (
    [string]$BaseDir = "."
)

function Get-MarkdownLang {
    param([string]$Ext)
    
    $Ext = $Ext.TrimStart('.').ToLower()
    
    switch ($Ext) {
        'cs' { return 'csharp' }
        'json' { return 'json' }
        { $_ -in 'md', 'markdown' } { return 'markdown' }
        { $_ -in 'sh', 'fish', 'ps1', 'bat', 'cmd' } { return 'bash' }
        default { return 'text' }
    }
}

function Print-FileContent {
    param([System.IO.FileInfo]$File)
    
    Write-Output "File: $($File.FullName)"
    
    $lang = Get-MarkdownLang -Ext $File.Extension
    
    Write-Output ('```' + $lang)
    
    Get-Content -Path $File.FullName -Raw | Write-Output
    
    Write-Output ('
```' + [Environment]::NewLine)
}

function Print-AllCode {
    param([string]$TargetDir)
    
    $excludePattern = '\\(Migrations|artifacts|obj|bin|docs|\.git)(\\|$)'

    $files = Get-ChildItem -Path $TargetDir -Recurse -File | 
             Where-Object { $_.FullName -notmatch $excludePattern }

    foreach ($file in $files) {
        Print-FileContent -File $file
    }
}

function Main {
    if (-not (Test-Path -Path $BaseDir -PathType Container)) {
        Write-Host "Error: The directory '$BaseDir' does not exist." -ForegroundColor Red
        exit 1
    }

    Print-AllCode -TargetDir $BaseDir
}

Main