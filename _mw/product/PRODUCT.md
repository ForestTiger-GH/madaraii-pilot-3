# PRODUCT-0001 — MiniDoc 0.1 Current Product

**Status:** admitted current repository Product Baseline  
**Admission date:** 2026-09-11  
**Version:** `0.1.0`  
**Verified Product source/configuration revision:** `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`  
**Verification:** `VERIFICATION-0001`  
**Evidence:** `EV-0001`  
**Integration:** `INTEGRATION-0001`

## Current Product identity

MiniDoc 0.1 is the repository's current admitted implementation of the accepted revision-2 Target. The Product realization is the source/build/install/test configuration under the following owner surfaces as they exist at candidate revision `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`:

- `src/MiniDoc/`
- `scripts/build.ps1`
- `scripts/verify-windows.ps1`
- `install/`
- `tests/MiniDoc.Checks/`
- `tests/fixtures/`
- `.github/workflows/windows-ci.yml`

Subsequent write-back commits through Product admission changed engineering/evidence owners only. Compare evidence from `b2caf...` through the verification write-back shows no Product-surface change; the admitted implementation is therefore exactly the verified configuration.

## Admitted capability boundary

Within `TARGET-WHAT-0001` revision 2, the Product contains:

- first-party WPF Ribbon application shell;
- bounded basic DOCX editing with direct text/paragraph formatting and simple rectangular tables;
- structure-bounded Find/Replace;
- fail-closed read-only compatibility handling for richer DOCX semantics;
- local read-only PDF rendering/navigation/zoom through Windows PDF APIs;
- explicit Save/Save As filesystem boundary;
- self-contained `win-x64` build;
- per-machine install/uninstall with finite declared owner state;
- reproducible semantic and Windows lifecycle verification assets.

The exact supported/non-goal boundary is owned by `TARGET-WHAT-0001` and `docs/COMPATIBILITY.md`; this Product owner does not duplicate or enlarge it.

## Admission/reliance envelope

Admission means this is the current repository Product Baseline. It does **not** mean:

- released to a distribution channel;
- deployed or installed on the user's workstation;
- outcome-validated on the user's documents;
- pixel/layout compatible with Microsoft Word;
- qualified by interactive human acceptance on Windows 11.

`VERIFICATION-0001` supports build, representative DOCX semantics, PDF render, dependency boundary, and tested install/process-exit/uninstall behavior. Windows 11 interactive/visual qualification and broad real-document population coverage remain bounded evidence gaps.

## Reproducible build output

Verified workflow run `34545614992` produced artifact `10178912526` (`MiniDoc-0.1.0-win-x64`) with GitHub-reported ZIP digest:

`sha256:54c235f7e1b8cab23ab615e930128ed6c18873be7076ec8427e03aad83f86f73`

Artifact retention is temporary. The durable Product owner is repository source plus build/install mechanisms, not the retained CI ZIP.

## Currentness and reopen triggers

Any material change to Product source, build/install/test configuration, dependency boundary, supported format semantics, Windows target, or lifecycle behavior creates a new Product candidate and invalidates affected verification claims until re-established.

A failed Windows 11 local acceptance run reopens only the affected verification/implementation responsibility rather than silently changing this owner.
