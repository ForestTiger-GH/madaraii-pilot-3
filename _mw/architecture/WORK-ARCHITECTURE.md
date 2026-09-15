# Work Architecture — WA-0001 revision 2

**Status:** accepted for the new post-closure development epoch  
**Engineering Subject:** MiniDoc — the standalone Windows desktop document application and the engineering corpus that defines, realizes, verifies, and closes it  
**Architecture Baseline:** legacy closed contour `WORK-0001` at repository commit `397168104312d97e97d030627f483c882403baa8`  
**Current Product Baseline:** verified Product candidate `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681` with admitted closure `CLOSURE-0001 = PASS`  
**Governing basis:** `ForestTiger-GH/MADARAII@721a199352b5c5282b1478cd6a5eb36a9872fb34` (`dev`)

## Current organizational question

The original Product-development contour is closed. A new human Commission now opens a distinct post-closure epoch to:

1. perform one bounded Jester challenge;
2. process its Report through non-Jester Work and integrate only justified Product/Knowledge/Work changes;
3. perform a second independent Jester challenge against the resulting current state;
4. process that second Report;
5. form missing semantic Work summaries and an Epoch Development Report;
6. audit the final development contour and prepare the workspace for archival without silently reopening the old closed Commission.

This is new Work. `WORK-0001` remains closed history and is never revived merely because the new epoch finds improvement opportunities.

## Architecture drivers

- Current MADARAII requires an explicit `_mw/AGENTS.md` workspace passport and one active Development Epoch.
- Jester Work is optional specialist Work, solo per live run, and cannot interpret or commission its own successor.
- Each Jester Report must enter separately authorized sequential non-Jester continuation, normally actor-input development and Work-state reconciliation before specialized follow-up.
- Logical context reset and rehydration from current owners is required before each distinct substantive Work execution.
- Reusable current Product, Knowledge, Decisions, Evidence, and prior closed-work records remain outside the new epoch unless a new accepted transition changes their actual owner.
- New epoch-specific inbox, Jester Reports, Work state, follow-up Results, summaries, Development Report, closure audit, and finalization evidence are epoch-bound.

## Semantic owners and current routes

| Role | Current owner / route |
|---|---|
| Workspace passport / cold-entry projection | `_mw/AGENTS.md` |
| Work Architecture | `_mw/architecture/WORK-ARCHITECTURE.md` |
| Legacy closed contour | `_mw/work/WORK-0001.md`, `_mw/work/STATE.md`, `_mw/results/CLOSURE.md` |
| Current Product | `_mw/product/PRODUCT.md` plus repository Product surfaces |
| Current Scientific Knowledge | `_mw/knowledge/SCIENCE.md` |
| Current Target WHAT | `_mw/knowledge/TARGET-WHAT.md` |
| Current Target HOW | `_mw/knowledge/TARGET-HOW.md` |
| Current Decisions | `_mw/decisions/DECISIONS.md` |
| Current Questions | `_mw/questions/QUESTIONS.md` |
| Shared Evidence | `_mw/evidence/` |
| Active epoch Work State | `_mw/epochs/001-jester-finalization/work/STATE.md` |
| Active epoch human carrier | `_mw/epochs/001-jester-finalization/inbox/INBOX-0001.md` |
| Active epoch Results | `_mw/epochs/001-jester-finalization/results/` as established |
| Active epoch summaries/reports | `_mw/epochs/001-jester-finalization/` only when the corresponding Work establishes them |

A locator does not acquire Authority from this table. Each dependent Work re-resolves current identity, Baseline, scope, integrity, and permission to rely.

## Epoch contract

Exactly one epoch is active:

`EPOCH-001 — post-closure Jester challenge, reconciliation, reporting, and archival preparation`.

Legacy `WORK-0001` is a pre-epoch closed historical contour. It remains resolvable in place rather than being moved merely for aesthetic conformity. The active epoch may reference it as Baseline/provenance but may not redefine its terminal state.

The epoch is complete only after both commissioned Jester passes have been processed by non-Jester continuation, all justified changes are reconciled, summaries and Development Report are formed, a closure audit passes, and finalization establishes archive-readiness.

## Work geometry

```text
workspace reconfiguration / new Commission intake
→ Jester pass 1
→ actor-input development of JR-01
→ Work-state reconciliation / exact specialized follow-up
→ authorized Product/Knowledge changes, verification and integration when justified
→ Jester pass 2 on the resulting current state
→ actor-input development of JR-02
→ Work-state reconciliation / exact specialized follow-up
→ authorized final changes when justified
→ semantic Work summaries
→ Epoch Development Report
→ development-contour closure audit
→ epoch finalization / archive-readiness
```

Arrows express commissioned dependencies in this epoch, not a universal MADARAII pipeline. A Jester observation may terminate in no-action after reconciliation; it does not become a defect, Topic, Task, Decision, or Product change by surprise alone.

## Authority and effect boundaries

The current human Commission authorizes repository-local organizational adjustment needed to use current MADARAII; two sequential Jester passes; non-Jester processing of their Reports; justified repository-local Product, Knowledge, Decision, Evidence, and Work-state updates; verification; summaries; Development Report; closure audit; and archive-readiness preparation.

No live external-system, protected-data, irreversible physical, deployment, release, or user-environment effect is authorized. Jester intervention should therefore prefer Observation or isolated derived/sacrificial surfaces. Direct mutation of authoritative Product sectors is not authorized unless a later exact Work contract separately proves the complete backup-and-restore shell required by MADARAII-40.

## Context-reset and rehydration contract

Before every distinct substantive Work:

1. treat the prior actor reasoning context as non-authoritative;
2. reopen `_mw/AGENTS.md` explicitly;
3. reread current `FOUNDATION`, the exact selected MADARAII, and its matching EXAMPLE from `MADARAII/dev`;
4. re-resolve the Work Commission, Subject, Baseline, current owners, Authority, required read closure, and stop conditions;
5. widen context only for a material dependency, contradiction, UNKNOWN, coverage obligation, applicability condition, or consequence.

A physical hard reset is not claimed unless an exact Work contract requires and the runtime can demonstrate it.

## Interaction, recovery, and archive posture

Human messages may be received and represented as immutable carriers. This epoch does not rely on blocking human interaction for progress. Active Work is recoverable from the passport and epoch Work State.

Archive-readiness means the active epoch has a truthful terminal state, no orphan Jester handoff, all material residue has an owner or terminal disposition, summaries/report are established where commissioned, and current Product/Knowledge owners no longer depend on transient epoch state for their meaning. Physical archival movement or deletion is a separate effect and is not implied by readiness.
