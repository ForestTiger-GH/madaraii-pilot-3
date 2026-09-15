# MiniDoc 0.1 — Build, Install, Uninstall and Verification

## Supported engineering environment

- Windows 11 x64 for the product promise.
- Official .NET 10 SDK for local build.
- PowerShell.
- Administrator rights for per-machine install/uninstall and full lifecycle verification.

No third-party Product package/runtime dependency is intended.

## Build

From the repository root:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\scripts\build.ps1 -Clean
```

The script redirects project-controlled CLI/NuGet/temp build state into repository-local `.build`, builds and runs `MiniDoc.Checks`, publishes a self-contained `win-x64` application, and assembles:

```text
artifacts\MiniDoc-0.1.0\
  payload\
  install.ps1
  uninstall.ps1
  OWNERSHIP.md
```

The product executable is also available under `artifacts\publish\MiniDoc.exe`.

## Install

Run from an elevated PowerShell session, or allow the installer to request elevation:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\artifacts\MiniDoc-0.1.0\install.ps1
```

Declared installed technical state is limited to the locations documented in `install/OWNERSHIP.md`: `%ProgramFiles%\MiniDoc`, all-users desktop and Start Menu shortcuts, and the MiniDoc HKLM Uninstall registration.

## Uninstall

Use Windows Installed Apps/Programs if the registration is available, or run:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "$env:ProgramFiles\MiniDoc\uninstall.ps1"
```

User documents are outside installer ownership and are not enumerated for deletion.

## Full Windows verification

First build the package, then run from an elevated clean Windows session:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\scripts\verify-windows.ps1
```

The verification contour checks:

1. clean pre-install owner state;
2. per-machine installation and declared shortcuts/registry owner;
3. absence of MiniDoc AppData state;
4. launch of the installed Product with a PDF fixture through the normal shell admission/render path;
5. successful selected-PDF admission and successful initial page render before verification-mode success;
6. normal main-window Close after that successful render, process termination, and process exit code `0`;
7. uninstall;
8. removal of declared installer-owned resources;
9. preservation of an unowned user-document sentinel;
10. absence of MiniDoc-named AppData/Temp residue covered by the test.

Verification mode is intentionally noninteractive: an open/render failure exits non-zero instead of being converted into a modal error followed by apparent lifecycle success. This gives the installed-path test a success oracle rather than proving only that a process eventually terminated.

GitHub Actions executes the same build and lifecycle path on a disposable Windows runner. A green workflow is evidence for that exact repository commit and runner configuration; it does not substitute for testing every end-user Windows configuration.

## Manual acceptance additions

For a release on a user's own Windows 11 machine, also inspect the Ribbon visually, open representative ordinary DOCX files, exercise Find/Replace and table editing, inspect a complex DOCX compatibility view, navigate a multi-page PDF, and verify Installed Apps presentation and shortcuts. Record any behavior that differs from the accepted target before treating that local environment as qualified.
