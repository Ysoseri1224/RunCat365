param([string]$OutputDirectory = (Join-Path $PSScriptRoot 'artifacts\RunCat-Local-x64'))
$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet publish RunCat365/RunCat365.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false -o $OutputDirectory --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Local publication failed.' }
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'LICENSE') -Destination $OutputDirectory
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'README.md') -Destination $OutputDirectory
    Write-Output (Join-Path $OutputDirectory 'RunCat Local.exe')
}
finally {
    Pop-Location
}
