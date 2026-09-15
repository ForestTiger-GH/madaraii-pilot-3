# EPOCH-001 Development Report — Post-closure Jester finalization

**Instruction:** MADARAII-35  
**Report state:** closure-audit candidate; unsealed  
**Epoch:** `EPOCH-001` — Post-closure Jester finalization  
**Initiating Commission:** human request of 2026-09-15, preserved as `INBOX-0001` and developed as `INPUT-DEV-E001-0001`  
**Outcome horizon:** challenge the already closed MiniDoc with two sequential independent Jester passes, process every retained signal through ordinary non-Jester engineering, admit only justified changes, then form summaries, Development Report, closure audit and archive-readiness  
**Start Product:** `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`  
**Cutoff Product:** `1df3b56528ac04b4f0d0a043593365b575007b93`  
**Cutoff Target:** `TARGET-WHAT-0001` revision 3 / `TARGET-HOW-0001` revision 5  
**Audience / use:** durable epoch history for closure audit, later archival orientation and cold reconstruction of the development outcome.

This Report owns the epoch-level historical synthesis only. Current Product, Knowledge, Decision, Evidence, Question and Work-State meaning remain in their semantic owners.

## Starting posture

EPOCH-001 began after legacy `WORK-0001` had already closed successfully. The starting MiniDoc Product was exact verified revision `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`; that historical closure was not invalidated or reopened.

At epoch start:

- Product `b2caf...` was admitted current;
- Target WHAT and Target HOW were revision 2;
- Scientific Knowledge was established and remained adequate for the bounded post-closure work;
- MiniDoc was already a deliberately small Windows 11 x64 DOCX editor/PDF viewer with fail-closed DOCX admission, compatibility mode, first-party WPF/Windows PDF stack, zero runtime technical state and bounded install/uninstall ownership;
- Q-0006 interactive Windows 11 qualification and Q-0007 broad real-document compatibility were explicit reliance limits rather than implementation blockers;
- release, deployment and user-workstation installation were outside the epoch state.

The Commission authorized two sequential Jester challenges, compulsory non-Jester continuation of each Report, justified Product/Knowledge changes only through ordinary owner/verification paths, then summaries/report/audit/finalization. It did not preclassify Jester surprises as defects or required repairs.

## Substantive Product outcome

The epoch advanced MiniDoc through two separately verified Product revisions:

`b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`
→ `4a7c1d5b2bea939ebd508dcce19c4e93f58149cf`
→ `1df3b56528ac04b4f0d0a043593365b575007b93`

The user-facing feature boundary did not expand. The changes strengthened correctness, authority boundaries and evidence around existing behavior:

- replacement-session admission became candidate-first;
- asynchronous PDF render completion gained request/session/page/zoom freshness fencing;
- WPF table column descriptors remain coherent with row-cell width after column edits;
- dual direct-save plus recovery failure now reports target-integrity uncertainty explicitly;
- asynchronous Open requests gained monotonic user-intent freshness;
- Open replacement authority is also bound to the document-content generation captured after unsaved-change handling;
- stale asynchronously acquired PDF candidates are disposed rather than admitted;
- verification-mode success now requires successful selected-document admission and successful initial PDF render, with noninteractive non-zero failure.

Current Product owner binds exact source/configuration revision `1df3b56528ac04b4f0d0a043593365b575007b93`. Later `_mw` reporting/owner commits do not redefine that Product configuration.

## Knowledge and Decision outcome

Scientific Knowledge remained current and unchanged; no new substantive Research was required.

Target HOW evolved from revision 2 to revision 5 through bounded reconciliations. Early reconciliation removed overstated compatibility-preview/search mechanisms; the first repair then incorporated candidate-first replacement, PDF freshness, table geometry and save-recovery semantics; revision 5 added Open intent/content-generation authority fencing and the strengthened installed PDF success oracle.

Target WHAT remained revision 2 through the first repair. The second Jester cycle exposed that its optional graphics/notes compatibility language no longer matched current HOW/Product/public behavior, so `WHAT-E001-02` established current revision 3 with a marker/text-only compatibility boundary.

Development Report source accounting then found one remaining owner inconsistency: historical Decisions D-0012/D-0013 still carried richer preview/notes-body clauses. `RECON-E001-0009` reconciled the Decision owner without changing Product or Target. Their compatibility-only rationale remains historical/current where applicable, while the richer presentation clauses are superseded for current 0.1 and accepted D-0015 owns the current marker/text-only decision.

## First Jester cycle

**Work:** `JST-E001-01`  
**Standalone Report:** `_mw/epochs/001-jester-finalization/results/jester/JR-E001-01.md`  
**Subject:** exact Product `b2caf...`  
**Mode:** `OBSERVATION`

The session deliberately attacked state/authority assumptions beyond ordinary verification. It retained six source/design observations: premature replacement-session teardown; stale PDF render completion; table descriptor divergence; compatibility preview/notes overstatement; nonexistent case-sensitive UI toggle despite HOW wording; and hidden target-integrity uncertainty when both save and restoration fail.

Observation mode performed no Product/Knowledge/external mutation, so no restoration was required. The Jester Report itself established observations only.

Mandatory continuation developed the report under MADARAII-03 and reconciled every act under MADARAII-04 before specialized follow-up. Useful mechanism overstatements were corrected rather than turned into features; justified Product repairs were planned, readiness-reviewed, implemented, independently verified and admitted. Exact candidate `4a7c1d5...` became current through `INTEGRATION-E001-01`; `b2caf...` remained reproducible historical lineage.

## Second independent Jester cycle

**Work:** `JST-E001-02`  
**Standalone Report:** `_mw/epochs/001-jester-finalization/results/jester/JR-E001-02.md`  
**Subject:** then-current Product `4a7c1d5...`, Target WHAT rev.2 / Target HOW rev.4  
**Mode:** `OBSERVATION`

Prior Jester creative framing was excluded from the second session. The new pass found a distinct authority class: render callbacks were now fenced, but asynchronous candidate **Open** admission itself was not. A slow earlier PDF Open could finish after a later New/Open intent and become current by completion order.

It also found that Save/Discard permission could become stale: after that decision but while PDF loading was pending, the still-active DOCX could be edited again; late admission could otherwise discard the newer edit without a fresh authority check.

Additional signals were a Target WHAT vs HOW/Product compatibility mismatch and an installed lifecycle oracle gap: process exit after an attempted PDF path did not itself prove successful installed UI admission/render. The memory cost of overwriting an enormous existing target was retained only as a bounded fragility/no-action item; a table insertion-vs-growth asymmetry established no target contradiction and was also terminal no-action.

Again, Observation mode mutated no authoritative state. `_mw/epochs/001-jester-finalization/intake/JR-E001-02-developed.md` plus MADARAII-04 reconciliation completed mandatory non-Jester continuation before any repair was authorized.

## Second repair mechanism

`WHAT-E001-02` and `HOW-E001-02` first reconciled target semantics. `PLAN-E001-02` then bounded implementation to asynchronous Open authority and installed verification; `READINESS-E001-02` found the Plan ready.

The implementation introduced `OpenTransitionGuard` with two monotonic dimensions:

- **intent generation** — which Open/New replacement intent is still current;
- **content generation** — whether the current editable document changed after an Open ticket captured replacement authority.

The shell captures both in an immutable ticket and rechecks them at candidate admission. DOCX admission occurs before current-session release. PDF loading may complete asynchronously, but stale candidates cannot replace current state and are disposed. Existing PDF render freshness remains a separate fence after admission.

The verification-only startup path now returns actual success/failure. PDF open returns success only after the first render succeeds. `--verify-close-after-open` suppresses modal failure blocking and exits non-zero on failure. Windows lifecycle verification refreshes the installed process and requires `ExitCode == 0`.

## Failed producer attempt retained

The second repair had one failed producer candidate: `2578a32599aa451b31d3db213c3e4f481911d655`. Windows CI failed at `Build, semantic checks, publish, package`; later steps were skipped. The new semantic check referenced internal `OpenTransitionGuard` without importing namespace `MiniDoc`.

This was a compile-only check-project defect within the admitted Plan. Adding `using MiniDoc;` produced final candidate `1df3b565...`. The failed candidate remains historical producer evidence and was never verified or admitted.

## Verification, evidence and final admission

`VERIFICATION-E001-02` independently reconstructed the second repair claims from Plan/Target and exact candidate source. It concluded `CONFORMS` for exact `1df3b565...` within the bounded repair scope:

- deterministic guard checks establish later-intent supersession and content-generation invalidation;
- exact shell control-flow inspection establishes those checks at the real admission boundary and stale PDF candidate disposal;
- existing render-request/session fencing protects stale render completion;
- verification-mode source establishes success only after initial render and noninteractive non-zero failure;
- exact commit comparison shows no unplanned Product-surface mutation.

Windows CI run `34976593344`, job `104405752141`, completed build/semantic checks/package, dependency boundary, installed PDF open/render + normal process exit + uninstall/residue verification and artifact upload successfully on exact `1df3b565...`.

`EV-E001-02` records artifact `10399488049`, `MiniDoc-0.1.0-win-x64`, GitHub digest `sha256:87bc5994131f217d81c58cce9e6eb74eb9376fe03c366d6993dc3b11ba94e294` while retained.

`INTEGRATION-E001-02` established identity-preserving admission: later repository commits after candidate freeze affected only Work/Evidence/owner records, so exact verified `1df3b565...` became current Product without post-verification Product transformation.

## Evidence limits and protected non-claims

No probabilistic/live WPF timing-stress harness was introduced. Open race protection is therefore supported by deterministic state-machine checks and exact source/control-flow reconstruction, while installed success behavior is executed separately on Windows CI. A future editable mutation mechanism bypassing content-generation fencing reopens the affected claim.

The epoch does not establish interactive Windows 11 visual acceptance, universal real-document compatibility, Word pixel/layout fidelity, exhaustive injected filesystem-failure testing, release/deployment/user-workstation installation, or user/business outcome validation. These remain explicit boundaries rather than hidden successor Tasks.

## Effects and final substantive state

Repository Product, Target, Decision and Evidence owners changed. No production, release, deployment, external-system, network, user-document, persistent runtime-state or irreversible external effect occurred. `UNKNOWN external effect`: none.

At Report cutoff:

- Product `PRODUCT-0001` is current at exact `1df3b56528ac04b4f0d0a043593365b575007b93`;
- Target WHAT revision 3 and Target HOW revision 5 are current;
- Scientific Knowledge is unchanged/current;
- Decisions are reconciled through D-0015; richer current-0.1 preview/body clauses in D-0012/D-0013 are superseded while their historical rationale remains auditable;
- `EV-E001-02` is the current Windows evidence route for the second repair; earlier evidence remains lineage history;
- Questions contain no human-, implementation- or integration-blocking item; Q-0006/Q-0007 remain bounded reliance limits with reopen triggers;
- both Jester Reports have complete mandatory non-Jester continuation and terminal downstream accounting;
- both Product repair contours are implemented, verified, integrated and terminal;
- release/deployment/validation remain not performed.

## Development summaries

- `_mw/epochs/001-jester-finalization/results/SUMMARY-E001-01.md` — first Jester repair block (`b2caf... → 4a7c...`);
- `_mw/epochs/001-jester-finalization/results/SUMMARY-E001-02.md` — second Jester repair block (`4a7c... → 1df3...`).

They are derived navigation views. This Report likewise does not replace underlying semantic owners.

## Residue and next posture

No material Product, Knowledge or Decision transition remains routed but unfinished. Jester-derived no-action items are terminally dispositioned rather than implied Tasks. There is no unresolved external effect.

The remaining commissioned Work is governance only: audit this exact substantive state under MADARAII-36. An accepted `PASS` may route MADARAII-37 on unchanged audited owners. Physical deletion or destructive archive relocation is not implied by archive-readiness.

## Exact source manifest

### Epoch and Commission
- `_mw/epochs/001-jester-finalization/EPOCH.md`
- `_mw/epochs/001-jester-finalization/inbox/archive/INBOX-0001.md`
- `_mw/epochs/001-jester-finalization/intake/INBOX-0001-developed.md`
- `_mw/epochs/001-jester-finalization/work/STATE.md`

### Current owners
- `_mw/product/PRODUCT.md`
- `_mw/knowledge/TARGET-WHAT.md`
- `_mw/knowledge/TARGET-HOW.md`
- `_mw/knowledge/SCIENCE.md`
- `_mw/decisions/DECISIONS.md`
- `_mw/questions/QUESTIONS.md`
- `_mw/evidence/INDEX.md`
- `_mw/AGENTS.md`

### Jester and mandatory continuation
- `_mw/epochs/001-jester-finalization/results/jester/JR-E001-01.md`
- `_mw/epochs/001-jester-finalization/intake/JR-E001-01-developed.md`
- `_mw/epochs/001-jester-finalization/results/jester/JR-E001-02.md`
- `_mw/epochs/001-jester-finalization/intake/JR-E001-02-developed.md`
- `_mw/epochs/001-jester-finalization/results/RECONCILIATION-0009.md`

### Product repair lineage
- `_mw/epochs/001-jester-finalization/results/HOW-E001-01.md`
- `_mw/epochs/001-jester-finalization/results/IMPLEMENTATION-E001-01.md`
- `_mw/epochs/001-jester-finalization/results/VERIFICATION-E001-01.md`
- `_mw/epochs/001-jester-finalization/results/INTEGRATION-E001-01.md`
- `_mw/epochs/001-jester-finalization/results/WHAT-E001-02.md`
- `_mw/epochs/001-jester-finalization/results/HOW-E001-02.md`
- `_mw/epochs/001-jester-finalization/results/IMPLEMENTATION-E001-02.md`
- `_mw/epochs/001-jester-finalization/results/VERIFICATION-E001-02.md`
- `_mw/epochs/001-jester-finalization/results/INTEGRATION-E001-02.md`
- `_mw/epochs/001-jester-finalization/results/RECONCILIATION-0008.md`
- `_mw/evidence/EV-E001-01-WINDOWS-CI.md`
- `_mw/evidence/EV-E001-02-WINDOWS-CI.md`

### Reader views
- `_mw/epochs/001-jester-finalization/results/SUMMARY-E001-01.md`
- `_mw/epochs/001-jester-finalization/results/SUMMARY-E001-02.md`

Version-control history is the deep route for exact source/configuration revisions and failed producer attempt lineage.

## Closure-readiness statement

At this candidate cutoff the commissioned substantive development is reconciled, every material current lesson has a semantic owner outside this Report, and the source manifest is repository-relative and deterministic. The Report is ready for MADARAII-36 closure audit.

This statement is not a closure verdict, seal, archive permission, Product admission, release or deployment claim. Report state remains **candidate / unsealed** until accepted closure audit and MADARAII-37 finalization.
