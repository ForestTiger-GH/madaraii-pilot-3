# Decisions

Current Decision owner for MiniDoc 0.1 lineage.

## D-0001 — Bootstrap Work Architecture

**Status:** accepted.  
Use `WA-0001` / `MW-PILOT3-0001`; keep raw input, developed input, Work State, Decisions, Questions, Research, Science, WHAT, HOW, Product, Evidence and closure roles distinct. Defer Product structure until Research/HOW.

## D-0002 — First-party Windows stack

**Status:** accepted.  
Use .NET 10 WPF for the desktop shell/editor and `Windows.Data.Pdf`/`Windows.Storage` for PDF viewing. Publish self-contained `win-x64`. Add no Product `PackageReference` and no external document/PDF engine.

## D-0003 — Initial fail-closed DOCX subset

**Status:** superseded in its feature boundary by `D-0010`; its fail-closed principle remains accepted.  
The original paragraph/run-only subset was sufficient for `INBOX-0001` but became stale after `INBOX-0002` made simple tables and richer formatting mandatory.

## D-0004 — Zero runtime technical state

**Status:** accepted.  
MiniDoc persists no settings, cache, recents, autosave, logs, telemetry, or technical temp state. Runtime writes only an explicit user-selected document destination.

## D-0005 — Memory-staged save over temp-file atomicity

**Status:** accepted.  
Build/validate DOCX entirely in memory, then write the explicit target with best-effort restoration on ordinary write failure. Do not create application temp/save-sidecar files. Accept and document lack of guaranteed power/process-failure atomic overwrite.

## D-0006 — Per-machine PowerShell installation

**Status:** accepted.  
Use official PowerShell/Windows mechanisms to copy the self-contained payload into Program Files, create all-users shortcuts, and register one HKLM Uninstall entry. Do not introduce MSI/MSIX authoring/tool dependencies in the first release.

## D-0007 — Windows 11 x64 target

**Status:** accepted.  
Commit first-release support to Windows 11 x64. Other Windows variants may work but stay outside the verified promise until separately tested.

## D-0008 — Explicit process-exit containment

**Status:** accepted; runtime verification obligation fulfilled for the tested CI Windows configuration.  
After the main window has definitively closed and resource disposal runs, call `Environment.Exit(0)`. This guards the “closed means closed” invariant against a documented historical WPF/Windows PDF process-lifecycle risk.

`EV-0001` / `VERIFICATION-0001` establish that the installed exact predecessor candidate opened/rendered a PDF through the normal shell path, received normal main-window `Close()`, and terminated within the bounded verification window on Windows Server 2025 `10.0.26100`. Later `EV-E001-02` / `VERIFICATION-E001-02` strengthen the installed-path success oracle for the current Product by requiring successful selected-document admission and initial PDF render before zero exit. Interactive Windows 11 local qualification remains a verification reliance limit under Q-0006, not a pending design Decision.

## D-0009 — Separate Product Architecture

**Status:** accepted.  
Maintain `PA-0001` because UI/session control, DOCX semantics, PDF rendering, explicit document writes, installation, and verification have different failure/evolution boundaries. Keep the architecture to responsibility allocation; avoid framework-like abstraction layers.

## D-0010 — Revised editable DOCX envelope

**Status:** accepted.  
Keep fail-closed editable admission, but extend the first-release owned main-story subset to conservative simple rectangular tables plus richer direct formatting. Supported formatting includes font family/size, text colour, bold/italic/single underline, text highlight, paragraph alignment and bounded clear RGB cell fill. Reject or route to compatibility mode on merges, nested/styled/floating tables, unsupported table properties, styles-based content semantics, arbitrary run/paragraph shading, drawings/fields/references and unknown material markup.

**Driver:** `INBOX-0002` makes tables and richer formatting requirements; `RT-005` establishes a bounded WPF/WordprocessingML implementation path.

## D-0011 — Word-like Ribbon information architecture

**Status:** accepted.  
Use the first-party WPF Ribbon to organize the application around a File/application menu, Quick Access commands, Home, Insert and View tabs, plus bounded contextual table controls where useful. The design follows familiar Word-like command grouping without Microsoft branding or a feature-for-feature visual clone.

Persistent Ribbon/QAT personalization remains outside the first release because it would create application-owned persistent state contrary to `D-0004`. Session-local minimization/contextual presentation is permitted.

## D-0012 — Graphics and diagram compatibility boundary

**Status:** accepted for the compatibility-only/editability boundary; richer preview clauses superseded for current MiniDoc 0.1 by `D-0015`.  
Word pictures, shapes, charts, SmartArt, grouped/canvas drawings and other unsupported drawing semantics remain outside editable/save authority and therefore force read-only compatibility mode. Shape creation/editing and native chart/diagram editing remain outside the release. A chart/SmartArt object must never be reduced to a replacement raster image during editable save.

Historical versions of this Decision permitted showing a relationship-backed raster/fallback preview when safely resolvable. That presentation clause is no longer current for MiniDoc 0.1; current presentation semantics are owned by `D-0015`, Target WHAT revision 3 and Target HOW revision 5.

## D-0013 — Footnotes, fields and TOC remain compatibility features

**Status:** accepted for the compatibility-only/editability boundary; resolved-notes-body presentation clause superseded for current MiniDoc 0.1 by `D-0015`.  
Footnote/endnote references and field/TOC semantics remain outside editable/save authority and therefore force read-only compatibility mode. Footnote authoring and TOC/field generation or recalculation remain outside the release.

Historical versions of this Decision allowed compatibility extraction to present resolved notes-body text where available. That richer presentation clause is no longer current for MiniDoc 0.1; current presentation semantics are owned by `D-0015`, Target WHAT revision 3 and Target HOW revision 5.

## D-0014 — Search/replace is structure-preserving editor logic

**Status:** accepted.  
Implement Find, Replace and Replace All in product code over text-bearing WPF ranges. Replacement may modify text only and must not cross table/object structural boundaries. Native WPF editing commands continue to own selection, cut/copy/paste/undo/redo; paste is normalized to plain text to keep the serializer boundary controlled.

## D-0015 — Current compatibility presentation is marker/text-only

**Status:** accepted current MiniDoc 0.1 decision.  
Compatibility mode presents safely extractable main-story text and explicit labeled markers/placeholders for unsupported graphics, notes references, fields and related semantics. Current 0.1 does not dereference package media relationships to render graphics/chart/SmartArt/shape previews and does not dereference notes parts to append footnote/endnote bodies.

Stored field-result/main-story text may appear where it is already safely extractable, but MiniDoc does not evaluate or recalculate fields/TOC. The original package remains read-only and untouched.

Relationship-backed visual preview extraction or resolved notes-body presentation requires a later explicit Target/Design change; it is not latent current functionality.

**Driver:** second Jester owner-mismatch signal, `WHAT-E001-02`, `HOW-E001-02`, and `RECON-E001-0009`; current Product and public `docs/COMPATIBILITY.md` already implement/describe this boundary.
