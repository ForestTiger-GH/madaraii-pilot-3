# Research Program — RP-0001

**Status:** completed for Target formation  
**Consumer:** `WORK-0001`, Scientific Knowledge assembly, Target WHAT/HOW  
**Baseline/cutoff:** public standards and platform state checked 2026-09-11  
**Governing work kinds:** `RESEARCH_TOPIC_DEVELOPMENT` and `EXTERNAL_DESCRIPTIVE_RESEARCH`

## Signal disposition

The research obligations developed from `INPUT-DEV-0001` resolve into four bounded Topics. Current Product reconstruction is inapplicable because the Product Baseline is absent. Language/platform/installer selection is a later design Decision, informed by Research rather than answered by the Topic itself.

| Topic | Research question | Decision value | Result |
|---|---|---|---|
| `RT-001` | What DOCX structure and preservation model permits useful editing without silent destructive round trips? | sets compatibility and save boundary | [`RT-001-DOCX.md`](results/RT-001-DOCX.md) |
| `RT-002` | Which Windows-provided UI and PDF capabilities can satisfy the product without third-party runtime components, and what lifecycle risks exist? | bounds platform/PDF HOW | [`RT-002-WINDOWS-PDF.md`](results/RT-002-WINDOWS-PDF.md) |
| `RT-003` | What build/install/uninstall model gives normal Windows installation while keeping owned artifacts explicit and removable? | bounds deployment HOW | [`RT-003-LIFECYCLE.md`](results/RT-003-LIFECYCLE.md) |
| `RT-004` | What minimum editor/viewer UX and verification strategy makes v1 practically useful and its claims testable? | bounds v1 feature set and evidence | [`RT-004-MVP-ASSURANCE.md`](results/RT-004-MVP-ASSURANCE.md) |

## Evidence strategy

Primary sources dominate normative and platform claims: ECMA-376, Microsoft Windows/.NET/WPF documentation. A Microsoft-hosted GitHub issue is used as negative practitioner evidence for one PDF lifecycle failure mode; it narrows confidence rather than establishing universal current behavior. Analogues inform boundaries only and supply no product code.

## Stopping basis

The corpus reached sufficient saturation for v0.1 when it could: define a fail-closed DOCX subset; identify a first-party PDF path; separate runtime from build dependencies; define exact installation-owned state; expose the PDF process-lifecycle risk; choose reproducible verification obligations; and leave no material target choice dependent on unknown third-party behavior.

Reopen Research on a Windows/.NET platform change, a requested expansion of the DOCX/PDF subset, a failed native PDF lifecycle test, or materially new evidence about the selected first-party APIs.
