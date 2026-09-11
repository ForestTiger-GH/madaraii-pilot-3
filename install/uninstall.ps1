[CmdletBinding()]
param(
    [switch]$NoElevation
)

$ErrorActionPreference = 'Stop'

function Test-Administrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = [Security.Principal.WindowsPrincipal]::new($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

if (-not (Test-Administrator)) {
    if ($NoElevation) { throw 'Administrator rights are required for per-machine uninstall.' }
    $arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`""
    $process = Start-Process -FilePath 'powershell.exe' -ArgumentList $arguments -Verb RunAs -Wait -PassThru
    exit $process.ExitCode
}

if (Get-Process -Name 'MiniDoc' -ErrorAction SilentlyContinue) {
    throw 'Close MiniDoc before uninstalling it.'
}

$installDir = Join-Path $env:ProgramFiles 'MiniDoc'
$desktopShortcut = Join-Path $env:PUBLIC 'Desktop\MiniDoc.lnk'
$startShortcut = Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\MiniDoc.lnk'
$uninstallKey = 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\MiniDoc'

Remove-Item $desktopShortcut -Force -ErrorAction SilentlyContinue
Remove-Item $startShortcut -Force -ErrorAction SilentlyContinue
Remove-Item $uninstallKey -Recurse -Force -ErrorAction SilentlyContinue

if (Test-Path $installDir) {
    Remove-Item $installDir -Recurse -Force
}

Write-Host 'MiniDoc was uninstalled. User documents were not enumerated or removed.'
