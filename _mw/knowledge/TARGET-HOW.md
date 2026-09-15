# TARGET-HOW-0001 — MiniDoc 0.1 mechanism set, revision 4

**Status:** accepted current design baseline for `TARGET-WHAT-0001` revision 2  
**Environment:** Windows 11 x64, .NET 10 Windows Desktop, Windows Runtime APIs  
**Architecture:** responsibility allocation in `docs/ARCHITECTURE.md`

This owner is operationally sufficient without reading the Scientific Knowledge corpus.

Revision 4 retains revision-3 compatibility/search corrections and reconciles the verified post-Jester Product mechanisms admitted in `EPOCH-001`: replacement-document admission is candidate-first, PDF presentation is fenced against stale asynchronous completion, simple-table column descriptors remain coherent with row-cell width, and direct-save recovery reports target integrity as unknown when both write and recovery fail. Target WHAT is unchanged.

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

A replacement open uses candidate-first admission: the selected DOCX is fully parsed/classified, or the selected PDF obtains a valid `PdfSession`, before the current PDF/session state is released and the new document becomes authoritative UI state. A failed candidate open therefore reports failure without dismantling the previously active session.

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
7. otherwise build a read-only compatibility document with extracted text, explicit reasons and bounded markers/placeholders for unsupported structures; no save authority is granted.

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

The reader maps this subset to WPF `Table`/`TableRowGroup`/`TableRow`/`TableCell`. The writer reconstructs the table from the current WPF tree. New-table insertion creates only this subset. Row/column mutation routines preserve rectangularity and keep `Table.Columns` descriptor count coherent with row-cell width; an inconsistent or non-rectangular mutation state is refused rather than serialized ambiguously.

### Find/replace

`TextSearch` traverses text-bearing segments in document order and returns matches as exact `TextPointer` ranges. Search treats each paragraph/table cell text flow as a bounded lane and never creates a match across a structural boundary.

- Find Next selects the next exact range from the caret, wrapping once when requested.
- Replace replaces the current exact selected match only.
- Replace All discovers lane-local matches and applies replacements from the end of each lane toward the beginning to preserve earlier positions.
- Current MiniDoc 0.1 UI search is case-insensitive; replacement is literal text, not regex. The reusable search mechanism may accept bounded case-sensitivity internally, but no case-sensitive UI control is part of the current target.

### Compatibility view for objects, references and fields

Compatibility mode is a read-only `FlowDocument` built from the parsed main-document story without granting serialization authority.

- Ordinary safely extractable main-story text is shown in document order.
- Graphics, drawings, charts, SmartArt, embedded objects and related unsupported visual structures are represented by explicit labeled markers/placeholders. MiniDoc 0.1 does not dereference package media relationships to render object previews in compatibility mode.
- Footnote/endnote references are represented by labeled reference markers. MiniDoc 0.1 does not dereference notes parts to append note-body text.
- Complex/simple field code is never evaluated. Main-story cached/result text that is safely extractable may appear as ordinary text while field-related structures are explicitly marked; TOC is not recalculated.
- Compatibility presentation never implies editable equivalence or save authority for unsupported source semantics.

Relationship-backed media preview extraction and resolved note-body presentation are outside the current 0.1 committed mechanism. They require a later target/design change if desired.

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

The document writer is the only runtime filesystem mutation boundary. It writes only a path chosen through Save/Save As. Before overwriting an existing target it retains prior bytes in memory. On an ordinary write exception it attempts bounded restoration; a newly created partial target is deleted when possible. If that recovery succeeds, the original write failure remains authoritative. If recovery itself fails, MiniDoc reports explicitly that target integrity is unknown and preserves both write and recovery failures as diagnostic causes. It creates no temp file and does not claim crash/power-failure atomicity.

### PDF session and asynchronous presentation freshness

Use `StorageFile.GetFileFromPathAsync` and `PdfDocument.LoadFromFileAsync`. For each page, obtain a `PdfPage` in a scoped lifetime, render through `PdfPageRenderOptions` into `InMemoryRandomAccessStream`, transfer bytes into a WPF `BitmapImage` with eager load, then dispose page/stream resources. Render only the current page. Zoom rerenders at 50–400%.

Every asynchronous render captures the active `PdfSession`, requested page, zoom and a monotonic local render-request identity. After each await, image/status/control mutation is allowed only if that captured identity still matches the current session/request. Releasing or replacing a PDF invalidates outstanding render identities. Stale work may finish internally, but it has no Authority to overwrite current presentation or current status.

Password-protected documents stop with an explicit unsupported message.

### Process lifecycle

No resident worker exists. PDF rendering is asynchronous only while the window is active. When the main window has completed unsaved-change handling and reaches its definitive `Closed` path, dispose the current PDF session and explicitly invoke `Environment.Exit(0)`. Windows verification must still prove the exact built configuration leaves no `MiniDoc` process after PDF use.

## Application and user operation

- New starts an empty editable DOCX session.
- Open resolves unsaved changes, obtains and validates the candidate document/session, and only then replaces the current session; a candidate-open failure leaves the previous active session intact.
- Editable DOCX exposes supported Ribbon controls, case-insensitive search/replace and save.
- Read-only DOCX exposes selection/copy plus compatibility reasons and extracted text/markers; editing/save controls are disabled.
- PDF exposes page/zoom controls only; stale asynchronous render results cannot replace current PDF presentation.
- Save uses the current DOCX path; Save As selects a new path.
- Close/replace with dirty editable content asks Save / Discard / Cancel.
- A save failure whose restore/delete recovery also fails is surfaced as target-integrity uncertainty rather than an ordinary recoverable save failure.

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
| Candidate-first session replacement | `FIXED` | failed candidate open cannot dismantle current session before admission |
| PDF render freshness fencing | `FIXED` | monotonic local request identity; stale completion has no current-UI mutation Authority |
| Fail-closed DOCX admission | `FIXED` | protects no-silent-loss invariant |
| Simple rectangular table subset | `FIXED` | required table editing; row-cell and `Table.Columns` descriptor geometry remain coherent |
| Direct text/paragraph/cell formatting subset | `FIXED` | required basic Word-like editing |
| Compatibility presentation | `FIXED` | read-only extracted text plus explicit unsupported-semantics markers/placeholders |
| Charts/SmartArt/shapes compatibility-only | `FIXED` | no lossy flattening or graphics engine |
| Footnotes/fields/TOC compatibility-only | `FIXED` | no hidden field/layout engine |
| Case-insensitive current UI search | `FIXED` | no case-sensitive UI toggle in current 0.1 target |
| Zero runtime technical state | `FIXED` | no AppData/Temp/cache/autosave/recents/customization persistence |
| Memory-staged non-temp save | `FIXED` | recovery is bounded; dual failure yields explicit integrity-unknown state; crash/power atomicity not claimed |
| Page-at-a-time Windows PDF rendering | `FIXED` | no alternate renderer |
| Explicit process exit after definitive close | `FIXED` | shutdown invariant verified on Windows CI configuration |
| Exact private helper decomposition | `BOUNDED_OPEN` | must preserve architecture responsibilities |
| Future installer/graphics/fields technology | `BOUNDED_OPEN` outside current target | new target required |

No `UNRESOLVED_BLOCKING` design choice remains for current implementation.

## WHAT trace and verification points

- Word-like UI → WPF Ribbon, grouped command availability and mode/context checks;
- document replacement integrity → candidate-first DOCX/PDF admission before current-session release;
- find/replace → structure-bounded, case-insensitive current UI search with `TextSearch` fixtures including tables;
- richer DOCX editing → compatibility scanner + FlowDocument paragraph/run/table model + output reopen;
- table geometry → rectangular row-cell structure plus coherent `Table.Columns` metadata under add/delete and serializer checks;
- no silent loss → unsupported object/reference classes force compatibility mode; non-main payload hashes preserved for editable files;
- chart/diagram handling → explicit marker/placeholder behavior only in read-only compatibility mode;
- PDF viewing → first-party `Windows.Data.Pdf`, page/zoom Windows tests plus current-request fencing for asynchronous UI mutation;
- explicit save boundary → direct write with bounded restore/delete attempt; dual write/recovery failure exposes integrity uncertainty;
- zero app state → source/write-boundary audit plus filesystem residue test;
- close means close → no resident mechanisms + explicit exit + process smoke test after PDF render;
- install/uninstall → finite ownership manifest + registry/shortcut/path verification;
- no third-party dependencies → project/reference and published-file dependency census;
- user-document preservation → uninstall has no user-path input and deletes only fixed product-owned locators.

## Recovery/reopen

A failed replacement-open test or evidence of prior-session destruction reopens shell transition HOW. A failed table/formatting/search round-trip or table descriptor-coherence check reopens that DOCX slice. A request for editable shape/chart/footnote/TOC or richer compatibility-preview presentation reopens WHAT/HOW. A stale PDF render mutating a replaced session, PDF render/process-lifecycle failure, or freshness-fence defect reopens PDF/process HOW. A save path that hides unresolved target integrity after failed recovery reopens the explicit writer HOW. A residue failure reopens install/uninstall or runtime write ownership. Persistent UI customization reopens the zero-state Decision. Any implementation discovery may reopen only the affected target/design slice.
