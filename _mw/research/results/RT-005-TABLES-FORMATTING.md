# RT-005 — Editable tables and richer direct formatting

**Question:** what editable table, font/color/highlight/shading and paragraph subset can be represented safely in WPF and WordprocessingML without silent loss?  
**Scope:** first-version DOCX main story; no styles engine, pagination engine or third-party library.  
**Status:** Research Result; input to Scientific Knowledge, not Product commitment.

## Findings

WPF `RichTextBox` edits a `FlowDocument` and can contain `Table`, `TableRowGroup`, `TableRow` and `TableCell`; table cells contain block-level content such as paragraphs. This makes first-party editable tables feasible without adding a UI/document framework.

WordprocessingML represents a table as `w:tbl`, rows as `w:tr`, cells as `w:tc`, with table properties/grid and potentially rich cell/row semantics. The full table vocabulary includes merges, nested tables, style inheritance, widths, borders, conditional styles, positioning and other semantics. A small codec should therefore admit only a recognized simple-table subset rather than treat any `w:tbl` as editable.

A defensible first subset is a rectangular non-nested table with a stable column count, no vertical/horizontal merge, no table/row/cell style references, no floating positioning, and cell contents restricted to the same supported paragraphs/runs/direct formatting as the ordinary main story. Basic cell background can be represented by cell `w:shd` with a clear fill. More complex patterns/theme-derived shading remain outside the editable subset.

WordprocessingML text highlighting uses `w:highlight`; run shading uses `w:shd`. They are distinct semantics and, where both exist, highlighting visually supersedes run shading for the run contents. The product must therefore avoid collapsing both concepts into one persisted property. For v1, text highlight can be supported as `w:highlight`; cell/background fill can use a bounded `w:shd` clear RGB fill. Arbitrary run/paragraph pattern shading can remain unsupported.

Existing direct formatting for font family, font size, RGB text colour, bold/italic/underline and paragraph alignment maps to WPF text/paragraph properties with known loss limits. WPF remains a flow layout rather than a Word pagination model, so the result cannot promise page-layout fidelity.

## Recommended engineering envelope

- Editable: simple rectangular tables; insertion of a new simple table; text editing inside cells; add/delete row and add/delete column if implemented with rectangularity checks.
- Editable formatting: font family, size, text colour, bold, italic, underline, text highlight; paragraph alignment; cell fill.
- Fail closed: merges, nested tables, table styles, conditional formatting, floating tables, complex width/layout semantics, unknown table markup, theme-dependent values that cannot be preserved exactly.
- Saving must serialize only the admitted subset and reopen/reclassify the output before disk mutation.

## Strongest challenge

A Word-created table that looks simple can still depend on styles or grid/width semantics invisible in a naive text-only inspection. Editable admission therefore needs structural checks on `tblPr`, `tblGrid`, row/cell properties and style references; visual simplicity is not evidence of semantic simplicity.

## Sources

- Microsoft WPF Flow Document overview: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/flow-document-overview
- Microsoft WordprocessingML document structure: https://learn.microsoft.com/en-us/office/open-xml/word/structure-of-a-wordprocessingml-document
- Microsoft `Highlight` schema documentation (`w:highlight`): https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.highlight
- Microsoft `Shading` schema documentation (`w:shd`): https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.shading
- ECMA-376 remains the normative format basis already registered by `RP-0001`.

## Assessment

**Confidence:** high for feasibility of the bounded subset; medium for fidelity until fixture/Word round-trip testing occurs on Windows.  
**Does not establish:** compatibility with arbitrary Word tables or page layout fidelity.  
**Reopen:** a supported fixture fails round trip, Word rejects produced table markup, or merged/styled/nested tables become requirements.
