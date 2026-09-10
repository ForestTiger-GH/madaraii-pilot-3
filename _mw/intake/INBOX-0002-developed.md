# INPUT-DEV-0002 — Developed additional human inbox

**Source:** `INBOX-0002`  
**Source Baseline:** immutable raw carrier at `_mw/inbox/INBOX-0002.md`  
**Receiving Work:** `WORK-0001`  
**Impact Baseline:** `TARGET-WHAT-0001` / `TARGET-HOW-0001` / `PLAN-0001` and the partial non-admitted implementation candidate present when this carrier arrived.

The carrier materially expands the DOCX/UI feature boundary. Modal words are preserved: explicit “должен/должны” statements are requirements; “желательно/хочется” statements remain preferences or candidate commitments pending research and authorized target assembly.

## Developed semantic acts

| Act | Type | Normalized meaning / boundary | Owner / disposition |
|---|---|---|---|
| B01 | Product requirement | The UI shall be recognizably similar in organization and interaction style to Microsoft Word or comparable desktop word processors, while remaining a distinct product rather than a visual clone. | Reopen Target WHAT/HOW UI slice. |
| B02 | Product requirement | Editable text work shall include copy, paste, selection, find and replace as normal first-class editing commands. | Reopen Target WHAT/HOW; implementation required. |
| B03 | Product requirement | Basic editing shall include font selection, text color, fill/highlight semantics, paragraph controls, and editable tables. | Reopen DOCX/UI Target WHAT/HOW; current table-as-read-only rule becomes stale. |
| B04 | Product preference | Geometry/shape creation should preferably be included in this contour if a bounded implementation is practical under the no-third-party constraint. | Research/design candidate; not yet a commitment. |
| B05 | Product requirement with bounded implementation freedom | Charts/diagrams in opened DOCX must receive an explicit supported behavior. Rendering an available chart/diagram representation as an image is acceptable for the first version if this is materially simpler; native chart editing is optional unless later admitted. | Research → Target WHAT/HOW. |
| B06 | Product preference | Footnotes, table of contents, and other popular Word functions are desired where a small coherent first-version boundary can support them honestly. | Research/value assessment; candidate commitments only. |
| B07 | Product preference | UI configurability should preferably follow familiar Word-like organization where practical. | Target design candidate; bound by small-product and zero-state constraints. |
| B08 | Constraint continuity | Original constraints remain in force unless explicitly contradicted: small standalone product, no third-party libraries/engines, controlled filesystem lifecycle, full process exit, honest DOCX compatibility, PDF view-only, reproducible Windows build/install/uninstall. | Existing Target/Decision owners remain authoritative. |
| B09 | Scope-change event | `TARGET-WHAT-0001`, `TARGET-HOW-0001`, `PA-0001`, and `PLAN-0001` are stale for the affected DOCX/UI slices until reconciled; implementation may continue only on unaffected mechanics after explicit revalidation. | Work State reconciliation. |

## Conflicts with prior target

- `TARGET-WHAT-0001` currently classifies tables as a compatibility-mode trigger; B03 requires editable tables. That commitment must be replaced for the current target revision.
- Prior UI language favored a compact menu/toolbar and explicitly rejected a large Ribbon. B01 does not require a full Office Ribbon, but it does require Word-like information architecture and familiar grouped editing controls. The target must reconcile “small/minimal” with “Word-like organization” rather than selecting either extreme silently.
- Current zero-persistent-state Decision `D-0004` limits persistent UI customization. B07 therefore cannot become “remember arbitrary custom ribbon state” without an explicit new Decision changing the persistence model. Session-only configurability or a fixed Word-like grouped UI remains compatible.

## Research/decision needs created

1. Determine a bounded editable-table subset and safe DOCX round-trip model under direct OOXML processing.
2. Determine how to represent text highlight versus paragraph/cell shading and which subset can be mapped to WPF without semantic collapse.
3. Determine WordprocessingML handling for drawing/picture/chart objects, including whether preview/fallback images can be displayed without silently treating editable charts as ordinary images on save.
4. Determine the minimal viable treatment of shapes, footnotes/endnotes, TOC/fields and other popular constructs under the no-third-party and no-silent-loss constraints.
5. Determine a Word-like but bounded desktop information architecture that does not require copying Microsoft visuals or building an oversized Ribbon framework.
6. Reconcile find/replace and table editing with the existing WPF editor model and serialization boundaries.

## Authority and status

The human input has Product Authority for B01–B03 and B05 as requirements and for B04/B06/B07 as preferences. It does not itself choose the OOXML mechanism, exact table/shape/chart subset, UI control technology, or persistence change. Those remain Research/Target HOW/Decision work.

**Immediate safe disposition:** pause affected implementation, qualify and perform the new research, revise Science where material, then issue a new Target WHAT/HOW/Architecture/Plan baseline before implementing the expanded feature slice.
