# MiniDoc 0.1 — User Guide

MiniDoc is a small Windows 11 x64 document application for basic DOCX editing and local PDF viewing. It deliberately supports a bounded DOCX subset and reports richer documents as read-only compatibility views rather than silently damaging unsupported content.

## Main interface

The window uses a Word-like Ribbon structure.

- **File**: New, Open, Save, Save As, Exit.
- **Home / Quick Access**: Undo/Redo, Cut/Copy/Paste, Select All, Find/Replace, font and paragraph formatting.
- **Insert**: simple editable tables.
- **View**: PDF page navigation and zoom are shown when a PDF is active.

One document is active at a time.

## DOCX editing

Supported editable content includes ordinary paragraphs/runs, direct font formatting, text colour, text highlight, paragraph alignment/indent/spacing/fill, and simple rectangular tables. Table cells contain ordinary paragraph/run content. The editor supports creating a basic table and adding/removing rows or columns.

Find and Replace work within the currently loaded editable document. Paste is intentionally plain-text so clipboard HTML/RTF/object markup cannot silently enter the supported save model.

### Compatibility mode

If a DOCX contains material semantics outside the editable subset, MiniDoc opens a read-only compatibility view and explains why editing is disabled. Examples include shapes, charts/SmartArt, fields/TOC, footnotes/endnotes, style-driven content, drawings without a safely representable preview, tracked changes, comments, equations, content controls, hyperlinks, list semantics, signatures, Strict OOXML, and other unsupported markup.

For supported graphical objects in an unsupported document, MiniDoc may display an available package preview/image in compatibility presentation. This is a view aid only; MiniDoc does not claim full Word layout fidelity or editable drawing/chart semantics.

## PDF viewing

Open a `.pdf` through File → Open. PDF is view-only. MiniDoc renders the current page locally using Windows PDF APIs. Use Previous/Next and zoom from 50% to 400%.

Password-protected PDFs are outside MiniDoc 0.1 support.

## Saving and cleanliness

MiniDoc writes a user document only after an explicit Save or Save As. It creates no MiniDoc settings, recent-file database, telemetry, autosave, document cache, background service, updater, tray process, or application temp state.

DOCX output is built and checked in memory before the final filesystem write. Overwrite recovery is best-effort for ordinary write errors; a process or machine failure during direct replacement is not crash-atomic. Use **Save As** for valuable originals when you want to keep the original file untouched.

## Closing

Closing the main window terminates MiniDoc. There is no resident background component.
