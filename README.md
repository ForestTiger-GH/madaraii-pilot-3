# MiniDoc 0.1

MiniDoc is a small standalone Windows desktop application for bounded basic DOCX editing and local PDF viewing. It uses a Word-like first-party WPF Ribbon, explicit compatibility limits, and a deliberately small installation/runtime footprint.

## Implemented

- DOCX: basic text editing, direct font/text formatting, paragraph formatting, Find/Replace, and simple rectangular tables.
- Compatibility mode: richer DOCX constructs remain read-only rather than receiving lossy save authority.
- PDF: local view-only rendering, page navigation and 50–400% zoom.
- Lifecycle: self-contained `win-x64` build, Program Files installation, all-users shortcuts, HKLM uninstall registration, bounded uninstall.
- Runtime state: no MiniDoc settings/cache/recents/autosave/telemetry/background service/updater state.

Native Word shapes/charts/SmartArt authoring, footnote authoring, TOC/field recalculation, full Word layout fidelity and PDF editing are outside 0.1.

## Build

On Windows with the official .NET 10 SDK:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\scripts\build.ps1 -Clean
```

The install source is produced at `artifacts\MiniDoc-0.1.0\`.

## Install / verify

See:

- [`docs/USER-GUIDE.md`](docs/USER-GUIDE.md)
- [`docs/COMPATIBILITY.md`](docs/COMPATIBILITY.md)
- [`docs/BUILD-INSTALL-VERIFY.md`](docs/BUILD-INSTALL-VERIFY.md)
- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)

The exact admitted source/configuration candidate `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681` passed Windows CI run [34545614992](https://github.com/ForestTiger-GH/madaraii-pilot-3/actions/runs/34545614992): build, semantic DOCX/search/table checks, real Windows PDF rendering, dependency census, install, normal PDF-close process termination, uninstall and bounded residue checks.

CI executed on Windows Server 2025 (`10.0.26100`), so interactive Windows 11 visual/usability qualification remains a documented local acceptance step rather than an inferred claim.

## Engineering state

The repository contains its complete cold-recoverable MADAR Workspace under [`_mw/`](_mw/). Start with [`_mw/WORKSPACE.md`](_mw/WORKSPACE.md) and [`_mw/work/STATE.md`](_mw/work/STATE.md). Product admission, evidence, verification and closure are owned separately from source code.
