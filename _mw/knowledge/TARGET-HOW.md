# TARGET-HOW-0001 — MiniDoc 0.1 mechanism set, revision 2

**Status:** accepted current design baseline for `TARGET-WHAT-0001` revision 2  
**Environment:** Windows 11 x64, .NET 10 Windows Desktop, Windows Runtime APIs  
**Architecture:** responsibility allocation in `docs/ARCHITECTURE.md`

This owner is operationally sufficient without reading the Scientific Knowledge corpus.

## Realization

### Application shell and Ribbon

Use WPF with a `RibbonWindow`/`Ribbon` from `System.Windows.Controls.Ribbon`, `RichTextBox`/`FlowDocument` for DOCX editable/read-only text presentation, and an `Image` inside a scroll viewer for rendered PDF pages.

The Ribbon is deliberately bounded:

- Application/File menu: New, Open, Save, Save As, Exit;
- Quick Access: Save, Undo, Redo;
- Home/Clipboard: Cut, Copy, Paste, Select All;
- Home/Font: font family, size, bold, italic, underline, text colour, highlight;
- Home/Paragraph: alignment, bounded indentation/spacing, paragraph background fill;
- Home/Editing: Find/Replace;
- Insert: simple Table;
- Table contextual controls: add/delete row/column and cell fill where an editable simple table is selected;
- View: PDF page/zoom controls and mode-relevant presentation commands.

Ribbon customization is session-local only. No preference is written to disk.

One document session is active at a time. Mode is `NewDocx`, `EditableDocx`, `ReadOnlyDocx`, or `Pdf`. The shell owns unsaved-change prompting, command availability, status messaging and transitions.

Paste is intercepted and inserted as plain text. The serializer independently rejects any unsupported in-memory block/inline/table shape that appears.

### DOCX package and compatibility analysis

`Docx` owns bounded ZIP/package inspection, relationship-based main-part resolution, compatibility analysis, mapping to/from `FlowDocument`, compatibility extraction and in-memory output validation.

Load algorithm:

1. read input bytes into memory after compressed-size bound;
2. open ZIP read-only; enforce entry-count, aggregate-uncompressed, main-XML and media-size limits;
3. resolve main part from package relationships;
4. parse XML with DTD prohibited and external resolver disabled;
5. classify namespace/signature/main-story structures and required related parts;
6. if every material main-story element is in the supported editable subset, build an editable `FlowDocument` plus immutable original-package session;
7. otherwise build a read-only compatibility document with extracted text, explicit reasons and supported object/reference presentations; no save authority is granted.

Strict OOXML, package signatures, macros/unsupported container forms, unsupported styles/references, unknown material markup and any ambiguity in ownership/classification fail closed.

### Editable paragraph/run mapping

The reader/writer recognizes direct run properties for bold, italic, single underline, explicit font family, half-point size, RGB colour and named `w:highlight`. Paragraph properties support alignment, bounded left/right indentation, before/after spacing and clear RGB paragraph `w:shd` fill.

Theme-derived values, style references and unsupported property children block editable admission rather than being normalized silently. The serializer emits only the exact supported property vocabulary.

### Editable simple-table mapping

A `w:tbl` may be editable only when its structure passes all checks:

- one table level only; no nested table;
- every ordinary row has the same logical cell count;
- no `gridSpan`, `vMerge`, row/cell style reference, floating table positioning or unsupported table/row/cell property;
- table grid is either absent or consistent with the admitted rectangular structure;
- each cell contains only supported paragraphs/runs;
- cell shading, when present, is a clear direct RGB fill supported by the codec.

The reader maps this subset to WPF `Table`/`TableRowGroup`/`TableRow`/`TableCell`. The writer reconstructs the table from the current WPF tree. New-table insertion creates only this subset. Row/column mutation routines preserve rectangularity; a mutation that cannot preserve the serializer model is refused.

### Find/replace

`TextSearch` traverses text-bearing segments in document order and returns matches as exact `TextPointer` ranges. Search treats each paragraph/table cell text flow as a bounded lane and never creates a match across a structural boundary.

- Find Next selects the next exact range from the caret, wrapping once when requested.
- Replace replaces the current exact selected match only.
- Replace All discovers lane-local matches and applies replacements from the end of each lane toward the beginning to preserve earlier positions.
- Search is case-insensitive by default with an optional case-sensitive toggle; replacement is literal text, not regex.

### Compatibility view for objects, references and fields

Compatibility mode is a read-only `FlowDocument` built from source package content without granting serialization authority.

- Ordinary extractable text is shown in document order where safe.
- A recognized embedded raster picture may be decoded in memory and inserted as a WPF `Image` when its `r:embed` relationship resolves to a bounded image part of an allowed raster content type.
- For chart/SmartArt/shape/drawing objects, the extractor searches only their explicit relationship/AlternateContent/fallback lineage for an associated safe image. If none is established, it inserts a labeled object placeholder.
- Footnote/endnote references may be rendered as labeled reference markers followed by an appended “Notes” section containing safely extracted referenced story text.
- Complex/simple field code is never evaluated. Stored field result text may be shown with a label indicating cached result; TOC is therefore view-only cached content, not recalculated content.

Any ambiguous relationship, oversized media, unsupported media type or malformed object yields a placeholder/reason rather than guessed rendering.

### DOCX save algorithm

For an editable session:

1. validate the complete WPF editor tree against supported paragraph/run/table semantics;
2. serialize supported main body structures and retained allowed final `sectPr` metadata;
3. copy the original package bytes to memory and update only the resolved main-document ZIP entry; for a new document, generate the minimum package;
4. reopen output with the product parser and require editable classification;
5. hash/compare every original non-main entry payload against output to guard preservation;
6. hand complete output bytes to the explicit document writer.

No chart, drawing, field, footnote or unsupported story is ever rewritten from compatibility mode because compatibility mode has no Save authority.

### Explicit document writer

The document writer is the only runtime filesystem mutation boundary. It writes only a path chosen through Save/Save As. Before overwriting an existing target it retains prior bytes in memory. On an ordinary write exception it attempts restoration; a newly created partial target is deleted when possible. It creates no temp file.

### PDF session

Use `StorageFile.GetFileFromPathAsync` and `PdfDocument.LoadFromFileAsync`. For each page, obtain a `PdfPage` in a scoped lifetime, render through `PdfPageRenderOptions` into `InMemoryRandomAccessStream`, transfer bytes into a WPF `BitmapImage` with eager load, then dispose page/stream resources. Render only the current page. Zoom rerenders at 50–400%.

Password-protected documents stop with an explicit unsupported message.

### Process lifecycle

No resident worker exists. PDF rendering is asynchronous only while the window is active. When the main window has completed unsaved-change handling and reaches its definitive `Closed` path, dispose the current PDF session and explicitly invoke `Environment.Exit(0)`. Windows verification must still prove the exact built configuration leaves no `MiniDoc` process after PDF use.

## Application and user operation

- New starts an empty editable DOCX session.
- Open resolves unsaved changes, then selects `.docx`/`.pdf` via the standard Windows picker.
- Editable DOCX exposes supported Ribbon controls, search/replace and save.
- Read-only DOCX exposes selection/copy plus compatibility reasons/previews/reference text; editing/save controls are disabled.
- PDF exposes page/zoom controls only.
- Save uses the current DOCX path; Save As selects a new path.
- Close/replace with dirty editable content asks Save / Discard / Cancel.

## Build, installation, update, uninstall

`build.ps1` requires official .NET 10 SDK, publishes `src/MiniDoc/MiniDoc.csproj` Release self-contained `win-x64` into repository-local artifacts, copies installer assets, and emits a complete install-source directory. Build-process cache/temp variables point inside repository `.build/` where supported.

`install.ps1` self-elevates when needed, refuses installation while MiniDoc is running, copies the publish payload to `%ProgramFiles%\MiniDoc`, creates all-users Desktop and Start Menu shortcuts, and writes one HKLM Uninstall subkey. Reinstallation replaces only application-owned payload.

`uninstall.ps1` removes shortcuts and registry registration, then performs bounded post-exit removal of the install directory. It does not enumerate, remember or delete user document paths.

No updater, file association, service, scheduled task, Run key, protocol handler or AppData state exists.

## Design-choice ledger

| Choice | State | Boundary |
|---|---|---|
| WPF + .NET 10 + Windows Runtime | `FIXED` | first-party-only editor/PDF path |
| WPF Ribbon organization | `FIXED` | Word-like UX without third-party UI framework |
| Windows 11 x64 supported target | `FIXED` | first-release verified target |
| Fail-closed DOCX admission | `FIXED` | protects no-silent-loss invariant |
| Simple rectangular table subset | `FIXED` | required table editing without full Word table engine |
| Direct text/paragraph/cell formatting subset | `FIXED` | required basic Word-like editing |
| Charts/SmartArt/shapes compatibility-only | `FIXED` | no lossy flattening or graphics engine |
| Footnotes/fields/TOC compatibility-only | `FIXED` | no hidden field/layout engine |
| Zero runtime technical state | `FIXED` | no AppData/Temp/cache/autosave/recents/customization persistence |
| Memory-staged non-temp save | `FIXED` | documented non-atomic crash limitation |
| Page-at-a-time Windows PDF rendering | `FIXED` | no alternate renderer |
| Explicit process exit after definitive close | `FIXED`, runtime evidence pending | shutdown invariant |
| Exact private helper decomposition | `BOUNDED_OPEN` | must preserve architecture responsibilities |
| Future installer/graphics/fields technology | `BOUNDED_OPEN` outside current target | new target required |

No `UNRESOLVED_BLOCKING` design choice remains for current implementation.

## WHAT trace and verification points

- Word-like UI → WPF Ribbon, grouped command availability and mode/context checks;
- find/replace → structure-bounded `TextSearch` fixtures including tables;
- richer DOCX editing → compatibility scanner + FlowDocument paragraph/run/table model + output reopen;
- no silent loss → unsupported object/reference classes force compatibility mode; non-main payload hashes preserved for editable files;
- chart/diagram handling → explicit preview-or-placeholder behavior only in read-only mode;
- PDF viewing → first-party `Windows.Data.Pdf`, page/zoom Windows tests;
- zero app state → source/write-boundary audit plus filesystem residue test;
- close means close → no resident mechanisms + explicit exit + process smoke test after PDF render;
- install/uninstall → finite ownership manifest + registry/shortcut/path verification;
- no third-party dependencies → project/reference and published-file dependency census;
- user-document preservation → uninstall has no user-path input and deletes only fixed product-owned locators.

## Recovery/reopen

A failed table/formatting/search round-trip test reopens that DOCX slice. A request for editable shape/chart/footnote/TOC reopens WHAT/HOW. A PDF render/process-lifecycle failure reopens PDF/process HOW. A residue failure reopens install/uninstall or runtime write ownership. Persistent UI customization reopens the zero-state Decision. Any implementation discovery may reopen only the affected target/design slice.
