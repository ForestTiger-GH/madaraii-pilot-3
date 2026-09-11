[CmdletBinding()]
param(
    [switch]$Clean
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$buildRoot = Join-Path $root '.build'
$artifacts = Join-Path $root 'artifacts'
$publish = Join-Path $artifacts 'publish'
$package = Join-Path $artifacts 'MiniDoc-0.1.0'

if ($Clean) {
    Remove-Item $buildRoot -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item $artifacts -Recurse -Force -ErrorAction SilentlyContinue
}

New-Item -ItemType Directory -Force -Path $buildRoot, $artifacts | Out-Null
$env:DOTNET_CLI_HOME = Join-Path $buildRoot 'dotnet-home'
$env:NUGET_PACKAGES = Join-Path $buildRoot 'nuget-packages'
$env:TEMP = Join-Path $buildRoot 'temp'
$env:TMP = $env:TEMP
New-Item -ItemType Directory -Force -Path $env:DOTNET_CLI_HOME, $env:NUGET_PACKAGES, $env:TEMP | Out-Null

$project = Join-Path $root 'src\MiniDoc\MiniDoc.csproj'
$checks = Join-Path $root 'tests\MiniDoc.Checks\MiniDoc.Checks.csproj'

& dotnet build $checks -c Release --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }

& dotnet run --project $checks -c Release --no-build
if ($LASTEXITCODE -ne 0) { throw 'Semantic checks failed.' }

Remove-Item $publish -Recurse -Force -ErrorAction SilentlyContinue
& dotnet publish $project -c Release -r win-x64 --self-contained true --nologo -o $publish
if ($LASTEXITCODE -ne 0) { throw 'Publish failed.' }
if (-not (Test-Path (Join-Path $publish 'MiniDoc.exe'))) { throw 'Publish did not produce MiniDoc.exe.' }

Remove-Item $package -Recurse -Force -ErrorAction SilentlyContinue
$payload = Join-Path $package 'payload'
New-Item -ItemType Directory -Force -Path $payload | Out-Null
Copy-Item (Join-Path $publish '*') $payload -Recurse -Force
Copy-Item (Join-Path $root 'install\install.ps1') $package -Force
Copy-Item (Join-Path $root 'install\uninstall.ps1') $package -Force
Copy-Item (Join-Path $root 'install\OWNERSHIP.md') $package -Force

Write-Host "MiniDoc package ready: $package"
Write-Host "Install with: powershell.exe -NoProfile -ExecutionPolicy Bypass -File `"$package\install.ps1`""
