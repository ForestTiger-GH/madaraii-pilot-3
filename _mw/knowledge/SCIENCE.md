# SCIENCE-0001 — Current Scientific Knowledge

**Status:** admitted current scientific owner for `WORK-0001`  
**Baseline:** `RP-0001` + research delta `RP-0002`, cutoff 2026-09-11  
**Consumers:** Target WHAT/HOW formation, implementation planning, verification challenge

This maintained owner assimilates the completed Research Results. Research history remains under `_mw/research/` for source challenge; ordinary Target formation can rely on this owner without rereading it.

## S1 — DOCX is a package with several independent semantic surfaces

Open XML word-processing documents are package graphs made of parts and relationships. The main story commonly uses `document/body/p/r/t`, while headers, footers, comments, settings, styles, footnotes, endnotes, media, charts, diagrams and other stories/parts can carry independent meaning. Therefore “read visible text and write a new DOCX” is not a safe round trip for arbitrary documents.

**Grounds:** ECMA-376; Microsoft WordprocessingML/package documentation.  
**Engineering implication:** compatibility admission must precede editable mode. Unowned package parts can be retained; unsupported semantics that would be affected by main-story rewrite must block edited saving.

## S2 — Editable admission must be feature-specific and fail closed

A small editor can safely support a deliberate WordprocessingML subset, provided it recognizes every material construct that enters the rewrite boundary. Unknown markup, style-dependent behavior, tracked changes, fields, drawings, unsupported table semantics and similar structures cannot be treated as ordinary text merely because visible text can be extracted.

**Grounds:** ECMA-376 markup breadth and compatibility model; `RT-001`, `RT-005`, `RT-007`, `RT-008`.  
**Engineering implication:** each newly supported construct needs structural admission rules, editor representation, serializer rules and round-trip evidence.

## S3 — In-memory package transformation supports the cleanliness invariant

.NET base compression/XML/file APIs are sufficient to read ZIP entries, parse a bounded XML subset, rebuild owned parts, and generate an output package in memory. No application temp/cache file is inherently required by this mechanism.

**Grounds:** Microsoft .NET API documentation; `RT-001`.  
**Limit:** an in-memory package can consume substantial memory, so bounded package/entry/XML size limits are required.

## S4 — WPF supplies the rich-text and simple-table editing surface

WPF `RichTextBox` edits `FlowDocument` content and supports formatted text plus `Table`, `TableRowGroup`, `TableRow`, `TableCell`, images and UI elements. It is therefore sufficient as the first-party editing surface for text/direct formatting and a conservative simple-table subset.

**Grounds:** Microsoft WPF Flow Document documentation; `RT-002`, `RT-004`, `RT-005`.  
**Limits:** WPF flow layout is not Word layout. `RichTextBox` does not provide Word pagination and does not itself supply a complete find/replace experience.

## S5 — A conservative simple-table subset is feasible

WordprocessingML tables are `w:tbl` structures containing rows/cells and potentially substantial table/grid/style/merge/layout semantics. A rectangular non-nested table with stable column count, no merges, no style references, no floating positioning and cells restricted to the supported paragraph/run subset can be mapped to a WPF `Table` and serialized independently.

**Grounds:** ECMA-376; Microsoft WordprocessingML table/schema documentation; `RT-005`.  
**Engineering implication:** a table that merely looks simple is insufficient. Editable admission must inspect table/grid/row/cell properties and fail closed on unsupported semantics.

## S6 — Text highlight and shading are different WordprocessingML semantics

Text highlighting is represented by `w:highlight`; run/paragraph/table/cell shading is represented by `w:shd`. Highlight can supersede run shading visually. A product that maps both to one persisted background property risks semantic loss.

**Grounds:** Microsoft `Highlight` and `Shading` schema documentation; `RT-005`.  
**Engineering implication:** v1 may support text highlight distinctly and a bounded clear RGB cell fill, while leaving arbitrary pattern/theme/run/paragraph shading unsupported.

## S7 — WPF has a first-party Ribbon suitable for Word-like organization

`System.Windows.Controls.Ribbon` supplies a WPF Ribbon, Application Menu, Quick Access Toolbar, tabs, groups and contextual UI. This supports a familiar Word-like command structure without importing a third-party UI framework.

**Grounds:** Microsoft WPF Ribbon documentation; Windows Ribbon control model; `RT-006`.  
**Engineering implication:** the useful analogue is information architecture: File, Home, Insert, View and contextual table controls. Microsoft branding/artwork and a feature-for-feature clone are unnecessary.

## S8 — Search/replace is local editor logic, not a new dependency

Selection/cut/copy/paste/undo/redo are native WPF editing capabilities. Search is not a standard built-in `RichTextBox` feature. A bounded find/replace implementation can walk `TextPointer`/text runs and map textual matches back to editable text ranges while refusing replacements that cross non-text structural boundaries.

**Grounds:** Microsoft WPF Flow Document/RichTextBox documentation; `RT-006`.  
**Engineering implication:** Replace All must preserve structural positions and operate only within text-bearing ranges.

## S9 — Windows supplies a first-party PDF-to-image rendering path

`Windows.Data.Pdf` loads PDFs, exposes pages, and renders a page to an in-memory random-access stream. This is enough for page-at-a-time view-only behavior. A 400% zoom ceiling stays within a bounded first-version viewer.

**Grounds:** Microsoft Windows API documentation; `RT-002`.  
**Limit:** password-protected behavior and broad malformed-PDF compatibility remain outside the promise unless tested/implemented.

## S10 — PDF rendering has a material shutdown uncertainty in WPF

An open Microsoft/CsWinRT issue reports historical WPF process survival after `PdfPage.RenderToStreamAsync`. Its configuration differs from the target and it does not establish a current defect, but it defeats any assumption that normal disposal alone proves process termination.

**Grounds:** microsoft/CsWinRT issue #1249; `RT-002`.  
**Engineering implication:** explicit process-exit containment plus Windows-local process verification are required before claiming “closed means closed.”

## S11 — Raster pictures are tractable; Word drawing objects are a different class

A narrowly recognized embedded raster picture can be resolved through package relationships and decoded with Windows/WPF imaging. Word shapes, grouped drawings and canvases use DrawingML/Office extension geometry and often compatibility markup. WPF shapes alone do not establish equivalent WordprocessingML semantics.

**Grounds:** Microsoft WordprocessingML Drawing specification and OOXML rich-content guidance; `RT-007`.  
**Engineering implication:** native shape creation/editing is a separate capability. Existing unsupported drawing objects should force a safe compatibility route unless a precise opaque-preservation contract is implemented and verified.

## S12 — Charts and SmartArt are multi-part semantic objects, not guaranteed pictures

Native Word charts reference chart markup, chart relationships and an embedded Excel workbook; SmartArt uses multiple parts for data/layout and related presentation information. A package may contain an image/fallback associated with an object, but a universal raster preview is not guaranteed.

**Grounds:** Microsoft Office OOXML rich-content documentation; `RT-007`.  
**Engineering implication:** v1 can show a verified associated preview when one exists, otherwise an explicit object placeholder. It cannot truthfully promise universal chart/SmartArt rendering without implementing a substantial renderer or adding a forbidden engine.

## S13 — Footnotes are a separate story and require reference semantics

The main story uses `w:footnoteReference`; footnote content lives in a separate Footnotes part and may itself contain block-level content. Word-like bottom-of-page placement depends on pagination behavior absent from the selected WPF editor model.

**Grounds:** Microsoft WordprocessingML structure and Footnote/FootnoteReference documentation; `RT-008`.  
**Engineering implication:** full footnote editing is larger than inline superscript editing. A first version may provide explicit compatibility viewing/extraction while deferring creation/editing.

## S14 — TOC is field behavior, not static paragraph generation

Complex Word fields carry begin/separate/end field markers, field instructions (`w:instrText`) and stored results. A Word table of contents depends on field semantics plus heading/style and update behavior. Displaying cached result text does not constitute recalculation.

**Grounds:** Microsoft FieldChar/FieldCode documentation; `RT-008`.  
**Engineering implication:** TOC creation/update would require a field/style engine not otherwise needed by the first product. Documents with fields/TOC should remain compatibility-mode inputs until that capability is commissioned.

## S15 — Persistent Ribbon personalization conflicts with zero-runtime-state

Contextual Ribbon behavior and session-level minimization need no persistent application state. Remembering arbitrary user customization across restarts requires state ownership and lifecycle that the current Product explicitly forbids.

**Grounds:** WPF/Windows Ribbon capability model plus current accepted zero-state requirement; `RT-006`.  
**Engineering implication:** v1 may be Word-like and responsive without persistent customization. Persistent personalization needs a later explicit Decision changing the state model.

## S16 — First-party-only deployment is feasible with modern .NET and Windows

A Windows-specific .NET 10 target can consume WinRT APIs directly and WPF is part of Windows Desktop .NET. A self-contained `win-x64` publish carries the required .NET runtime. No application `PackageReference` is required for the selected implementation.

**Grounds:** Microsoft modern desktop WinRT and .NET deployment documentation; `RT-002`, `RT-003`.  
**Validity:** target Windows 11 x64.

## S17 — Normal installation can have a finite auditable ownership surface

A per-machine Program Files payload, all-users shortcuts, and one HKLM Uninstall registration create a recognizable installed desktop application without an always-running helper. The product can carry zero application-owned runtime data state. Uninstall can target only declared resources and leave all user documents untouched.

**Grounds:** Microsoft uninstall registry/special-folder/PowerShell documentation; `RT-003`.  
**Limit:** Windows UI presence, UAC path, shortcut behavior and residue require Windows execution evidence.

## S18 — Cleanliness and crash-atomic saving are competing concerns in a zero-temp design

A memory-staged direct replacement avoids temp artifacts, yet ordinary overwrite semantics truncate an existing target. Best-effort in-memory restoration can cover ordinary exceptions after process survival, while power/process failure during final write remains non-atomic. Same-directory temporary/replace would strengthen crash durability but introduces a filesystem artifact and possible crash residue.

**Grounds:** .NET file semantics; `RT-001`, `RT-004`.  
**Engineering implication:** retain the explicit trade-off unless the human changes priority; Save As remains the safer route for valuable originals.

## Source registry

| ID | Source | Role |
|---|---|---|
| `SRC-ECMA-376` | https://ecma-international.org/publications-and-standards/standards/ecma-376/ | normative format architecture |
| `SRC-MS-WML` | https://learn.microsoft.com/en-us/office/open-xml/word/structure-of-a-wordprocessingml-document | package/story/main markup orientation |
| `SRC-MS-WPF-FLOW` | https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/flow-document-overview | editor/table capability and limits |
| `SRC-MS-WPF-RIBBON` | https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.ribbon?view=windowsdesktop-10.0 | first-party Ribbon capability |
| `SRC-MS-RIBBON-TAB` | https://learn.microsoft.com/en-us/windows/win32/windowsribbon/windowsribbon-controls-tab | tab/contextual organization model |
| `SRC-MS-HIGHLIGHT` | https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.highlight | `w:highlight` semantics |
| `SRC-MS-SHADING` | https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.shading | `w:shd` semantics |
| `SRC-MS-OOXML-RICH` | https://learn.microsoft.com/en-us/office/dev/add-ins/word/create-better-add-ins-for-word-with-office-open-xml | chart/SmartArt multi-part structure |
| `SRC-MS-ODRAWXML` | https://learn.microsoft.com/en-us/openspecs/office_standards/ms-odrawxml/6aaf2ba5-6974-4560-9983-3fcd7fead7cb | Word drawing extension/AlternateContent |
| `SRC-MS-FOOTNOTE-REF` | https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.footnotereference | footnote reference semantics |
| `SRC-MS-FOOTNOTE` | https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.footnote | footnote story content |
| `SRC-MS-FIELDCHAR` | https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.fieldchar | complex field boundaries/results |
| `SRC-MS-FIELDCODE` | https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.fieldcode | field instruction semantics |
| `SRC-MS-WINRT-DESKTOP` | https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/winrt-apis-desktop-apps | WinRT access from modern .NET |
| `SRC-MS-PDF` | https://learn.microsoft.com/en-us/uwp/api/windows.data.pdf?view=winrt-26100 | PDF API capability |
| `SRC-CSWINRT-1249` | https://github.com/microsoft/CsWinRT/issues/1249 | negative lifecycle evidence |
| `SRC-MS-DOTNET-DEPLOY` | https://learn.microsoft.com/en-us/dotnet/core/deploying/ | deployment capability |
| `SRC-MS-DOTNET-WINDOWS` | https://learn.microsoft.com/en-us/dotnet/core/install/windows | supported OS/runtime context |
| `SRC-MS-UNINSTALL` | https://learn.microsoft.com/en-us/windows/win32/msi/uninstall-registry-key | uninstall registry convention |

## Completeness and freshness

The corpus is complete for the current first-release decisions: simple editable tables and richer direct formatting are scientifically supported; Word-like Ribbon organization is feasible first-party; graphics/fields/footnotes have an explicit safe compatibility boundary; PDF/lifecycle conclusions remain current. It makes no claim of full DOCX/PDF/Word coverage.

Raw Research can be amputated from ordinary Target use because material conclusions, challenges, limits and source routes required by downstream work are present here.

Reopen on target OS/runtime change, failed round-trip evidence, requirement for editable shapes/charts/footnotes/TOC, contradictory Windows-local evidence, persistent UI-customization requirement, or materially new first-party API capability.
