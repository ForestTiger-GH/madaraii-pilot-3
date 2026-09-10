# RT-001 — DOCX structure, editable subset, and round-trip safety

**Question:** What DOCX structure and preservation model permits useful editing without silent destructive round trips?  
**Scope:** `.docx` package structure, main WordprocessingML story, unsupported-feature handling, package preservation, save failure boundary.  
**Non-goals:** full ISO/IEC 29500 implementation, Office fidelity, macro-enabled files, conversion of arbitrary Word documents.

## Findings

DOCX is an Open XML package, not one text file. ECMA-376 defines document vocabularies, representation, packaging, Markup Compatibility/Extensibility, and Transitional features. Microsoft documents the main WordprocessingML structure as `document → body → paragraph → run → text` and also identifies many optional parts/stories such as headers, footers, comments, footnotes, endnotes, settings, and styles. A typical file can therefore carry materially meaningful content well beyond `word/document.xml`.

A small editor has two different preservation problems:

1. **Package preservation:** keep package parts and relationships the product does not own.
2. **Main-story semantic preservation:** avoid rebuilding an unsupported main story into a lossy approximation.

The first problem is tractable with the .NET base class library: `System.IO.Compression.ZipArchive` can update an archive in memory. The second requires an explicit compatibility scanner and a fail-closed editing policy. A text-extraction fallback can support reading an unsupported document, but that fallback cannot safely become the source for a saved DOCX.

## Viable v0.1 compatibility model

An editable document is deliberately narrow:

- WordprocessingML Transitional namespace for the main story;
- body made of paragraphs plus an optional final section-properties element;
- paragraphs made of ordinary runs;
- run content limited to text, tab, and ordinary line breaks;
- direct run formatting limited to bold, italic, single underline, explicit font family, font size, and RGB text color;
- paragraph alignment limited to left, center, right, and justify;
- opaque optional section properties may be retained unchanged because v0.1 does not edit them.

Main-story structures outside that model — including tables, drawings/images, hyperlinks, fields, lists/numbering, styles referenced from paragraphs/runs, tracked changes, content controls, bookmarks/comments anchors, equations, embedded objects, page/column breaks, and other unknown elements that affect interpretation — trigger **read-only compatibility mode**. The file may be viewed as approximate extracted text, while Save and Save As as an edited DOCX stay disabled. This avoids silent conversion.

Strict OOXML is also read-only in v0.1. Signed packages are read-only because modifying a signed package would invalidate its signature. Encrypted/non-ZIP Office documents are rejected with an explicit unsupported message. `.docm` is outside the input surface.

## Preservation model for editable files

For an accepted editable file:

- retain the original package bytes in memory during the session;
- resolve the main document part through package relationships rather than hard-coded filename discovery;
- parse XML with DTD processing prohibited and external resolution disabled;
- retain all non-main package entries unchanged at the semantic part level;
- rebuild only the supported main document body from the editor model, carrying the original root/body metadata and final section properties where supported;
- preserve relationship parts and all unowned parts; never follow or execute external relationships;
- reopen the generated in-memory package through the product parser before disk write.

The ZIP container itself is not promised to remain byte-identical: entry ordering/compression metadata can change. The protected claim is that unowned package part payloads remain untouched and unsupported main-story semantics are never saved through an editable route.

## Size/security boundary

A self-written parser needs resource limits. v0.1 should reject inputs above conservative limits (for example 64 MiB package bytes, 4096 entries, 256 MiB aggregate uncompressed size, and 16 MiB main-document XML). These values are product limits, not format limits. Their purpose is bounded memory/CPU exposure and decompression-bomb resistance.

## Save durability trade-off

The cleanliness constraint disfavors hidden or arbitrary temporary files. An in-memory package build followed by one explicit target-file write creates no application temp artifact. However, replacing an existing file by truncating and writing is not power-failure atomic. The product can keep the prior target bytes in memory and attempt restoration after an ordinary I/O exception, but a process/OS/power failure during final write can still leave a partial target. This limitation must be a visible compatibility/durability statement; Save As to a new path preserves the source file.

## Assessment

A useful subset editor is feasible without the Open XML SDK or another third-party document library. Safety comes from **conservative admission to editable mode**, not from pretending to preserve arbitrary WordprocessingML. This boundary is acceptable for a small v0.1 if the UI tells the user why a document is read-only and documentation lists the exact subset.

## Sources

- ECMA-376, Office Open XML file formats, 5th edition overview and Parts 1–4: https://ecma-international.org/publications-and-standards/standards/ecma-376/
- Microsoft Learn, “Structure of a WordprocessingML document”: https://learn.microsoft.com/en-us/office/open-xml/word/structure-of-a-wordprocessingml-document
- Microsoft Learn, “How to: Create a package”: https://learn.microsoft.com/en-us/office/open-xml/general/how-to-create-a-package
- Microsoft Learn, `ZipArchiveMode`: https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchivemode?view=net-10.0
- Microsoft Learn, `LoadOptions`: https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.loadoptions?view=net-10.0
- Microsoft Learn, `File.WriteAllBytes`: https://learn.microsoft.com/en-us/dotnet/api/system.io.file.writeallbytes?view=net-10.0

**Evidence limit:** no claim here states Microsoft Word-equivalent rendering or arbitrary DOCX round-trip compatibility. Those are outside the studied/selected boundary.
