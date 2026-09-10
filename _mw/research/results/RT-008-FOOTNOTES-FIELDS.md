# RT-008 — Footnotes, fields and table of contents

**Question:** what bounded treatment of footnotes, fields/TOC and similar popular Word constructs is defensible in the first product without creating an implicit Word layout/field engine?  
**Scope:** first release under the small-product and no-third-party constraints.  
**Status:** Research Result.

## Findings

Footnotes are not simply inline superscript text. The main story carries `w:footnoteReference` elements that identify footnotes, while footnote contents live in a separate Footnotes story/part. Footnote contents may themselves contain block-level WordprocessingML. Correct Word-like bottom-of-page placement also depends on pagination/layout that WPF `RichTextBox` does not reproduce.

Complex Word fields use begin/separate/end field characters plus `w:instrText` field instructions and a stored field result. Field semantics can nest and the displayed result is distinct from the instruction. A table of contents is a field-driven feature in Word: its useful behavior depends on heading/style semantics, field instructions, ordering and update/recalculation. A local editor can display stored result text, but that does not amount to a correct TOC engine.

The original product explicitly avoided a styles engine and Word pagination fidelity. Adding editable footnotes plus live TOC regeneration in the same first release would therefore introduce several new semantic owners: styles/heading resolution, field parsing/evaluation, reference numbering, separate-story editing and pagination-aware presentation. This expansion is materially larger than the new hard requirements in `INBOX-0002`.

## Recommended engineering envelope

- Footnotes: keep documents containing footnote references in explicit read-only compatibility mode for the first release. Extract footnote body text into the compatibility view when the reference/part can be resolved safely; preserve the source unchanged.
- TOC/fields: keep documents with complex fields, simple fields, TOC or other field instructions in read-only compatibility mode. Display stored result text where safely extractable and label it as cached document content, not recalculated output.
- Footnote creation/editing and TOC insertion/update are deferred preference candidates, not current first-release commitments.
- Ordinary headings may later become a separate supported style feature; they should not be introduced solely to fake TOC support.

## Strongest challenge

A read-only compatibility route is less feature-rich than the user preference. It is nevertheless semantically safer than allowing text edits around references/fields when the serializer cannot guarantee reference numbering, field boundaries and regenerated results.

## Sources

- Microsoft WordprocessingML document structure: https://learn.microsoft.com/en-us/office/open-xml/word/structure-of-a-wordprocessingml-document
- Microsoft `FootnoteReference` schema documentation: https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.footnotereference
- Microsoft `Footnote` schema documentation: https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.footnote
- Microsoft `FieldChar` schema documentation: https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.fieldchar
- Microsoft `FieldCode` (`w:instrText`) schema documentation: https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.wordprocessing.fieldcode
- ECMA-376 remains the normative basis.

## Assessment

**Confidence:** high that full footnote/TOC editing is a materially separate capability.  
**Does not establish:** that every cached field result or footnote can be rendered faithfully in the compatibility view.  
**Reopen:** footnote editing or TOC generation becomes an explicit requirement, a separate styles/fields contour is commissioned, or a first-party renderer/editor mechanism changes the cost boundary materially.
