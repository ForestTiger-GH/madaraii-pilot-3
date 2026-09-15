# Product Architecture Reconciliation — ARCH-E001-01

**Instruction:** MADARAII-25  
**Subject:** accepted `PA-0001 revision 2` in `docs/ARCHITECTURE.md`  
**Current Product:** `1df3b56528ac04b4f0d0a043593365b575007b93`  
**Target inputs:** `TARGET-WHAT-0001` revision 3 / `TARGET-HOW-0001` revision 5  
**Trigger:** closure-preparation owner audit found stale Target revision and compatibility/Open-boundary wording in the current architecture owner.

## Separation value

A separate compact Product Architecture remains justified. MiniDoc still has independently meaningful Shell, Editor, DOCX, PDF, Storage, Build/Install and Verification failure/evolution boundaries. The epoch repairs did not collapse those responsibilities into Target HOW or create new deployment units.

## Reconciliation result

`PA-0001 revision 3` is justified with no Product implementation change:

- bind architecture to current Target WHAT rev.3 / Target HOW rev.5;
- keep the same realization units and dependency directions;
- make Shell explicitly own replacement-intent/content-generation fencing and candidate admission authority;
- state that PDF owns load/render resources only and that late render presentation authority is controlled by the Shell freshness fence;
- narrow DOCX compatibility extraction to safely extractable main-story text plus explicit markers/placeholders; current 0.1 architecture does not allocate media/notes-part dereference or graphics-preview rendering;
- remove stale failure wording that implied a compatibility-object rendering path that no current unit owns;
- keep Verification responsible for exact-candidate fixture/static/system checks and the installed PDF success/exit-code oracle without letting Verification redefine Product meaning.

No new component, interface, dependency, persistent state, external effect, feature, Target commitment or Product source change is introduced.

## Choice state

- separate compact architecture: `FIXED` / retained;
- realization-unit boundaries: `FIXED`, unchanged;
- Shell transition guard helper decomposition: implementation-local `BOUNDED_OPEN` so long as Shell remains authority owner;
- richer compatibility media/notes presentation: outside current architecture; later Target/Design change required;
- deployment topology: unchanged single-process desktop application.

## Verification / downstream

The architecture repair is a current-owner consistency update derived from already admitted Target/Product semantics. Separate MADARAII-26 review is not required before closure because no allocation alternative or new architecture decision exists; MADARAII-36 will independently audit the final architecture/owner consistency as part of the exact closure Baseline.

**Status:** architecture candidate revision 3 is admission-ready under the epoch's Product/architecture Authority; no implementation or re-verification of Product is required because source/configuration and protected behavior are unchanged.
