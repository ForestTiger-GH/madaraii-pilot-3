[CmdletBinding()]
param(
    [string]$PackagePath,
    [string]$PdfFixture
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($PackagePath)) { $PackagePath = Join-Path $root 'artifacts\MiniDoc-0.1.0' }
if ([string]::IsNullOrWhiteSpace($PdfFixture)) { $PdfFixture = Join-Path $root 'tests\fixtures\sample.pdf' }
$PackagePath = [IO.Path]::GetFullPath($PackagePath)
$PdfFixture = [IO.Path]::GetFullPath($PdfFixture)

function Assert-True([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw $Message }
}

function Test-Administrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = [Security.Principal.WindowsPrincipal]::new($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

Assert-True (Test-Administrator) 'Windows verification must run from an elevated PowerShell session or disposable elevated CI runner.'
Assert-True (Test-Path (Join-Path $PackagePath 'install.ps1')) 'Build the install package first with scripts\build.ps1.'
Assert-True (Test-Path $PdfFixture) 'PDF verification fixture was not found.'

$installDir = Join-Path $env:ProgramFiles 'MiniDoc'
$desktopShortcut = Join-Path $env:PUBLIC 'Desktop\MiniDoc.lnk'
$startShortcut = Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\MiniDoc.lnk'
$uninstallKey = 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\MiniDoc'
$appDataPath = Join-Path $env:APPDATA 'MiniDoc'
$localAppDataPath = Join-Path $env:LOCALAPPDATA 'MiniDoc'
$tempPattern = Join-Path $env:TEMP 'MiniDoc*'

$existing = @($installDir, $desktopShortcut, $startShortcut, $uninstallKey) | Where-Object { Test-Path $_ }
Assert-True ($existing.Count -eq 0) 'Verification requires a clean machine with no existing MiniDoc installation state.'

$documents = [Environment]::GetFolderPath([Environment+SpecialFolder]::MyDocuments)
$sentinel = Join-Path $documents ("MiniDoc-Verification-Sentinel-{0}.docx" -f [Guid]::NewGuid().ToString('N'))
[IO.File]::WriteAllText($sentinel, 'user-document sentinel; uninstall must preserve this file')
$installed = $false

try {
    & (Join-Path $PackagePath 'install.ps1') -NoElevation
    $installed = $true

    Assert-True (Test-Path (Join-Path $installDir 'MiniDoc.exe')) 'Installed MiniDoc.exe is missing.'
    Assert-True (Test-Path $desktopShortcut) 'Desktop shortcut is missing.'
    Assert-True (Test-Path $startShortcut) 'Start Menu shortcut is missing.'
    Assert-True (Test-Path $uninstallKey) 'Windows Uninstall registration is missing.'
    Assert-True (-not (Test-Path $appDataPath)) 'MiniDoc unexpectedly owns an AppData roaming directory.'
    Assert-True (-not (Test-Path $localAppDataPath)) 'MiniDoc unexpectedly owns an AppData local directory.'

    $exe = Join-Path $installDir 'MiniDoc.exe'
    $process = Start-Process -FilePath $exe -ArgumentList @('--verify-close-after-open', "`"$PdfFixture`"") -PassThru
    Wait-Process -Id $process.Id -Timeout 30
    Start-Sleep -Milliseconds 400
    Assert-True (-not (Get-Process -Id $process.Id -ErrorAction SilentlyContinue)) 'MiniDoc process remained alive after PDF open/render and normal window Close().'

    & (Join-Path $installDir 'uninstall.ps1') -NoElevation
    $installed = $false

    Assert-True (-not (Test-Path $installDir)) 'Install directory remained after uninstall.'
    Assert-True (-not (Test-Path $desktopShortcut)) 'Desktop shortcut remained after uninstall.'
    Assert-True (-not (Test-Path $startShortcut)) 'Start Menu shortcut remained after uninstall.'
    Assert-True (-not (Test-Path $uninstallKey)) 'Uninstall registry key remained after uninstall.'
    Assert-True (Test-Path $sentinel) 'Uninstall removed an unowned user document sentinel.'
    Assert-True (-not (Test-Path $appDataPath)) 'AppData roaming residue exists after uninstall.'
    Assert-True (-not (Test-Path $localAppDataPath)) 'AppData local residue exists after uninstall.'
    Assert-True (@(Get-ChildItem -Path $tempPattern -Force -ErrorAction SilentlyContinue).Count -eq 0) 'MiniDoc-named Temp residue exists after uninstall.'

    Write-Host 'MiniDoc Windows lifecycle verification: PASS'
}
finally {
    if ($installed -and (Test-Path (Join-Path $installDir 'uninstall.ps1'))) {
        try { & (Join-Path $installDir 'uninstall.ps1') -NoElevation } catch { Write-Warning "Cleanup uninstall failed: $_" }
    }
    Remove-Item $sentinel -Force -ErrorAction SilentlyContinue
}
