# Verification Result — VERIFICATION-E001-02

**Instruction:** MADARAII-33  
**Subject:** exact implementation candidate `1df3b56528ac04b4f0d0a043593365b575007b93`  
**Pre-change Product:** `PRODUCT-0001 / 4a7c1d5b2bea939ebd508dcce19c4e93f58149cf`  
**Plan:** `PLAN-E001-02`  
**Target:** `TARGET-WHAT-0001` revision 3 / `TARGET-HOW-0001` revision 5  
**Implementation Result:** `IMPLEMENTATION-E001-02`  
**Windows evidence:** workflow run `34976593344`, job `104405752141`, artifact `10399488049`

## Independence and exact binding

Verification was reconstructed from the admitted Plan/Target semantics and exact candidate source before relying on the executor narrative. The failed producer attempt `2578a32599aa451b31d3db213c3e4f481911d655` is not the verification Subject.

Repository comparison from candidate `1df3b565...` to current `main` shows only the later Implementation Result artifact; no Product source changed after the candidate freeze. The tested candidate therefore remains exact and reproducible.

The verification boundary is bounded to the second Jester repair contour: asynchronous replacement authority, post-prompt document mutation freshness, installed PDF success oracle, and absence of unplanned Product-surface mutation. This Result does not re-verify unrelated MiniDoc 0.1 semantics already admitted at the predecessor Product except where the exact Windows workflow necessarily re-exercised them.

## Expected behavior reconstructed from Target and Plan

1. A later Open or New transition must supersede an unresolved earlier Open attempt.
2. A user-visible document mutation after an Open ticket is captured must invalidate that ticket even when the Open intent is still the latest intent.
3. Candidate loading may complete internally after supersession, but stale work has no authority to replace the current session.
4. A stale PDF candidate that has acquired resources must be disposed instead of becoming current.
5. Verification mode may report success only after candidate admission and successful first-page PDF render; open/render failure must terminate non-zero without modal blocking.
6. The repair must not add persistent runtime state, dependencies, network behavior, release/deployment effects, or unrelated Product changes.

## Evidence and per-claim conclusions

| Claim | Independent method / oracle | Evidence | Conclusion |
|---|---|---|---|
| later intent supersedes earlier unresolved Open | source/state-machine reconstruction plus deterministic guard checks | `OpenTransitionGuard.BeginOpenIntent/SupersedeOpen`, `IsCurrentIntent/IsCurrent`; `CheckOpenTransitionGuard` exercises second-intent and New-style supersession | `CONFORMS` |
| content mutation invalidates prior replacement authority | source/state-machine reconstruction plus deterministic guard checks | ticket captures `(Intent, ContentRevision)`; `MarkDirty()` advances content generation; guard check proves same intent becomes non-current after `ContentChanged()` | `CONFORMS` |
| shell admits only a current ticket | direct source inspection of exact candidate | DOCX checks `CanAdmitOpen` before session release; PDF checks it after async `PdfSession.OpenAsync` and before `ReleasePdf`; stale candidate is disposed | `CONFORMS` |
| stale render completion cannot overwrite later presentation | regression inspection of existing render fence | render request identity + session/page/zoom checks remain intact; `ReleasePdf` increments request identity and disposes current PDF | `CONFORMS` |
| verification success means initial PDF render succeeded | source reconstruction plus installed-system execution | `OpenPdfAsync` returns `RenderPdfAsync`; render returns true only after bitmap assignment/currentness check; `App` exits `2` on false in verification mode | `CONFORMS` |
| verification failure is noninteractive and non-zero | direct exact-source inspection | verification mode calls `OpenInitialPathAsync(..., showErrors:false)`; open/render error paths suppress `MessageBox`; false/missing path exits `2` | `CONFORMS` |
| installed artifact exercises success oracle and lifecycle | exact Windows workflow evidence | run `34976593344`, job `104405752141`: build/semantic checks/package, dependency boundary, installed PDF open/render + process exit + uninstall residue, artifact upload all `success` | `CONFORMS` |
| no unplanned Product mutation in repair contour | exact commit comparison | `4a7c... -> 1df3...` Product delta is limited to `scripts/verify-windows.ps1`, `App.xaml.cs`, `MainWindow.Startup.cs`, `MainWindow.xaml.cs`, new `OpenTransitionGuard.cs`, and `tests/MiniDoc.Checks/Program.cs`; remaining changes are Work/Knowledge/Evidence owners | `CONFORMS` |

## Strongest-failure challenge

The important adversarial schedules are:

- Open A begins; Open B begins before A completes → B advances intent; A cannot admit.
- Open A begins; New occurs before A completes → New supersedes intent; A cannot admit.
- Open A is authorized after Save/Discard; current editable content changes before A completes → content generation changes; A cannot admit and reports cancellation rather than replacing the edited document.
- PDF A obtains a candidate session after it has become stale → currentness check fails and candidate is disposed before session replacement.
- admitted PDF render becomes stale because another transition releases/replaces PDF state → render-request/session fence rejects late UI mutation.

The deterministic guard checks directly cover intent and content generations. Source inspection covers their integration at the actual admission boundary. The Windows system run covers the successful installed path and the strengthened success oracle.

## Coverage limitation

No probabilistic/live WPF timing-stress harness intentionally delays file opening while synthesizing concurrent UI actions. Therefore the temporal claims are supported by deterministic state-machine tests and exact source/control-flow reconstruction rather than statistical race reproduction. This is a declared evidence limitation, not an `EVIDENCE_GAP` blocking the bounded admission claim: the protected authority decision is explicit, deterministic and locally testable, and the installed success path is separately executed on Windows.

The existing Ribbon direct routed formatting commands were not introduced by this repair and are outside this verification claim except insofar as the existing editor dirty/event path remains unchanged. This Result does not generalize to all possible future editor mutation mechanisms; any new mutation path that bypasses the document-content generation is a reopen trigger.

## Expected-to-actual delta

All Plan slices are present. No extra Product feature or persistent/external behavior was introduced. The producer's first candidate failed to compile in the check project because of a missing namespace import; the final candidate contains only the bounded correction and independently passes the exact Windows workflow.

No observed external or irreversible effect is `UNKNOWN`.

## Verdict and reliance envelope

**Overall typed verdict:** `CONFORMS` for the exact candidate `1df3b56528ac04b4f0d0a043593365b575007b93` against `PLAN-E001-02`, Target WHAT revision 3 and Target HOW revision 5 within the declared second-repair scope.

Supported downstream reliance:

- candidate is eligible for MADARAII-34 Product integration/admission;
- the strengthened Windows workflow is sufficient evidence for the installed PDF success-oracle/lifecycle claim on the tested Windows CI configuration;
- the open-transition authority repair is supported for the current MiniDoc mutation/admission paths represented by the candidate.

Not established by this Result: release, deployment, business/user outcome validation, universal Office/PDF compatibility, or race safety for future mutation mechanisms not connected to the guard.

## Reopen triggers

Candidate/source change; Target WHAT/HOW change; admission-boundary refactor; new editable mutation mechanism bypassing `MarkDirty`/content generation; change to verification-mode startup semantics; Windows workflow/oracle change; dependency/environment change material to tested behavior.

**Status:** verification complete; unchanged exact candidate may proceed to MADARAII-34. No repair was performed during verification.
