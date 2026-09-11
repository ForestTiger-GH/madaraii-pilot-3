# MADAR Workspace — Pilot 3

**Workspace identity:** `MW-PILOT3-0001`  
**Engineering Subject:** MiniDoc pilot — a small standalone Windows desktop document application  
**Work contour:** `WORK-0001` — post-integration, closure audit pending  
**Governing MADARAII revision:** `7d5ef3d92c4e0982061d422208fdf913c55cc604`  
**Workspace architecture:** [`architecture/WORK-ARCHITECTURE.md`](architecture/WORK-ARCHITECTURE.md)

## Cold-entry route

1. Read [`work/WORK-0001.md`](work/WORK-0001.md) for Commission, boundary, effects, and completion semantics.
2. Read [`work/STATE.md`](work/STATE.md) for the current durable posture.
3. Resolve only the semantic owners required by that posture through the table below.
4. Revalidate Product/evidence identity before any consequential successor Work. Do not infer currentness from branch names or chat history.

## Current owner resolver

| Semantic role | Stable identity / resolver | Current locator |
|---|---|---|
| Work Architecture | `WA-0001` | [`architecture/WORK-ARCHITECTURE.md`](architecture/WORK-ARCHITECTURE.md) |
| Work contour / Commission | `WORK-0001` | [`work/WORK-0001.md`](work/WORK-0001.md) |
| Work State | `WORKSTATE-0001` | [`work/STATE.md`](work/STATE.md) |
| Raw initial human inbox | `INBOX-0001` | [`inbox/INBOX-0001.md`](inbox/INBOX-0001.md) |
| Raw additional human inbox | `INBOX-0002` | [`inbox/INBOX-0002.md`](inbox/INBOX-0002.md) |
| Inbox registry | `INBOX-REGISTRY-0001` | [`inbox/INDEX.md`](inbox/INDEX.md) |
| Developed initial actor input | `INPUT-DEV-0001` | [`intake/INBOX-0001-developed.md`](intake/INBOX-0001-developed.md) |
| Developed additional actor input | `INPUT-DEV-0002` | [`intake/INBOX-0002-developed.md`](intake/INBOX-0002-developed.md) |
| Decisions | `DECISION-OWNER-0001` | [`decisions/DECISIONS.md`](decisions/DECISIONS.md) |
| Questions / residue | `QUESTION-OWNER-0001` | [`questions/QUESTIONS.md`](questions/QUESTIONS.md) |
| Research history | `RP-0001` + delta | [`research/`](research/) |
| Current Scientific Knowledge | `SCIENCE-0001` | [`knowledge/SCIENCE.md`](knowledge/SCIENCE.md) |
| Target WHAT | `TARGET-WHAT-0001` revision 2 | [`knowledge/TARGET-WHAT.md`](knowledge/TARGET-WHAT.md) |
| Target HOW | `TARGET-HOW-0001` revision 2 | [`knowledge/TARGET-HOW.md`](knowledge/TARGET-HOW.md) |
| Product Architecture | `PA-0001` revision 2 | [`../docs/ARCHITECTURE.md`](../docs/ARCHITECTURE.md) |
| Plan | `PLAN-0001` revision 2 | [`work/PLAN-0001.md`](work/PLAN-0001.md) |
| Implementation Result | `IMPLEMENTATION-0001` | [`results/IMPLEMENTATION.md`](results/IMPLEMENTATION.md) |
| Verification Result | `VERIFICATION-0001` | [`results/VERIFICATION.md`](results/VERIFICATION.md) |
| Integration Result | `INTEGRATION-0001` | [`results/INTEGRATION.md`](results/INTEGRATION.md) |
| Current Product | `PRODUCT-0001` | [`product/PRODUCT.md`](product/PRODUCT.md) |
| Evidence | `EVIDENCE-INDEX-0001` | [`evidence/INDEX.md`](evidence/INDEX.md) |
| Closure Result | `CLOSURE-0001` | [`results/CLOSURE.md`](results/CLOSURE.md) when established by MADARAII-34 |

## Exact Product/evidence anchor

The admitted Product realization is the exact verified candidate commit `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`. Windows CI run `34545614992` and `EV-0001` bind the tested configuration. Later `_mw`/README write-back does not redefine Product source identity.

## Local policy

Repository files are UTF-8 text unless the Product format requires otherwise. `_mw` contains engineering-state owners and provenance; it does not mirror the MADARAII catalog. Raw inbox content is immutable. Product source does not take Work-State Authority. Admission, release, deployment and user validation remain distinct.

## Re-evaluation triggers

Reopen affected scope when the human changes requirements, a governing MADARAII change materially invalidates this contour, Product source/build/install/test configuration changes, Windows 11 local qualification fails, or evidence contradicts an admitted Product claim.
