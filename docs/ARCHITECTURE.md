# MiniDoc 0.1 Product Architecture — PA-0001 revision 3

**Status:** accepted current  
**Target:** `TARGET-WHAT-0001` revision 3 / `TARGET-HOW-0001` revision 5  
**Current Product:** `1df3b56528ac04b4f0d0a043593365b575007b93`

A separate architecture has independent value because editor/UI orchestration, DOCX semantics, search, Windows PDF rendering, explicit filesystem mutation, installation and verification have different failure/evolution boundaries. The allocation stays small and maps to concrete responsibilities rather than framework layers.

## Realization units

| Unit | Exclusive responsibility | Allowed platform dependencies | Material boundary |
|---|---|---|---|
| `Shell` | Ribbon window, command availability, active document/session transitions, unsaved-change handling, replacement intent/content-generation fencing, candidate admission authority, status/user messages | WPF + first-party WPF Ribbon + common dialogs | never parses package XML or writes install state |
| `Editor` | supported in-memory `FlowDocument` operations: formatting, simple tables, selection and structure-bounded find/replace | WPF document model | never decides DOCX package compatibility or replacement-session authority |
| `Docx` | bounded package validation, editable compatibility classification, main-story reading, FlowDocument mapping, marker/text compatibility extraction, output serialization/validation | `System.IO.Compression`, LINQ to XML, WPF document model | never writes a filesystem target directly; current 0.1 does not dereference media/notes parts for compatibility previews |
| `Pdf` | load a PDF document and render one requested page to an in-memory WPF image | `Windows.Data.Pdf`, `Windows.Storage`, WPF image model | read-only; no disk cache; does not decide whether a late result still has current presentation authority |
| `Storage` | explicit Save/Save As target mutation and bounded best-effort restoration | `System.IO` | writes only user-selected document target |
| `Build/Install` | publish, per-machine placement, shortcuts, uninstall registration/removal | PowerShell, .NET SDK, Windows shell/registry | executes only on explicit build/install/uninstall command |
| `Verification` | fixture/static/system checks and evidence output, including installed PDF success/exit-code oracle | PowerShell/.NET tooling/Windows | observes/fails; cannot redefine target or admit Product state |

`Editor` remains a responsibility boundary rather than a framework: it centralizes operations that must preserve the WPF document tree regardless of whether the source was a new or loaded editable DOCX.

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

## State ownership and authority

`Shell` owns only in-memory active-session state and the authority needed to decide whether an asynchronously acquired replacement may become current. Two local freshness dimensions protect replacement semantics:

- Open/New **intent generation** — later replacement intent supersedes earlier unresolved Open work;
- editable-document **content generation** — an Open ticket captured after unsaved-change handling becomes stale if the current document changes before admission.

The exact helper decomposition for these generations is implementation-local; the authority responsibility itself belongs to `Shell`.

`DocxSession` owns immutable original DOCX bytes, resolved main-part identity and editable/compatibility classification for the lifetime of an open document. `PdfSession` owns only an in-memory `PdfDocument` and render resources while open. `Editor` owns no durable state beyond the active `FlowDocument` tree.

A PDF render result has presentation authority only while the `Shell` request/session/page/zoom identity remains current. Releasing or replacing the PDF session invalidates that authority.

No runtime technical state persists after process termination except user documents explicitly saved by the user.

## DOCX internal responsibility split

The `Docx` unit may use several source files because their failure modes differ:

- package reader/limits/relationship resolution needed for package and main-part integrity;
- editable compatibility scanner;
- editable FlowDocument reader/writer for paragraphs/runs/simple tables;
- read-only compatibility extractor for safely extractable main-story text plus explicit unsupported-object/reference/field markers;
- package writer/new-package creation/output revalidation.

Current 0.1 compatibility presentation does **not** allocate relationship-backed graphics preview rendering or notes-part body dereferencing to any unit. Adding that behavior requires a later Target/Design change.

These source modules remain implementation details under one semantic responsibility, not independent truth planes.

## Cross-cutting invariants

- Every runtime filesystem write enters through `Storage.DocumentWriter` and originates from explicit Save/Save As.
- DOCX editable authority is granted only by the compatibility analyzer and can be revoked by serializer validation.
- Replacement-session authority remains with `Shell`; stale Open work cannot become current merely because it completed later.
- Editable simple tables remain rectangular and within the supported property set after every editor command; row-cell width and WPF column descriptors remain coherent.
- Find/replace mutates text only and never crosses structural lanes.
- Unsupported charts/SmartArt/shapes/fields/footnotes remain outside editable save authority; compatibility presentation uses text/markers/placeholders and never turns them into rewritten editable semantics.
- PDF output is transient in memory; stale render completion cannot mutate newer current presentation.
- The main window is the process lifetime boundary.
- Installed technical state is enumerated by `install/OWNERSHIP.md`; uninstall may delete only those locators.
- No Product module may introduce a third-party package/reference without reopening Target WHAT/HOW and verification.

## Failure containment

A DOCX parser/compatibility failure produces an error or read-only result before current-session replacement is admitted. Unsupported rich DOCX semantics degrade to explicit marker/text compatibility presentation without editable authority.

A table/format serializer failure leaves disk untouched because output is built/validated in memory first. A disk-write failure triggers bounded best-effort target restoration; if both direct write and recovery fail, target integrity is reported as unknown.

A PDF candidate may finish loading after it has become stale, but `Shell` rejects/disposes it before admission. A PDF render failure leaves the source PDF untouched and returns an explicit error; a late stale render result lacks current presentation authority.

Installation failures report the step and preserve only declared owner paths, allowing bounded rerun/uninstall cleanup.

## Verification allocation

Verification binds exact candidate/configuration identity. Deterministic semantic checks cover DOCX/search/table/transition-guard logic, while Windows system verification covers build/package/dependency boundary and installed lifecycle behavior.

The installed verification path treats PDF success as successful candidate admission plus successful initial page render before zero process exit. Noninteractive verification failure returns non-zero so process termination alone cannot masquerade as successful PDF behavior.

Verification evidence does not itself admit Product state, release, deploy, or validate user outcomes.

## Evolution seams

New editable DOCX constructs extend the `Docx` recognition/conversion tests and `Editor` operations only where needed. Native shapes/charts/fields/footnotes or richer compatibility preview/note-body presentation require explicit target/design reopening before new responsibilities are allocated. New viewer functions extend `Pdf`/Shell. New formats receive their own codec/viewer responsibility only when commissioned. A future packaging technology may replace `Build/Install` without entering document logic.

A new editor mutation mechanism must participate in Shell document-content freshness semantics if it can occur while replacement Open work is pending; otherwise the affected authority claim is reopened.
