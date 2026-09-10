# TARGET-WHAT-0001 — MiniDoc 0.1, target revision 2

**Status:** accepted current target baseline for `WORK-0001` after `INBOX-0002`  
**Product identity:** `MiniDoc`  
**Target version:** `0.1.0`  
**Environment:** Windows 11 x64  
**Primary beneficiary:** a user who wants a deliberately small local Word-like DOCX editor and PDF viewer with explicit compatibility limits

## Purpose and value boundary

MiniDoc is an installed Windows desktop application for practical basic editing of a controlled DOCX subset and local PDF viewing. It provides familiar Word-like command organization while prioritizing safe document handling, transparent compatibility limits, simple process lifecycle and system cleanliness over broad Office/PDF feature coverage.

## User-visible application

A normal launch opens one main window with a first-party Ribbon-style command surface organized in familiar groups rather than a traditional flat demo toolbar. File commands, common quick-access actions and contextual controls remain visible only where they have real implemented behavior.

The application supports:

- New, Open, Save, Save As and Exit;
- Undo, Redo, Cut, Copy, Paste and Select All;
- Find, Replace and Replace All for text;
- DOCX editing within the supported subset;
- PDF viewing in the same application;
- clear mode/status/compatibility messages.

The UI may resemble Word in command grouping and interaction conventions. It does not copy Microsoft branding, proprietary artwork, full Ribbon inventory or claim UI parity.

## DOCX editable mode

A DOCX enters editable mode only when every material main-story construct within the rewrite boundary belongs to the supported Transitional WordprocessingML subset.

### Text and run formatting

Supported:

- ordinary paragraphs and runs;
- text, tabs and ordinary line breaks;
- bold, italic and single underline;
- explicit font family and font size;
- explicit RGB text colour;
- Word text highlighting using a bounded set of named highlight colours;
- plain-text paste into the current insertion/selection position.

### Paragraph formatting

Supported:

- left, center, right and justify alignment;
- bounded left/right paragraph indentation;
- bounded space before/after paragraphs;
- clear RGB paragraph background fill where it can be represented directly without theme/pattern semantics.

Lists/numbering, styles-based paragraph semantics and page-layout features remain outside the editable subset.

### Tables

Supported tables are deliberately simple:

- rectangular, non-nested tables;
- stable column count across ordinary rows;
- no horizontal/vertical merge;
- no table/row/cell style references or conditional styles;
- no floating/positioned table semantics;
- cells contain supported paragraphs/runs only;
- bounded clear RGB cell background fill.

The editor supports inserting a simple table and editing text/formatting inside its cells. Basic row/column add/delete commands are part of the target where they preserve rectangularity and the supported subset.

### Search and replace

Find locates text without flattening the document model. Replace/Replace All changes text only. Replacement cannot span table-cell boundaries, non-text object boundaries or another structural boundary that would make the operation ambiguous.

### Save safety

Save and Save As produce `.docx`. The application:

1. grants save authority only to a document admitted as editable;
2. validates the current in-memory document against the supported serializer model;
3. rebuilds only owned main-story semantics while retaining permitted envelope/section metadata;
4. preserves original non-main package entries at payload level unless the current supported edit explicitly owns a related part;
5. reopens and reclassifies generated output before filesystem mutation;
6. refuses saving when the editor state cannot be represented safely.

MiniDoc never silently turns an unsupported Word feature into ordinary text or an image during an editable save.

## DOCX compatibility mode

A structurally readable DOCX whose material semantics exceed the editable subset opens in explicit **read-only compatibility mode**. The original file remains untouched. The view provides best-effort readable content and states why editing is unavailable.

Compatibility triggers include, among others:

- merged/nested/styled/positioned/otherwise unsupported tables;
- Word shapes, grouped drawings, drawing canvases and unsupported DrawingML/VML;
- charts and SmartArt/diagram semantics;
- footnote/endnote references;
- simple or complex fields, including TOC field semantics;
- hyperlinks, numbering/list semantics, styles referenced by content, tracked changes, content controls, comments/bookmark anchors in the main story, equations, embedded objects and unknown material markup;
- Strict OOXML, signed packages or unsupported/encrypted containers.

### Graphic objects in compatibility mode

For embedded raster pictures and other graphic objects, the compatibility viewer attempts only evidence-backed presentation:

- a recognized raster image part may be shown when a valid relationship resolves it;
- for chart/SmartArt/shape objects, an associated image/fallback preview is shown only when package relationships/markup identify one safely;
- where no reliable preview exists, the view shows a labeled object placeholder rather than inventing a representation.

Native shape creation/editing and native chart/SmartArt editing are outside this release.

### Footnotes and TOC/fields in compatibility mode

Where safely resolvable, compatibility view may expose footnote/endnote body text and stored field-result text. It does not promise Word-like bottom-of-page footnote placement or recalculation/update of fields/TOC. Footnote authoring and TOC creation/update remain outside this release.

## PDF view mode

PDF is view-only. The user can move to previous/next page, see current/total page numbers, and zoom within 50–400%. Rendering occurs inside the application. PDF editing, annotation, printing, search, form filling and conversion are outside the release. Password-protected PDFs are unsupported with an explicit message.

## Word-like UI organization

The current release includes a bounded Ribbon information architecture:

- File/application surface: New, Open, Save, Save As, Exit;
- Quick Access: Save, Undo, Redo;
- Home: Clipboard, Font, Paragraph, Editing;
- Insert: Table and only other insertion commands that are actually supported;
- View: mode-relevant document/PDF viewing actions;
- contextual table controls when a supported table selection makes them meaningful.

Persistent user customization of Ribbon/QAT is outside the release because MiniDoc owns no runtime settings state. Session-local minimization/contextual presentation is allowed.

## Lifecycle and cleanliness invariants

- Closing the main window terminates MiniDoc execution; no tray process, service, scheduled task, updater, helper daemon or background resident component exists.
- Runtime creates no MiniDoc settings, cache, recent-files list, telemetry file, autosave copy, crash-log file or technical temp file in AppData, Temp, Documents or another service location.
- Runtime file writes occur only when the user explicitly chooses a Save/Save As destination for a user document.
- Installer-owned machine state is finite: `%ProgramFiles%\MiniDoc`, documented all-users Desktop and Start Menu shortcuts, and one HKLM Uninstall registration.
- Uninstall removes installer-owned technical state while preserving user documents.
- No network operation is part of runtime behavior.

## Installation/build commitments

The repository contains a reproducible PowerShell build using the official .NET SDK and install/uninstall scripts. Publishing is self-contained for `win-x64`, so the installed app does not require a separately installed .NET runtime. Build tooling may use Microsoft SDK/runtime packs and caches; the supplied build script redirects project-controlled caches/intermediates to repository-local build/artifact paths where feasible.

Installation is per-machine and requires elevation. It creates Desktop and Start Menu shortcuts and an Uninstall registration. No file association or background updater is registered.

## Size and input limits

DOCX parsing uses conservative resource limits: package ≤64 MiB, ≤4096 entries, aggregate uncompressed entries ≤256 MiB, main-document XML ≤16 MiB. Object/image extraction also applies bounded size checks. Limit failures are explicit MiniDoc limits, not DOCX specification limits.

## Save durability limit

MiniDoc builds DOCX output fully in memory before final write and retains prior target bytes for best-effort restoration after ordinary write failure. It creates no temporary save artifact. Final overwrite is not guaranteed power/process-failure atomic; a machine/process failure during replacement can damage the target. Save As to a new file is the safer workflow for valuable originals.

## Prohibited outcomes

- silently saving a main story that exceeded the recognized editable subset;
- flattening charts, SmartArt, shapes, fields or references into lossy replacements during editable save;
- deleting unowned user documents during uninstall;
- persistent MiniDoc process after normal window closure;
- undeclared runtime state/cache/temp files;
- third-party product/runtime/document-engine dependency;
- representing MiniDoc as a full Word or Acrobat replacement.

## Non-goals

Full Office compatibility; DOC/DOCM/macros; styles/list/numbering engine; merged/nested/complex tables; native shape/chart/SmartArt authoring; footnote editing; TOC/field generation or recalculation; Word pagination fidelity; PDF editing/printing/OCR; cloud sync/collaboration; auto-update; file associations; background operation; persistent Ribbon customization.

## Completion semantics

The target is complete when repository implementation and available static/algorithmic evidence support the code-level claims, a reproducible Windows procedure exists for system/runtime claims, and all claims requiring actual Windows execution remain explicitly marked pending until such execution occurs. Supported DOCX fixtures must demonstrate the enlarged table/formatting/search boundary, while compatibility fixtures must demonstrate fail-closed behavior for representative unsupported object/reference classes.
