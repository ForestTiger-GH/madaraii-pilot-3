# MiniDoc 0.1 — Compatibility Contract

This document is the user/engineering compatibility summary for the accepted MiniDoc target. The authoritative engineering semantics remain in `_mw/knowledge/TARGET-WHAT.md` and `_mw/knowledge/TARGET-HOW.md`.

## Editable DOCX subset

MiniDoc grants edit/save authority only after package validation and compatibility analysis. The current editable subset is Transitional WordprocessingML with:

- paragraphs and runs;
- text, tabs and ordinary line breaks;
- bold, italic and single underline;
- direct font family and size;
- opaque RGB text colour and text highlight;
- paragraph alignment, bounded indentation/spacing and opaque paragraph fill;
- simple rectangular tables without merged/nested cells, using ordinary supported paragraph/run content inside cells;
- supported simple direct table/cell visual properties produced by MiniDoc;
- optional final section properties retained as session metadata.

Saving rebuilds only MiniDoc-owned main-story semantics. Original non-main package entries are payload-preserved and output is re-opened through MiniDoc's compatibility analyzer before disk mutation.

## Read-only / unsupported DOCX boundary

Material structures outside that model block edited saving. This includes, among other things:

- style-driven/list-numbering semantics;
- hyperlinks and fields, including TOC field semantics;
- footnotes/endnotes and their cross-part references;
- drawings, shapes, charts and SmartArt as editable objects;
- tracked changes, comments, bookmarks/content controls where they carry material main-story semantics;
- equations, embedded objects and unknown material markup;
- merged/nested/structurally irregular tables;
- Strict OOXML and signed packages;
- encrypted/non-ZIP containers.

A compatibility view can extract text and, where a directly usable preview/image already exists in the package, surface that preview. MiniDoc never represents such a preview as equivalent to the editable source object.

## PDF boundary

PDF is local, view-only, page-at-a-time rendering through Windows APIs. Supported viewer operations are page navigation and 50–400% zoom. Editing, annotation, OCR, forms, printing, search, conversion and password-protected PDF support are outside 0.1.

## Fidelity statement

MiniDoc is not a Word layout engine. Editable admission means the document's supported semantics can be represented by MiniDoc's bounded model; it does not mean pixel-identical pagination or typography relative to Microsoft Word.
