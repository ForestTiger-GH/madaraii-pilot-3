# TARGET-WHAT-0001 — MiniDoc 0.1

**Status:** accepted target baseline for `WORK-0001`  
**Product identity:** `MiniDoc`  
**Target version:** `0.1.0`  
**Environment:** Windows 11 x64  
**Primary beneficiary:** a user who wants a deliberately small local DOCX editor and PDF viewer

## Purpose and value boundary

MiniDoc is a small installed Windows desktop application for opening simple DOCX documents for basic editing and opening PDF documents for local viewing. It favors explicit compatibility limits, simple lifecycle, and system cleanliness over broad Office/PDF feature coverage.

## User-visible product

A normal launch opens one main window. File → Open uses the standard Windows file picker for `.docx` and `.pdf`. The application also supports New, Save, Save As, and Exit for editable DOCX work.

### DOCX editable mode

A DOCX may enter editable mode only when its main WordprocessingML story is within the v0.1 supported subset:

- Transitional WordprocessingML main document;
- ordinary paragraphs and runs;
- text, tabs, ordinary line breaks;
- bold, italic, single underline;
- explicit font family, font size, RGB text color;
- paragraph alignment: left, center, right, justify;
- an optional existing final section-properties element retained opaquely.

The editor supplies Undo/Redo, Cut/Copy/Paste/Select All and the supported formatting actions. Paste inserts plain text only.

Save and Save As produce `.docx`. The application preserves all original non-main package entries at their payload level and rebuilds only the supported main story. The output is revalidated by MiniDoc before disk write.

### DOCX compatibility mode

A structurally valid DOCX whose material main-story semantics exceed the supported subset opens in an explicit **read-only compatibility view** showing approximate extracted text and the reason editing is unavailable. Save/Save As of an edited DOCX are disabled. The original file remains untouched.

Triggers include material unsupported structures such as tables, drawings/images, hyperlinks, fields, list/numbering semantics, styles referenced by content, tracked changes, content controls, comments/bookmark anchors in the main story, equations, embedded objects, unknown material markup, Strict OOXML, and signed packages. Encrypted/non-ZIP packages produce an explicit unsupported/error result rather than an editable approximation.

The product never claims layout fidelity with Microsoft Word.

### PDF view mode

PDF is view-only. The user can move to previous/next page, see current/total page numbers, and zoom within 50–400%. Rendering occurs inside the application. No PDF edit, annotation, print, search, form filling, or conversion is promised. Password-protected PDFs are outside v0.1 support and receive an explicit message.

## Lifecycle and cleanliness invariants

- Closing the main window terminates MiniDoc execution; no tray process, service, scheduled task, updater, helper daemon, or background resident component exists.
- Runtime creates no MiniDoc application state, cache, recent-files list, telemetry file, autosave copy, crash-log file, or temp file in AppData, Temp, Documents, or another hidden/service location.
- Runtime file writes occur only when the user explicitly selects a Save/Save As destination for a user document.
- Installer-owned machine state is finite: `%ProgramFiles%\MiniDoc`, the documented all-users desktop and Start Menu shortcuts, and one HKLM Uninstall registration.
- Uninstall removes installer-owned technical state. User documents are always outside uninstall ownership.
- No network operation is part of runtime behavior.

## Installation/build commitments

The repository contains a reproducible PowerShell build using the official .NET SDK, plus install/uninstall scripts. Publishing is self-contained for `win-x64`, so the installed app has no separately installed .NET runtime prerequisite. The build process may use Microsoft SDK/runtime packs and build-tool caches; the supplied build script redirects project-controlled caches/intermediates to repository-local build/artifact paths where feasible.

Installation is per-machine and requires elevation. It creates a desktop shortcut, Start Menu shortcut, and Windows Uninstall registration. No file association is registered in v0.1.

## Size and input limits

DOCX parsing uses conservative resource limits: package ≤64 MiB, ≤4096 entries, aggregate uncompressed entries ≤256 MiB, main-document XML ≤16 MiB. A limit failure is explicit. These are MiniDoc limits, not DOCX specification limits.

## Save durability limit

MiniDoc builds DOCX output fully in memory before final write and retains the prior target bytes for best-effort restoration after ordinary write failure. It creates no temp save artifact. Final overwrite is **not guaranteed power/process-failure atomic**: a machine/process failure during replacement can damage the target. For valuable originals, Save As to a new file is the safer workflow.

## Prohibited outcomes

- silently saving a DOCX main story that exceeded the recognized editable subset;
- deleting unowned user documents during uninstall;
- persistent process after normal window closure;
- undeclared application runtime state/cache/temp files;
- third-party runtime/product dependency;
- representing MiniDoc as a full Word or Acrobat replacement.

## Non-goals

Full Office compatibility, DOC/DOCM, macros, style/list/table/image/field/comment/track-change editing, Word pagination fidelity, PDF editing, printing, OCR, cloud sync, collaboration, auto-update, file associations, background operation, and future-format support.

## Completion semantics

The target is complete when repository implementation and static/algorithmic evidence support the code-level claims, a reproducible Windows procedure exists for system/runtime claims, and all claims requiring actual Windows execution remain explicitly marked pending until such execution occurs.
