# MiniDoc 0.1 Product Architecture — PA-0001

**Status:** accepted  
**Target:** `TARGET-WHAT-0001` / `TARGET-HOW-0001`

A separate architecture has value because document parsing/writing, Windows PDF rendering, UI state, explicit filesystem mutation, and installation have different failure/evolution boundaries. This allocation stays small: it defines responsibilities and interfaces rather than a speculative framework.

## Realization units

| Unit | Exclusive responsibility | Allowed platform dependencies | Material boundary |
|---|---|---|---|
| `Shell` | main window, commands, session transitions, user messages, unsaved handling | WPF, common dialogs | never parses package XML or writes installation state |
| `Docx` | DOCX package validation, compatibility classification, FlowDocument conversion, output package validation | `System.IO.Compression`, LINQ to XML, WPF document model | never writes a filesystem target directly |
| `Pdf` | load/render one PDF page to an in-memory WPF image | `Windows.Data.Pdf`, `Windows.Storage`, WPF image model | read-only; no disk cache |
| `Storage` | explicit Save/Save As target mutation and best-effort restoration | `System.IO` | writes only user-selected document target |
| `Build/Install` | publish, per-machine placement, shortcuts, uninstall registration/removal | PowerShell, .NET SDK, Windows shell/registry | executes only on explicit build/install/uninstall command |
| `Verification` | static/fixture/system checks and evidence output | PowerShell/.NET tooling/Windows | observes and fails; does not redefine target |

## Dependency direction

```text
Shell ──> Docx
  │       └──> Storage (through shell orchestration)
  └────> Pdf

Build/Install ──> published Product payload
Verification ──> target + exact candidate + installed system
```

`Docx` and `Pdf` have no dependency on each other. Installer logic is never loaded by the runtime application. Runtime modules have no dependency on installer registry/shortcut state.

## State ownership

The Shell owns only in-memory session state. `DocxSession` owns original DOCX package bytes and compatibility identity for the lifetime of an open document. `PdfSession` owns only an in-memory `PdfDocument` handle/current page index while open. No runtime state persists after process termination except user documents explicitly saved by the user.

## Cross-cutting invariants

- Every runtime filesystem write enters through `Storage.DocumentWriter` and originates from explicit Save/Save As.
- DOCX editable authority is granted only by the compatibility analyzer.
- PDF output is always transient in memory.
- The main window is the process lifetime boundary.
- Installed technical state must be enumerated by `install/OWNERSHIP.md`; uninstall may delete only those locators.
- No Product module may introduce a third-party package/reference without reopening Target WHAT/HOW and verification.

## Failure containment

A DOCX parser/compatibility failure produces an error or read-only result before mutation. A save-format failure leaves disk untouched because output is built/validated in memory first. A disk-write failure triggers best-effort target restoration. A PDF render failure leaves the source PDF untouched and returns the shell to an explicit error state. Installation failures report the step and preserve only declared owner paths, allowing rerun/uninstall cleanup.

## Evolution

New DOCX constructs extend `Docx` recognition/conversion tests. New viewer functions extend `Pdf`/Shell. New formats should receive their own codec/viewer responsibility only when implemented. A future packaging technology may replace `Build/Install` without entering document logic. These are extension seams, not current features.
