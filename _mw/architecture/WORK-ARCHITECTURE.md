# Work Architecture — WA-0001

**Status:** accepted for bootstrap and current contour  
**Engineering Subject:** a small standalone Windows desktop document application developed within this repository  
**Boundary:** the complete development contour commissioned by the initial human request, from intake through a verified, locally buildable Product candidate and truthful closure  
**Bootstrap Baseline:** empty `madaraii-pilot-3` repository  
**Governing basis:** `ForestTiger-GH/MADARAII@7d5ef3d92c4e0982061d422208fdf913c55cc604`

## Drivers

The Work system must support: faithful preservation and development of the human input; external research before technical commitment; distinct Scientific Knowledge, Target WHAT, Target HOW, Product, Evidence, Decisions, Questions, and Work State; actual implementation in this repository; reproducible Windows verification; and cold recovery without chat memory.

## Semantic owners

| Role | Authoritative current path |
|---|---|
| Workspace front door | `_mw/WORKSPACE.md` |
| Work contour and Commission binding | `_mw/work/WORK-0001.md` |
| Current Work State | `_mw/work/STATE.md` |
| Raw actor inputs | `_mw/inbox/INDEX.md` and immutable carriers under `_mw/inbox/` |
| Developed actor input | `_mw/intake/INBOX-0001-developed.md` |
| Decisions | `_mw/decisions/DECISIONS.md` |
| Questions | `_mw/questions/QUESTIONS.md` |
| Research history | `_mw/research/` once established |
| Current Scientific Knowledge | `_mw/knowledge/SCIENCE.md` once admitted |
| Target WHAT | `_mw/knowledge/TARGET-WHAT.md` once admitted |
| Target HOW | `_mw/knowledge/TARGET-HOW.md` once admitted |
| Product | repository product surfaces resolved through `_mw/product/PRODUCT.md` once admitted |
| Evidence | `_mw/evidence/` with the current evidence index once established |
| Work/closure results | `_mw/results/` as required |

A path listed as “once established” is an owner route, not evidence that the artifact already exists.

## Work geometry

1. **Intake and bootstrap.** Preserve the raw carrier, develop semantic acts, realize only the workspace required for this contour.
2. **Problem-space knowledge.** Qualify and execute external Research where the project truly lacks knowledge; assimilate it into maintained Scientific Knowledge.
3. **Target definition and design.** Form Target WHAT from authorized intent plus admitted knowledge, then form Target HOW. Create a separate Product Architecture only if allocation has independent value.
4. **Realization.** Resolve the current-to-target implementation contour, form a bounded implementation contract, implement, and produce factual evidence.
5. **Assurance and integration.** Verify the exact candidate within the available environment, distinguish local Windows verification obligations, then admit only adequately supported Product state.
6. **Closure.** Reconcile Work State, Evidence, Knowledge, Product, Decisions, Questions, limitations, and reopen triggers. Stop when the commissioned contour is sufficiently complete.

This geometry is dependency-driven. It is not a mandatory numbered pipeline.

## Authority

The initiating human message is the Commission for this bounded development cycle and authorizes repository-local analysis, external research, design, implementation, documentation, and GitHub writes in `madaraii-pilot-3`. It prohibits use of other project repositories and prohibits third-party runtime/product dependencies. The agent may admit project-local engineering choices needed to fulfill the Commission when they stay inside the stated product boundary and are evidence-backed. Claims requiring execution on Windows remain bounded by available evidence.

## Artifact and resolver policy

- Stable identities are explicit in artifact headings or indexes when independent reference/recovery has value.
- Current owner paths are resolved through `_mw/WORKSPACE.md`; search and filenames are discovery aids only.
- Raw human carriers are immutable. Their interpretation lives elsewhere.
- Research history, current Science, Target WHAT, Target HOW, Product, and Evidence remain distinct owners.
- Derived summaries never replace the owner they summarize.
- Repository history supplies revision history; current files supply current owner state.
- Any mandatory input that resolves to zero, multiple, stale, or incompatible states blocks that dependent Work until reconciled.

## Continuity and closure

`_mw/work/STATE.md` is the durable recovery cursor. Each material transition updates its exact governing revision, established Results, current owner states, material residue, and next safe action. A cold actor starts from `_mw/WORKSPACE.md`, then follows only the required owner routes.

No separate transition package is required while this front door plus Work State can reconstruct the exact terminal or next-safe posture.
