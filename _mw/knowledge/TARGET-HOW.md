# TARGET-HOW-0001 — MiniDoc 0.1 mechanism set, revision 5

**Status:** accepted current design baseline for `TARGET-WHAT-0001` revision 3  
**Environment:** Windows 11 x64, .NET 10 Windows Desktop, Windows Runtime APIs  
**Architecture:** responsibility allocation in `docs/ARCHITECTURE.md`

This owner is operationally sufficient without reading the Scientific Knowledge corpus.

Revision 5 retains all admitted revision-4 mechanisms, binds to Target WHAT revision 3, and adds bounded authority semantics for asynchronous document opening plus a truthful installed PDF-path success oracle. It does not enlarge the MiniDoc 0.1 feature boundary.

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

One document session is active at a time. Mode is `NewDocx`, `EditableDocx`, `ReadOnlyDocx`, or `Pdf`. The shell owns unsaved-change prompting, current document identity, command availability, status messaging and replacement transitions.

#### Candidate admission and open freshness

Replacement is candidate-first: a selected DOCX is parsed/classified, or a selected PDF obtains a valid candidate `PdfSession`, before current session state is released. Candidate success alone is not sufficient for admission.

The shell owns two independent monotonic generations:

- **open-intent generation**: every New/Open replacement intent supersedes all earlier unresolved Open attempts;
- **document-content generation**: every material edit/formatting mutation advances the generation of the current document state.

An Open attempt captures its open-intent identity and the current document-content generation after the applicable unsaved-change decision and immediately before candidate work begins. Immediately before current-session replacement, admission requires both the attempt to remain the current open intent and the current document-content generation to remain equal to the captured generation.

Consequences:

- a later New/Open intent invalidates an earlier slow candidate even if that earlier candidate finishes successfully later;
- an edit made while asynchronous PDF opening is in flight invalidates the earlier Save/Discard authorization; the late candidate is not allowed to discard those new edits;
- a stale candidate has no Authority to release/replace the current session, overwrite newer status, or present a failure belonging to an obsolete intent;
- an acquired stale PDF candidate is disposed before return;
- if content changed while an otherwise-current candidate was loading, the current document remains authoritative and the shell reports that the open was cancelled because the current document changed; the user may initiate Open again.

Stale work may finish internally. The current target does not require a cancellation framework; freshness is an authority fence, not a scheduler.

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

Every asynchronous render captures the active `PdfSession`, requested page, zoom and a monotonic local render-request identity. After each await, image/status/control mutation is allowed only if that captured identity still matches the current session/request. Releasing or replacing a PDF invalidates outstanding render identities. Stale render work may finish internally, but it has no Authority to overwrite current presentation or current status.

Open-intent freshness and render freshness are separate contracts: a PDF candidate must first remain eligible for session admission; after admission, each page render must remain eligible for presentation mutation.

Password-protected documents stop with an explicit unsupported message.

### Process lifecycle and installed-path verification oracle

No resident worker exists. PDF work is asynchronous only while the window is active. When the main window has completed unsaved-change handling and reaches its definitive `Closed` path, dispose the current PDF session and explicitly invoke `Environment.Exit(0)` for normal operation.

The explicit verification-only startup route returns a success value rather than treating a caught open failure as successful completion. For a PDF, verification success requires both current candidate admission and successful first-page render. In verification-close mode, the process exits with code `0` only after that normal installed shell path succeeds and the window then closes normally; an open/render failure produces a non-zero process exit. `scripts/verify-windows.ps1` must require both process termination and a zero exit code in addition to its install/uninstall/residue assertions.

This verification route creates no persistent marker file, runtime setting or user-visible feature.

## Application and user operation

- New starts an empty editable DOCX session and supersedes any unresolved earlier Open attempt.
- Open resolves current unsaved changes, captures current open/content generations, obtains and validates the candidate document/session, and admits it only while that intent and authorized document state remain current.
- A candidate-open failure leaves the previous active session intact.
- A later New/Open supersedes an earlier unresolved candidate; stale completion is discarded without changing newer current state.
- New edits made while an asynchronous candidate is loading preserve the current document and invalidate the earlier replacement authorization rather than being silently discarded.
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
| Candidate-first session replacement | `FIXED` | candidate success precedes current-session release |
| Open-intent generation | `FIXED` | later New/Open supersedes earlier unresolved Open |
| Document-content generation | `FIXED` | post-decision edits invalidate a late replacement candidate |
| Stale-open status/resource handling | `FIXED` | stale attempt cannot mutate newer status/session; stale PDF candidate is disposed |
| Open cancellation framework | `BOUNDED_OPEN / NOT REQUIRED` | may be added later but cannot replace freshness Authority checks |
| PDF render freshness fencing | `FIXED` | monotonic local request identity; stale completion has no current-UI mutation Authority |
| Verification-mode success oracle | `FIXED` | successful candidate admission + first PDF render required; non-zero exit on failure |
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
| Exact private helper decomposition | `BOUNDED_OPEN` | must preserve architecture responsibilities and testable authority semantics |
| Future installer/graphics/fields technology | `BOUNDED_OPEN` outside current target | new target required |

No `UNRESOLVED_BLOCKING` design choice remains for current implementation.

## WHAT trace and verification points

- Word-like UI → WPF Ribbon, grouped command availability and mode/context checks;
- document replacement integrity → candidate-first admission plus open-intent and document-content generations before current-session release;
- unsaved-content preservation → edits after an earlier replacement decision invalidate that late candidate rather than inheriting stale discard Authority;
- stale Open isolation → stale candidate resources discarded and stale failure/status output suppressed;
- find/replace → structure-bounded, case-insensitive current UI search with `TextSearch` fixtures including tables;
- richer DOCX editing → compatibility scanner + FlowDocument paragraph/run/table model + output reopen;
- table geometry → rectangular row-cell structure plus coherent `Table.Columns` metadata under add/delete and serializer checks;
- no silent loss → unsupported object/reference classes force compatibility mode; non-main payload hashes preserved for editable files;
- chart/diagram handling → explicit marker/placeholder behavior only in read-only compatibility mode;
- PDF viewing → first-party `Windows.Data.Pdf`, page/zoom checks plus independent open-admission and render-presentation freshness fences;
- installed PDF-path evidence → verification startup returns explicit success; zero process exit requires successful admission + first render before normal close;
- explicit save boundary → direct write with bounded restore/delete attempt; dual write/recovery failure exposes integrity uncertainty;
- zero app state → source/write-boundary audit plus filesystem residue test;
- close means close → no resident mechanisms + explicit exit + process smoke test after successful PDF render;
- install/uninstall → finite ownership manifest + registry/shortcut/path verification;
- no third-party dependencies → project/reference and published-file dependency census;
- user-document preservation → uninstall has no user-path input and deletes only fixed product-owned locators.

## Recovery/reopen

Evidence that an older Open intent can replace newer state, a post-prompt edit can be discarded by late admission, or stale Open status can overwrite newer UI reopens shell transition HOW. A failed table/formatting/search round-trip or table descriptor-coherence check reopens that DOCX slice. A request for editable shape/chart/footnote/TOC or richer compatibility-preview presentation reopens WHAT/HOW. A stale PDF render mutating a replaced session, PDF render/process-lifecycle failure, or freshness-fence defect reopens PDF/process HOW. A verification-mode zero exit without successful installed PDF admission/render reopens the verification oracle. A save path that hides unresolved target integrity after failed recovery reopens the explicit writer HOW. A residue failure reopens install/uninstall or runtime write ownership. Persistent UI customization reopens the zero-state Decision. Any implementation discovery may reopen only the affected target/design slice.
