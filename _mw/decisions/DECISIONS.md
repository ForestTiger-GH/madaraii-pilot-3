# Decisions

Current Decision owner for `WORK-0001`.

## D-0001 — Bootstrap Work Architecture

**Status:** accepted.  
Use `WA-0001` / `MW-PILOT3-0001`; keep raw input, developed input, Work State, Decisions, Questions, Research, Science, WHAT, HOW, Product, Evidence and closure roles distinct. Defer Product structure until Research/HOW.

## D-0002 — First-party Windows stack

**Status:** accepted.  
Use .NET 10 WPF for the desktop shell/editor and `Windows.Data.Pdf`/`Windows.Storage` for PDF viewing. Publish self-contained `win-x64`. Add no Product `PackageReference` and no external document/PDF engine.

**Driver:** it satisfies the dependency constraint using Microsoft platform/runtime capabilities while retaining a practical rich-text surface and PDF renderer.

## D-0003 — Fail-closed DOCX editing

**Status:** accepted.  
Grant editable mode only to the exact Transitional paragraph/run/direct-formatting subset in `TARGET-WHAT-0001`. Open structurally richer files as explicit read-only compatibility views or reject unsupported containers. Preserve unowned package parts for editable documents.

**Driver:** silent lossy round trips are a prohibited outcome.

## D-0004 — Zero runtime technical state

**Status:** accepted.  
MiniDoc persists no settings, cache, recents, autosave, logs, telemetry, or technical temp state. Runtime writes only an explicit user-selected document destination.

## D-0005 — Memory-staged save over temp-file atomicity

**Status:** accepted.  
Build/validate DOCX entirely in memory, then write the explicit target with best-effort restoration on ordinary write failure. Do not create application temp/save-sidecar files. Accept and document lack of guaranteed power/process-failure atomic overwrite.

**Driver:** the Commission gives exceptional priority to controlled filesystem cleanliness. Reopen if crash-atomic durability becomes a higher-priority requirement.

## D-0006 — Per-machine PowerShell installation

**Status:** accepted.  
Use official PowerShell/Windows mechanisms to copy the self-contained payload into Program Files, create all-users shortcuts, and register one HKLM Uninstall entry. Do not introduce MSI/MSIX authoring/tool dependencies in v0.1.

## D-0007 — Windows 11 x64 target

**Status:** accepted.  
Commit v0.1 support to Windows 11 x64. Other Windows variants may work but are outside the verified product promise until separately tested.

## D-0008 — Explicit process-exit containment

**Status:** accepted pending runtime verification.  
After the main window has definitively closed and resource disposal runs, call `Environment.Exit(0)`. This guards the “closed means closed” invariant against a documented historical WPF/Windows PDF process-lifecycle risk. The exact shipped configuration still requires Windows-local process verification.

## D-0009 — Separate Product Architecture

**Status:** accepted.  
Maintain `PA-0001` because UI/session control, DOCX semantics, PDF rendering, explicit document writes, installation, and verification have different failure/evolution boundaries. Keep the architecture to responsibility allocation; avoid framework-like abstraction layers.
