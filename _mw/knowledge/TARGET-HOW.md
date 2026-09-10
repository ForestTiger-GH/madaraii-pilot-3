# TARGET-HOW-0001 — MiniDoc 0.1 mechanism set

**Status:** accepted design baseline for `TARGET-WHAT-0001`  
**Environment:** Windows 11 x64, .NET 10 Windows Desktop, Windows Runtime APIs  
**Architecture:** responsibility allocation in `docs/ARCHITECTURE.md`

This owner is operationally sufficient without reading the Scientific Knowledge corpus.

## Realization

### Application shell and editor

Use a WPF `Window` with a standard menu/compact toolbar, `RichTextBox`/`FlowDocument` for DOCX editable/read-only text presentation, and an `Image` inside a scroll viewer for rendered PDF pages. Project source is C# plus XAML and uses no application `PackageReference`.

One document session is active at a time. Mode is `NewDocx`, `EditableDocx`, `ReadOnlyDocx`, or `Pdf`. The shell owns unsaved-change prompting, command availability, status messaging, and transitions between sessions.

Paste into the editor is intercepted and inserted as plain text. The serializer independently refuses any unsupported internal block/inline shape that still appears.

### DOCX codec

`DocxPackage` owns bounded package inspection, relationship-based main-part resolution, compatibility analysis, mapping to/from `FlowDocument`, and in-memory output validation.

Load algorithm:

1. read input bytes into memory after the compressed-size bound;
2. open ZIP read-only; enforce entry-count, aggregate-uncompressed, and main-XML limits;
3. resolve main part from package relationships;
4. parse XML with DTD prohibited and external resolver disabled;
5. classify namespace/signature/main-story structures;
6. if fully supported, create editable FlowDocument plus an immutable original-package session;
7. otherwise create approximate read-only extracted text with explicit reasons and no save authority.

Save algorithm for an editable session:

1. validate that the editor tree contains only supported paragraphs/runs/spans/line breaks and supported formatting values;
2. clone/rebuild the supported main body while retaining allowed original envelope/section metadata;
3. copy original package bytes to memory and update only the resolved main-document ZIP entry;
4. reopen output with the product parser and require editable classification;
5. hash/compare every original non-main entry payload against output to guard preservation;
6. hand the complete output bytes to the explicit document writer.

New DOCX generates the minimum package parts needed by the implemented Open XML package relationship/content-type model.

### Explicit document writer

The document writer is the only runtime filesystem mutation boundary. It writes only a path chosen by the Save/Save As command. Before overwriting an existing target it retains that target’s prior bytes in memory. On an ordinary write exception it attempts restoration; a newly created partial target is deleted when possible. It creates no temp file.

### PDF session

Use `StorageFile.GetFileFromPathAsync` and `PdfDocument.LoadFromFileAsync`. For each page, obtain a `PdfPage` in a scoped lifetime, render with `PdfPageRenderOptions` into `InMemoryRandomAccessStream`, transfer the bytes into a WPF `BitmapImage` with eager load, then dispose page/stream resources. Render only the current page. Zoom changes trigger rerender and are bounded to 50–400%.

Password-protected documents stop with an explicit unsupported message in v0.1.

### Process lifecycle

No resident worker exists. PDF rendering is asynchronous only while the window is active. When the main window has completed unsaved-change handling and reaches its `Closed` event, dispose the current PDF session and explicitly invoke `Environment.Exit(0)`. This is a containment decision tied to the process-termination invariant. Windows verification must still prove the exact built configuration leaves no `MiniDoc` process after PDF use.

## Application and user operation

- New starts an empty editable DOCX session.
- Open first resolves unsaved changes, then selects `.docx`/`.pdf` via the standard Windows picker.
- Editable DOCX exposes formatting/save commands.
- Read-only DOCX exposes selection/copy and an explicit compatibility reason; save commands are unavailable.
- PDF exposes page/zoom controls only.
- Save uses the current DOCX path; Save As selects a new path.
- Close/replace with dirty editable content asks Save / Discard / Cancel.

## Build, installation, update, uninstall

`build.ps1` requires official .NET 10 SDK, publishes `src/MiniDoc/MiniDoc.csproj` Release self-contained `win-x64` into `artifacts/publish`, copies installer assets, and emits `artifacts/MiniDoc-0.1.0/` as the install source. Build-process cache/temp variables point inside repository `.build/` where supported.

`install.ps1` self-elevates when needed, copies the publish payload to `%ProgramFiles%\MiniDoc`, creates all-users shortcuts, and writes one HKLM Uninstall subkey. It first refuses installation while MiniDoc is running. Reinstallation replaces only application-owned payload.

`uninstall.ps1` removes shortcuts and the registry key, then invokes a bounded built-in command-shell cleanup to remove the install directory after the uninstall script process exits. It does not enumerate, remember, or delete user document paths.

No updater, file association, service, scheduled task, Run key, protocol handler, or AppData state exists.

## Design-choice ledger

| Choice | State | Boundary |
|---|---|---|
| WPF + .NET 10 + Windows Runtime | `FIXED` | required for first-party-only rich edit/PDF path |
| Windows 11 x64 supported target | `FIXED` | v0.1 verified target |
| Conservative DOCX subset/read-only fallback | `FIXED` | protects no-silent-loss invariant |
| Zero runtime technical state | `FIXED` | no AppData/Temp/cache/autosave/recent-files |
| Memory-staged, non-temp save | `FIXED` | documented non-atomic crash limitation |
| Page-at-a-time Windows PDF rendering, zoom ≤400% | `FIXED` | no alternate renderer in v0.1 |
| Explicit process exit after definitive window close | `FIXED` pending runtime evidence | protects shutdown invariant |
| Exact source-class/file decomposition inside modules | `BOUNDED_OPEN` | may vary if architecture responsibilities remain intact |
| Future installer technology | `BOUNDED_OPEN` outside v0.1 | may later move to MSI/MSIX under a new target |

No `UNRESOLVED_BLOCKING` design choice remains for implementation.

## WHAT trace and verification points

- safe DOCX editing → compatibility scanner, controlled FlowDocument model, output re-open, non-main payload hash preservation;
- PDF viewing → first-party `Windows.Data.Pdf`, page/zoom tests on Windows;
- zero app state → no runtime state owner plus filesystem/residue verification;
- close means close → no resident mechanisms, explicit exit, process smoke test after PDF render;
- install/uninstall → finite ownership manifest + registry/shortcut/path verification;
- no third-party dependencies → project/reference census and published-file dependency inspection;
- user-document preservation → uninstall has no user-path input and deletes fixed product-owned locators only.

## Recovery/reopen

A failed compatibility test reopens the DOCX slice. A PDF render/process-lifecycle failure reopens the PDF/process mechanism. A residue failure reopens install/uninstall or runtime write ownership. Any target expansion reopens only the affected WHAT/HOW responsibility.
