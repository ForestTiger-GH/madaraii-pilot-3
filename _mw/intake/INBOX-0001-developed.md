# INPUT-DEV-0001 — Developed initial human inbox

**Source:** `INBOX-0001`  
**Source Baseline:** immutable raw carrier preserved at `_mw/inbox/INBOX-0001.md`  
**Receiving Work:** `WORK-0001`

The source carrier is a rich product request. Development below preserves modality: explicit requirements remain requirements; examples and preferences stay bounded as such; research questions remain research concerns until evidence resolves them.

## Developed semantic acts

| Act | Type | Normalized meaning / boundary | Owner / disposition |
|---|---|---|---|
| A01 | Commission / Work request | Execute a complete development contour, not a code-only prototype. | `WORK-0001`; admitted by enclosing instruction. |
| A02 | Product outcome requirement | Deliver a small standalone Windows desktop document application with normal launch, install, uninstall, and a desktop shortcut. | Target WHAT input. |
| A03 | Product constraint | No third-party libraries, frameworks, ready document/PDF engines, or external product components. Windows capabilities, standard language/runtime facilities, and necessary official build tooling require explicit boundary analysis. | Research + Target WHAT/HOW. |
| A04 | Product quality requirement | Product runtime must intentionally avoid AppData, Temp, hidden caches, random service folders, background helpers, services, tray processes, and updater processes. Closing the app ends its execution. | Target WHAT; Research/verification. |
| A05 | Lifecycle requirement | Technical state, if materially required, must live in an explicit controlled location with documented creation/update/uninstall lifecycle. Uninstall removes application-owned technical artifacts while preserving user documents. | Target WHAT/HOW; verification. |
| A06 | DOCX requirement | Open and edit a practically useful basic DOCX subset, support Save and Save As, and define the minimum editor commands from research. | Research → Target WHAT/HOW. |
| A07 | DOCX safety invariant | Unsupported DOCX constructs must not be silently destroyed or distorted by saving. Round-trip limitations require explicit detection and honest behavior. | Target WHAT invariant; Research/HOW/verification. |
| A08 | PDF requirement | Open PDF in the same application for viewing only; define the minimal viewer functions from research. | Research → Target WHAT/HOW. |
| A09 | UX requirement | Minimal, understandable, native-feeling Windows UI; no large Ribbon or demo-form feel. | Target WHAT/HOW. |
| A10 | Research requirement | Study DOCX structure, safe save semantics, PDF rendering complexity, Windows capabilities, dependency boundary, installation lifecycle, system-cleanliness risks, and relevant analogues before fixing technical design. | Qualified Research program. |
| A11 | Build requirement | Repository contains a clear reproducible local Windows build using official tooling and a simple command/script that yields installable output. | Target WHAT/HOW/Product. |
| A12 | Installation requirement | Installation placement, shortcut, registrations if needed, update posture, uninstall cleanup, and residue verification are first-class product behavior. | Research → Target WHAT/HOW/Product/verification. |
| A13 | Architecture constraint | Keep v1 small but separate independently evolving responsibilities; avoid both a monolith and speculative enterprise layers. | Target HOW; separate Product Architecture only if independently useful. |
| A14 | Verification requirement | Actually verify claimed properties; where agent environment cannot execute Windows behavior, provide reproducible evidence/procedure and distinguish proven from locally pending. | Verification/Evidence/closure. |
| A15 | Scope exclusion | PDF editing and full Office/Acrobat parity are outside this contour. Future formats/features are possibilities, not current commitments. | Target WHAT non-goals. |
| A16 | Documentation requirement | Final documentation states promises, rationale for compatibility boundary, architecture, build/install/use/uninstall/verification, proven facts, unsupported behavior, and significant limitations. | Product documentation + closure. |

## Authority and epistemic limits

The human source has Product/Commission Authority for requirements and constraints. It does not itself choose a programming language, document subset details, rendering mechanism, installer technology, or architecture. Those are downstream design Decisions after Research.

Examples such as a possible `Documents` state folder are preferences/permissions, not requirements to create such state. Statements about what “should be researched” create Research obligations, not predetermined conclusions.

## Immediate dispositions

A10/A03/A06/A07/A08/A12/A14 require external descriptive Research before design commitment. A02–A09 and A11–A16 become the authoritative commitment universe for Target WHAT assembly after Scientific Knowledge is adequate. No additional target feature is implied by the carrier.
