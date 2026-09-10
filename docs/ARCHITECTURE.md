# MiniDoc 0.1 Product Architecture — PA-0001 revision 2

**Status:** accepted current  
**Target:** `TARGET-WHAT-0001` / `TARGET-HOW-0001`, revision 2

A separate architecture has value because editor/UI orchestration, DOCX semantics, search, Windows PDF rendering, explicit filesystem mutation, installation and verification have different failure/evolution boundaries. The allocation stays small and maps to concrete responsibilities rather than framework layers.

## Realization units

| Unit | Exclusive responsibility | Allowed platform dependencies | Material boundary |
|---|---|---|---|
| `Shell` | Ribbon window, command availability, document session transitions, user messages, unsaved handling | WPF + first-party WPF Ribbon + common dialogs | never parses package XML or writes install state |
| `Editor` | supported in-memory `FlowDocument` operations: formatting, simple tables, selection and structure-bounded find/replace | WPF document model | never decides DOCX package compatibility |
| `Docx` | package validation, editable compatibility classification, main-story/related-part reading, FlowDocument mapping, compatibility extraction, output serialization/validation | `System.IO.Compression`, LINQ to XML, WPF document/image model | never writes a filesystem target directly |
| `Pdf` | load/render one PDF page to an in-memory WPF image | `Windows.Data.Pdf`, `Windows.Storage`, WPF image model | read-only; no disk cache |
| `Storage` | explicit Save/Save As target mutation and best-effort restoration | `System.IO` | writes only user-selected document target |
| `Build/Install` | publish, per-machine placement, shortcuts, uninstall registration/removal | PowerShell, .NET SDK, Windows shell/registry | executes only on explicit build/install/uninstall command |
| `Verification` | fixture/static/system checks and evidence output | PowerShell/.NET tooling/Windows | observes/fails; cannot redefine target |

`Editor` is a small responsibility boundary rather than a framework: it centralizes operations that must preserve the WPF document tree regardless of whether the source was a new or loaded editable DOCX.

## Dependency direction

```text
Shell ──> Editor
  │        │
  │        └── edits FlowDocument owned by active session
  ├────> Docx <── FlowDocument
  └────> Pdf

Shell ──> Storage <── serialized DOCX bytes from Docx

Build/Install ──> published Product payload
Verification ──> target + exact candidate + installed system
```

`Docx` and `Pdf` have no dependency on each other. Installer logic is never loaded by the runtime application. Runtime modules have no dependency on installer registry/shortcut state.

## DOCX internal responsibility split

The `Docx` unit may use several source files because their failure modes differ:

- package reader/limits/relationship resolution;
- editable compatibility scanner;
- editable FlowDocument reader/writer for paragraphs/runs/simple tables;
- compatibility extractor for text/pictures/unsupported-object placeholders/reference material;
- package writer/new-package creation/output revalidation.

These are implementation modules under one semantic owner, not independent truth planes.

## State ownership

`Shell` owns only in-memory active-session state. `DocxSession` owns immutable original DOCX bytes, resolved main-part identity and editable/compatibility classification for the lifetime of an open document. `PdfSession` owns only an in-memory `PdfDocument` and current page state while open. `Editor` owns no durable state beyond the active `FlowDocument` tree.

No runtime technical state persists after process termination except user documents explicitly saved by the user.

## Cross-cutting invariants

- Every runtime filesystem write enters through `Storage.DocumentWriter` and originates from explicit Save/Save As.
- DOCX editable authority is granted only by the compatibility analyzer and can be revoked by serializer validation.
- Editable simple tables remain rectangular and within the supported property set after every editor command.
- Find/replace mutates text only and never crosses structural lanes.
- Unsupported charts/SmartArt/shapes/fields/footnotes remain outside editable save authority; compatibility presentation never turns them into rewritten semantics.
- PDF output is transient in memory.
- The main window is the process lifetime boundary.
- Installed technical state is enumerated by `install/OWNERSHIP.md`; uninstall may delete only those locators.
- No Product module may introduce a third-party package/reference without reopening Target WHAT/HOW and verification.

## Failure containment

A DOCX parser/compatibility failure produces an error or read-only result before mutation. A table/format serializer failure leaves disk untouched because output is built/validated in memory first. A disk-write failure triggers best-effort target restoration. A compatibility-object rendering failure degrades to a labeled placeholder rather than editor authority. A PDF render failure leaves the source PDF untouched and returns an explicit error. Installation failures report the step and preserve only declared owner paths, allowing bounded rerun/uninstall cleanup.

## Evolution seams

New editable DOCX constructs extend the `Docx` recognition/conversion tests and `Editor` operations only where needed. Native shapes/charts/fields/footnotes would justify separate internal responsibilities when implemented, not before. New viewer functions extend `Pdf`/Shell. New formats receive their own codec/viewer responsibility only when commissioned. A future packaging technology may replace `Build/Install` without entering document logic.
