# RT-007 — Images, shapes, charts and diagrams

**Question:** how should images, Word shapes, charts and SmartArt/diagrams be handled under a self-written DOCX codec when no external rendering/document engine is allowed?  
**Scope:** first-version WordprocessingML viewing/editability boundary; existing objects only unless a creation path is separately admitted.  
**Status:** Research Result.

## Findings

WordprocessingML graphics are package-graph objects, not one generic image node. A simple raster picture normally references an image part through DrawingML relationship markup. That case can be decoded by the product from the package and displayed in WPF without a third-party engine, provided the exact recognized shape/relationship subset is constrained.

Modern Word drawing extensions also use OOXML extensibility/`mc:AlternateContent`. WordprocessingML drawing objects can include shapes, groups and canvases, with compatibility fallback markup. A general Word-shape renderer would require implementing a meaningful part of DrawingML/Office extension geometry, transforms, fills, strokes, text and fallback semantics. WPF shape controls do not by themselves establish DOCX rendering fidelity.

Charts are substantially richer. Microsoft documentation describes charts as multiple related package parts, including chart markup, a chart-specific relationship part and a required embedded Excel workbook for native editable charts. SmartArt similarly uses several related parts for layout/data and optional drawing/style/color information. A generic “show it as an image” strategy is therefore available only when the package actually carries a usable image/fallback representation; the format does not guarantee a universal raster preview for every chart or SmartArt object.

Because the product must avoid silent loss, treating a chart/SmartArt/shape as a normal editable WPF image and serializing only that image would destroy underlying editable semantics. Native chart/SmartArt/shape creation/editing would expand the product into a separate graphics/layout engine and is disproportionate to the first release.

## Recommended engineering envelope

- Support a narrowly recognized embedded raster-picture subset for display and safe preservation only when relationship/type/size checks pass. Picture editing beyond deletion/replacement is outside the first release unless later admitted.
- For charts, SmartArt, Word shapes, grouped drawings, canvases and unrecognized drawing markup: open the DOCX in explicit read-only compatibility mode for v1.
- In compatibility view, extract and show an associated image preview only when an actual package image/fallback relationship can be resolved safely. Otherwise show an object placeholder and explanatory compatibility reason. Never synthesize a fake raster preview and call it faithful.
- Preserve the original file untouched in compatibility mode.
- Defer shape creation and native chart/diagram editing. This honors the preference as a future candidate without converting it into an unreliable current commitment.

## Strongest challenge

A package may contain both a drawing object and image-like parts that are thumbnails, effects layers or unrelated media. Relationship lineage and content type must establish that a displayed image is actually associated with the object. Filename proximity is insufficient.

## Sources

- Microsoft Office OOXML rich-content insertion guidance: https://learn.microsoft.com/en-us/office/dev/add-ins/word/create-better-add-ins-for-word-with-office-open-xml
- Microsoft WordprocessingML Drawing extension specification: https://learn.microsoft.com/en-us/openspecs/office_standards/ms-odrawxml/6aaf2ba5-6974-4560-9983-3fcd7fead7cb
- Microsoft WordprocessingML document structure: https://learn.microsoft.com/en-us/office/open-xml/word/structure-of-a-wordprocessingml-document
- ECMA-376 remains the normative package/markup basis registered by `RP-0001`.

## Assessment

**Confidence:** high that generic charts/SmartArt/shapes cannot be safely reduced to images without object-specific evidence; high that read-only compatibility is safe.  
**Does not establish:** a complete object-to-preview resolver for arbitrary Word files.  
**Reopen:** editable shapes/charts become requirements, a supported preview fixture fails, or a first-party Windows API is found that renders arbitrary Word drawing/chart semantics without an external document engine.
