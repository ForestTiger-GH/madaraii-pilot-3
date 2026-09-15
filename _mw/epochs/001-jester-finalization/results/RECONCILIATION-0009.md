# Work State / Decision Owner Reconciliation — RECON-E001-0009

**Instruction:** MADARAII-04  
**Trigger:** Development Report source-accounting discovered a conflict between current Decision owner and already-admitted Target/Product state.  
**Current Product:** `1df3b56528ac04b4f0d0a043593365b575007b93`  
**Current Target:** WHAT revision 3 / HOW revision 5

## Exact conflict

`DECISIONS.md` still described `D-0012` and `D-0013` as accepted current decisions permitting, where safely resolvable, relationship-backed raster/fallback preview and footnote/endnote body presentation in compatibility mode.

Those statements belonged to the pre-reconciliation target state. The second Jester continuation and `WHAT-E001-02` / `HOW-E001-02` explicitly resolved current MiniDoc 0.1 in the opposite narrower direction:

- compatibility mode shows safely extractable main-story text plus explicit markers/placeholders;
- current 0.1 does not dereference package media relationships to render graphic previews;
- current 0.1 does not dereference notes parts to append footnote/endnote bodies;
- richer presentation requires a later target/design change rather than being latent current behavior.

Current Product `1df3b565...` implements that narrower boundary. Therefore leaving `D-0012/D-0013` as unqualified current accepted decisions would create a competing semantic owner at closure.

## Disposition

This is an owner-currentness repair, not a new Product feature/design selection. The authoritative choice was already admitted through Target WHAT/HOW under the epoch's Product/Decision Authority and realized by the current Product.

Decision-owner reconciliation is justified as follows:

1. retain `D-0012` and `D-0013` as historical decisions that explain why graphics/notes/fields remain compatibility-only rather than editable;
2. mark their richer preview/body-presentation clauses superseded for current MiniDoc 0.1;
3. add `D-0015` as the accepted current compatibility-presentation decision: marker/text-only main-story presentation, no media/notes-part dereference in 0.1, richer preview/body presentation requiring a later target/design change.

No Target WHAT/HOW or Product mutation is required because those owners already express the current decision correctly.

## Impact

- Decision owner becomes consistent with Target WHAT rev.3, Target HOW rev.5, `docs/COMPATIBILITY.md`, and current Product.
- `D-0012/D-0013` historical rationale remains auditable; no history is erased.
- no implementation, verification, release, deployment, user-document, runtime or external effect occurs;
- existing `VERIFICATION-E001-02` remains applicable because candidate/configuration and criteria are unchanged.

**Status:** bounded Decision-owner repair authorized and required before Development Report cutoff / closure audit.
