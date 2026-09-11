# MADAR Workspace — Pilot 3

**Workspace identity:** `MW-PILOT3-0001`  
**Engineering Subject:** MiniDoc pilot — a small standalone Windows desktop document application  
**Work contour:** `WORK-0001` — **closed**  
**Closure:** `CLOSURE-0001` = `PASS`  
**Governing MADARAII revision:** `7d5ef3d92c4e0982061d422208fdf913c55cc604`  
**Workspace architecture:** [`architecture/WORK-ARCHITECTURE.md`](architecture/WORK-ARCHITECTURE.md)

## Cold-entry route

1. Read [`work/STATE.md`](work/STATE.md) first. It owns the terminal/current Work posture.
2. Read [`product/PRODUCT.md`](product/PRODUCT.md) for the current admitted Product Baseline and reliance envelope.
3. Read [`work/WORK-0001.md`](work/WORK-0001.md) and [`results/CLOSURE.md`](results/CLOSURE.md) only when the closed Commission/closure argument is relevant.
4. For new work, resolve only Target/Architecture/Decision/Knowledge owners materially required by the new Commission. Do not revive `WORK-0001` merely because improvement opportunities exist.
5. Revalidate exact Product/evidence identity before any consequential mutation. Do not infer currentness from branch names or chat history.

## Current owner resolver

| Semantic role | Stable identity / resolver | Current locator |
|---|---|---|
| Work Architecture | `WA-0001` | [`architecture/WORK-ARCHITECTURE.md`](architecture/WORK-ARCHITECTURE.md) |
| Closed Work contour / Commission | `WORK-0001` | [`work/WORK-0001.md`](work/WORK-0001.md) |
| Work State | `WORKSTATE-0001` | [`work/STATE.md`](work/STATE.md) |
| Raw initial human inbox | `INBOX-0001` | [`inbox/INBOX-0001.md`](inbox/INBOX-0001.md) |
| Raw additional human inbox | `INBOX-0002` | [`inbox/INBOX-0002.md`](inbox/INBOX-0002.md) |
| Inbox registry | `INBOX-REGISTRY-0001` | [`inbox/INDEX.md`](inbox/INDEX.md) |
| Developed initial actor input | `INPUT-DEV-0001` | [`intake/INBOX-0001-developed.md`](intake/INBOX-0001-developed.md) |
| Developed additional actor input | `INPUT-DEV-0002` | [`intake/INBOX-0002-developed.md`](intake/INBOX-0002-developed.md) |
| Decisions | `DECISION-OWNER-0001` | [`decisions/DECISIONS.md`](decisions/DECISIONS.md) |
| Questions / terminal residue | `QUESTION-OWNER-0001` | [`questions/QUESTIONS.md`](questions/QUESTIONS.md) |
| Research history | `RP-0001` + delta `RP-0002` | [`research/`](research/) |
| Current Scientific Knowledge | `SCIENCE-0001` | [`knowledge/SCIENCE.md`](knowledge/SCIENCE.md) |
| Target WHAT | `TARGET-WHAT-0001` revision 2 | [`knowledge/TARGET-WHAT.md`](knowledge/TARGET-WHAT.md) |
| Target HOW | `TARGET-HOW-0001` revision 2 | [`knowledge/TARGET-HOW.md`](knowledge/TARGET-HOW.md) |
| Product Architecture | `PA-0001` revision 2 | [`../docs/ARCHITECTURE.md`](../docs/ARCHITECTURE.md) |
| Terminal Plan | `PLAN-0001` revision 2 | [`work/PLAN-0001.md`](work/PLAN-0001.md) |
| Implementation Result | `IMPLEMENTATION-0001` | [`results/IMPLEMENTATION.md`](results/IMPLEMENTATION.md) |
| Verification Result | `VERIFICATION-0001` | [`results/VERIFICATION.md`](results/VERIFICATION.md) |
| Integration Result | `INTEGRATION-0001` | [`results/INTEGRATION.md`](results/INTEGRATION.md) |
| Current Product | `PRODUCT-0001` | [`product/PRODUCT.md`](product/PRODUCT.md) |
| Evidence | `EVIDENCE-INDEX-0001` | [`evidence/INDEX.md`](evidence/INDEX.md) |
| First closure audit history | `CLOSURE-AUDIT-0001` attempt 1 | [`results/CLOSURE-AUDIT-0001-ATTEMPT-1.md`](results/CLOSURE-AUDIT-0001-ATTEMPT-1.md) |
| Closure Result | `CLOSURE-0001` | [`results/CLOSURE.md`](results/CLOSURE.md) |

## Exact Product/evidence anchor

The current admitted Product realization is exact verified candidate commit `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`. Windows CI run `34545614992` and `EV-0001` bind the tested configuration. Later README/`_mw` write-back and closure-state commits do not redefine Product source identity.

Verified install-package artifact: `MiniDoc-0.1.0-win-x64`, artifact `10178912526`, GitHub-reported ZIP digest `sha256:54c235f7e1b8cab23ab615e930128ed6c18873be7076ec8427e03aad83f86f73`. Artifact retention is temporary; repository source/build mechanisms are durable.

## Local policy

Repository files are UTF-8 text unless the Product format requires otherwise. `_mw` contains engineering-state owners and provenance; it does not mirror the MADARAII catalog. Raw inbox content is immutable. Product source does not take Work-State Authority. Admission, release, deployment and user validation remain distinct.

## Terminal posture and reopen triggers

No Work is active after `CLOSURE-0001`. Do not manufacture successor scope.

Open new Work only on a new human Commission or a material trigger such as Product/source change, Windows 11 local qualification failure that matters to reliance, new commissioned format breadth, governing change that invalidates this state, or evidence contradicting an admitted claim.
