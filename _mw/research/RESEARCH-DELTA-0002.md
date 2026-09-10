# Research Delta — RP-0002

**Status:** completed for the `INBOX-0002` target revision  
**Consumer:** `WORK-0001`, Scientific Knowledge delta, revised Target WHAT/HOW  
**Baseline/cutoff:** `SCIENCE-0001` + `INPUT-DEV-0002`; public standards/platform state checked 2026-09-11  
**Governing work kinds:** `RESEARCH_TOPIC_DEVELOPMENT` and `EXTERNAL_DESCRIPTIVE_RESEARCH`

## Qualification

`INBOX-0002` changes only part of the existing problem space. The first-party Windows stack, PDF viewing path, zero-runtime-state policy, build/install ownership model and process-termination risk remain covered by `RP-0001` unless contradicted by later implementation evidence. Four new knowledge gaps materially gate the expanded DOCX/UI target.

| Topic | Research question | Decision value | Result |
|---|---|---|---|
| `RT-005` | What editable table, font/color/highlight/shading and paragraph subset can be represented safely in WPF and WordprocessingML without silent loss? | revises editable DOCX boundary | [`RT-005-TABLES-FORMATTING.md`](results/RT-005-TABLES-FORMATTING.md) |
| `RT-006` | What first-party Windows/WPF UI organization can provide a Word-like editing experience, including find/replace, without a third-party Ribbon or persistent UI state? | revises shell/UI HOW | [`RT-006-WORDLIKE-UX.md`](results/RT-006-WORDLIKE-UX.md) |
| `RT-007` | How should images, Word shapes, charts and SmartArt/diagrams be handled under a self-written DOCX codec when no external rendering/document engine is allowed? | sets object compatibility and preservation boundary | [`RT-007-GRAPHICS-OBJECTS.md`](results/RT-007-GRAPHICS-OBJECTS.md) |
| `RT-008` | What bounded treatment of footnotes, fields/TOC and similar popular Word constructs is defensible in the first product without creating an implicit Word layout/field engine? | decides which preferences enter v1 | [`RT-008-FOOTNOTES-FIELDS.md`](results/RT-008-FOOTNOTES-FIELDS.md) |

## Non-Research dispositions

- Copy/cut/paste/selection are already platform capabilities and Product commitments; implementation is required rather than further research.
- Find/replace requires local editor logic because WPF `RichTextBox` does not supply a complete built-in search experience; the architectural choice belongs to Target HOW after `RT-006`.
- Whether a preferred feature is admitted to this release is a Product decision, not a Research conclusion.
- Persistent user-customized Ribbon/QAT state would conflict with current zero-runtime-state policy and requires an explicit Product/Decision change. Research does not silently create that state.

## Evidence strategy

Primary Microsoft documentation and OOXML structure documentation are used for platform/format claims. SDK class pages are treated as schema-oriented documentation only; the product will not take a dependency on the Open XML SDK. Existing Word/Office behavior is used as an analogue, not code or a runtime dependency.

## Stopping basis

The delta is sufficient when it establishes: a conservative simple-table editing envelope; distinct text highlight versus shading semantics; a first-party Ribbon path; the absence of a generic first-party Word chart/SmartArt renderer usable by this WPF app; the multi-part nature of charts/diagrams; and the separate-story/field semantics that make full footnote/TOC behavior materially larger than the current product.

Reopen on failed implementation/round-trip evidence, a requirement for editable charts/shapes/TOC, a change to the no-third-party constraint, or evidence that a current Windows API provides a materially simpler compliant renderer/editor path.
