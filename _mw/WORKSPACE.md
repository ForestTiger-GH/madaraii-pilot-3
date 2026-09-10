# MADAR Workspace — Pilot 3

**Workspace identity:** `MW-PILOT3-0001`  
**Engineering Subject:** MiniDoc pilot — a small standalone Windows desktop document application  
**Active Work:** `WORK-0001`  
**Governing MADARAII revision:** `7d5ef3d92c4e0982061d422208fdf913c55cc604`  
**Workspace architecture:** [`architecture/WORK-ARCHITECTURE.md`](architecture/WORK-ARCHITECTURE.md)

## Cold-entry route

1. Read [`work/WORK-0001.md`](work/WORK-0001.md) for Commission, boundary, effects, and completion.
2. Read [`work/STATE.md`](work/STATE.md) for the current durable posture and next safe action.
3. Resolve only the semantic owners required by that next Work through the table below.
4. Apply the exact MADARAII revision and work kind bound in `STATE.md`. Re-resolve current owner state before consequential writes.

## Current owner resolver

| Semantic role | Stable identity / resolver | Current locator |
|---|---|---|
| Work Architecture | `WA-0001` | [`architecture/WORK-ARCHITECTURE.md`](architecture/WORK-ARCHITECTURE.md) |
| Work contour / Commission | `WORK-0001` | [`work/WORK-0001.md`](work/WORK-0001.md) |
| Work State | `WORKSTATE-0001` | [`work/STATE.md`](work/STATE.md) |
| Raw human inbox | `INBOX-0001` | [`inbox/INBOX-0001.md`](inbox/INBOX-0001.md) |
| Inbox registry | `INBOX-REGISTRY-0001` | [`inbox/INDEX.md`](inbox/INDEX.md) |
| Developed actor input | `INPUT-DEV-0001` | [`intake/INBOX-0001-developed.md`](intake/INBOX-0001-developed.md) |
| Decisions | `DECISION-OWNER-0001` | [`decisions/DECISIONS.md`](decisions/DECISIONS.md) |
| Questions | `QUESTION-OWNER-0001` | [`questions/QUESTIONS.md`](questions/QUESTIONS.md) |
| Research history | owner route | `_mw/research/` when established |
| Current Scientific Knowledge | `SCIENCE-0001` | `_mw/knowledge/SCIENCE.md` when admitted |
| Target WHAT | `TARGET-WHAT-0001` | `_mw/knowledge/TARGET-WHAT.md` when admitted |
| Target HOW | `TARGET-HOW-0001` | `_mw/knowledge/TARGET-HOW.md` when admitted |
| Product | `PRODUCT-0001` | `_mw/product/PRODUCT.md` when admitted |
| Evidence | `EVIDENCE-INDEX-0001` | `_mw/evidence/INDEX.md` when established |
| Closure Result | `CLOSURE-0001` | `_mw/results/CLOSURE.md` when established |

An absent “when established” locator is an explicit bootstrap absence, not a missing-file error.

## Local policy

Repository files are UTF-8 text unless the Product format requires otherwise. Product source and documentation may use their native conventional names. `_mw` contains engineering-state owners and provenance; it does not mirror the MADARAII catalog. Raw inbox content is immutable. Product source does not take Work-State Authority.

## Re-evaluation triggers

Reconcile Work State before continuing when the human changes scope or constraints, the governing MADARAII revision materially changes, mandatory evidence contradicts a target commitment, a Windows-local verification materially fails, or the authoritative Product candidate changes after verification.
