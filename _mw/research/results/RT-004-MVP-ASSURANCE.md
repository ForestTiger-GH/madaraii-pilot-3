# RT-004 — Minimum useful UX and assurance boundary

**Question:** What minimum editor/viewer UX and verification strategy makes v0.1 practically useful and its claims testable?  
**Scope:** user actions, formatting surface, viewer controls, negative behavior, evidence model.  
**Non-goals:** Ribbon parity, pagination/layout fidelity, print, collaboration, spellcheck commitment, accessibility certification.

## Findings

WPF’s standard rich-text control already carries normal text-selection, keyboard editing, clipboard, undo/redo, and formatting command semantics. A small useful editor therefore needs a thin product shell around those primitives rather than a custom text engine or a Word-like Ribbon.

A coherent v0.1 surface is:

- File: New, Open, Save, Save As, Exit;
- Edit: Undo, Redo, Cut, Copy, Paste, Select All;
- formatting: Bold, Italic, Underline, font family, font size, text color, paragraph alignment;
- document-status banner when a DOCX is read-only because its structure exceeds the editable subset;
- PDF: previous/next page, current page/total, zoom down/up with a bounded 50–400% range;
- unsaved-change prompt before replacing/closing an editable document.

The editor should force clipboard paste to plain text in v0.1. This prevents WPF from injecting FlowDocument structures (tables/images/hyperlinks) that the DOCX serializer cannot represent. Formatting may then be applied using the supported toolbar/commands. The serializer still validates the editor tree before Save and refuses any structure outside the supported internal model.

## Verification model

Claims divide into three evidence classes:

1. **Repository/static:** no `PackageReference`; source contains no application writes to AppData/Temp; only declared platform APIs; DOCX scanner gates editing; installer/uninstaller target exact owned paths; documentation matches target.
2. **Portable algorithmic tests:** DOCX fixtures can be generated in repository tests and used to prove editable-subset load/save, read-only fallback, preservation of opaque package entries, signature rejection, resource-limit behavior, and output re-open validation. These tests can run anywhere a compatible .NET SDK can compile the project; WPF/WinRT itself remains Windows-specific.
3. **Windows-local system/runtime:** build/publish, install/UAC, shortcuts, Installed Apps entry, GUI actions, native PDF rendering, save behavior, process termination after PDF render, filesystem/registry residue, uninstall cleanup.

The Windows verification script should fail on a surviving `MiniDoc` process and on any remaining declared installer-owned artifact. It should print explicit MANUAL steps for GUI/DOCX/PDF behavior that cannot be safely automated without introducing another UI automation dependency.

## Strongest failure modes and controls

- Complex DOCX opens as editable and gets flattened → fail-closed compatibility scanner + tests.
- Non-main DOCX parts disappear → compare entry names and SHA-256 payload hashes before/after save, excluding the intentionally changed main part.
- Parser resource exhaustion → package/entry/XML size limits and DTD prohibition.
- Rich clipboard content injects unsupported editor structure → paste as plain text + pre-save internal validation.
- PDF render leaves process alive → dispose page/session, explicit post-window process exit, Windows smoke test.
- Runtime creates hidden state → zero-state design + Windows residue scan.
- Uninstall removes user document → uninstall paths are fixed product-owned paths only; no history of opened file paths is persisted.
- Save fails midway → in-memory generation and best-effort restoration of pre-existing target bytes on ordinary I/O failure; document power-failure non-atomicity.

## Assessment

The proposed UX is small but forms a complete user journey. Its honesty depends on conspicuous compatibility mode and Save refusal where the product cannot preserve document semantics. Assurance must keep static/algorithmic proof separate from Windows-local claims.

## Sources

- Microsoft Learn, WPF `RichTextBox` overview and editing commands: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/richtextbox-overview
- Microsoft Learn, Windows PDF APIs: https://learn.microsoft.com/en-us/uwp/api/windows.data.pdf?view=winrt-26100
- Microsoft Learn, current .NET on Windows support: https://learn.microsoft.com/en-us/dotnet/core/install/windows

**Reopen:** material UX expansion, format support expansion, failure of any named acceptance claim, or introduction of runtime state/background behavior.
