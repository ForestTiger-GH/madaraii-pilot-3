# MiniDoc 0.1

MiniDoc is a small standalone Windows desktop application for bounded basic DOCX editing and local PDF viewing. It uses a Word-like first-party WPF Ribbon, explicit compatibility limits, and a deliberately small installation/runtime footprint.

## Implemented

- DOCX: basic text editing, direct font/text formatting, paragraph formatting, Find/Replace, and simple rectangular tables.
- Compatibility mode: richer DOCX constructs remain read-only with safely extractable main-story text plus explicit markers/placeholders rather than lossy save authority.
- PDF: local view-only rendering, page navigation and 50–400% zoom.
- Lifecycle: self-contained `win-x64` build, Program Files installation, all-users shortcuts, HKLM uninstall registration, bounded uninstall.
- Runtime state: no MiniDoc settings/cache/recents/autosave/telemetry/background service/updater state.
- Session integrity: replacement Open is candidate-first and freshness-fenced; stale Open/render work cannot acquire newer current-session presentation authority.

Native Word shapes/charts/SmartArt authoring, footnote authoring, TOC/field recalculation, full Word layout fidelity and PDF editing are outside 0.1.

## Current admitted Product

Current `PRODUCT-0001` binds exact verified Product source/configuration revision:

`1df3b56528ac04b4f0d0a043593365b575007b93`

It realizes `TARGET-WHAT-0001` revision 3 and `TARGET-HOW-0001` revision 5 under `PA-0001 revision 3`.

Windows CI run `34976593344` on that exact candidate passed build/semantic checks/package, dependency boundary, installed PDF admission/render success + process exit, uninstall/residue checks and artifact upload. The current evidence route is `_mw/evidence/EV-E001-02-WINDOWS-CI.md` / `VERIFICATION-E001-02`.

Admission is not a release/deployment claim. Interactive Windows 11 visual acceptance and broad real-document qualification remain explicit bounded evidence limits.

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

## Development history

The post-closure Jester finalization epoch moved the admitted Product lineage:

`b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`
→ `4a7c1d5b2bea939ebd508dcce19c4e93f58149cf`
→ `1df3b56528ac04b4f0d0a043593365b575007b93`

Its closure audit passed and `FINALIZATION-E001-01` established the epoch as finalized / archive-ready, while explicitly leaving physical archival as a separate future action.

- [Finalization Result](_mw/epochs/001-jester-finalization/results/FINALIZATION-E001-01.md)
- [Sealed Development Report](development-reports/epoch-001-jester-finalization.md)

The standalone Jester Reports and detailed Work history remain separately preserved under the epoch history; the sealed Development Report does not replace them.

## Engineering state

The complete cold-recoverable MADAR Workspace is under [`_mw/`](_mw/). Start with [`_mw/AGENTS.md`](_mw/AGENTS.md), which resolves current semantic owners and the most recent finalized epoch state.

Current Product, Scientific Knowledge, Target WHAT/HOW, Decisions, Questions and Evidence live outside the epoch-specific Work history. A new global outcome must begin a new Development Epoch rather than appending substantive Work to the sealed EPOCH-001 Baseline.
