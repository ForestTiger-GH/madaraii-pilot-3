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
    if ($NoElevation) { throw 'Administrator rights are required for per-machine installation.' }
    $arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`""
    $process = Start-Process -FilePath 'powershell.exe' -ArgumentList $arguments -Verb RunAs -Wait -PassThru
    exit $process.ExitCode
}

if (Get-Process -Name 'MiniDoc' -ErrorAction SilentlyContinue) {
    throw 'Close MiniDoc before installing or updating it.'
}

$payload = Join-Path $PSScriptRoot 'payload'
$sourceExe = Join-Path $payload 'MiniDoc.exe'
if (-not (Test-Path $sourceExe)) { throw "Installer payload is incomplete: $sourceExe was not found." }

$installDir = Join-Path $env:ProgramFiles 'MiniDoc'
$desktopShortcut = Join-Path $env:PUBLIC 'Desktop\MiniDoc.lnk'
$startShortcut = Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\MiniDoc.lnk'
$uninstallKey = 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\MiniDoc'

if (Test-Path $installDir) { Remove-Item $installDir -Recurse -Force }
New-Item -ItemType Directory -Path $installDir -Force | Out-Null
Copy-Item (Join-Path $payload '*') $installDir -Recurse -Force
Copy-Item (Join-Path $PSScriptRoot 'uninstall.ps1') (Join-Path $installDir 'uninstall.ps1') -Force
Copy-Item (Join-Path $PSScriptRoot 'OWNERSHIP.md') (Join-Path $installDir 'OWNERSHIP.md') -Force

$installedExe = Join-Path $installDir 'MiniDoc.exe'
if (-not (Test-Path $installedExe)) { throw 'MiniDoc executable was not installed.' }

$shell = New-Object -ComObject WScript.Shell
foreach ($shortcutPath in @($desktopShortcut, $startShortcut)) {
    $parent = Split-Path -Parent $shortcutPath
    New-Item -ItemType Directory -Path $parent -Force | Out-Null
    $shortcut = $shell.CreateShortcut($shortcutPath)
    $shortcut.TargetPath = $installedExe
    $shortcut.WorkingDirectory = $installDir
    $shortcut.IconLocation = "$installedExe,0"
    $shortcut.Description = 'MiniDoc document editor and PDF viewer'
    $shortcut.Save()
}

New-Item -Path $uninstallKey -Force | Out-Null
$uninstallScript = Join-Path $installDir 'uninstall.ps1'
$estimatedKb = [Math]::Ceiling(((Get-ChildItem $installDir -File -Recurse | Measure-Object Length -Sum).Sum) / 1KB)
New-ItemProperty -Path $uninstallKey -Name 'DisplayName' -Value 'MiniDoc' -PropertyType String -Force | Out-Null
New-ItemProperty -Path $uninstallKey -Name 'DisplayVersion' -Value '0.1.0' -PropertyType String -Force | Out-Null
New-ItemProperty -Path $uninstallKey -Name 'Publisher' -Value 'MiniDoc' -PropertyType String -Force | Out-Null
New-ItemProperty -Path $uninstallKey -Name 'InstallLocation' -Value $installDir -PropertyType String -Force | Out-Null
New-ItemProperty -Path $uninstallKey -Name 'DisplayIcon' -Value $installedExe -PropertyType String -Force | Out-Null
New-ItemProperty -Path $uninstallKey -Name 'UninstallString' -Value "powershell.exe -NoProfile -ExecutionPolicy Bypass -File `"$uninstallScript`"" -PropertyType String -Force | Out-Null
New-ItemProperty -Path $uninstallKey -Name 'NoModify' -Value 1 -PropertyType DWord -Force | Out-Null
New-ItemProperty -Path $uninstallKey -Name 'NoRepair' -Value 1 -PropertyType DWord -Force | Out-Null
New-ItemProperty -Path $uninstallKey -Name 'EstimatedSize' -Value ([int]$estimatedKb) -PropertyType DWord -Force | Out-Null

Write-Host "MiniDoc 0.1.0 installed in $installDir"
