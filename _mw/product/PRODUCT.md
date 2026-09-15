# PRODUCT-0001 — MiniDoc 0.1 Current Product

**Status:** admitted current repository Product Baseline  
**Admission date:** 2026-09-15  
**Version:** `0.1.0`  
**Verified Product source/configuration revision:** `4a7c1d5b2bea939ebd508dcce19c4e93f58149cf`  
**Verification:** `VERIFICATION-E001-01`  
**Evidence:** `EV-E001-01`  
**Integration:** `INTEGRATION-E001-01`

## Current Product identity

MiniDoc 0.1 is the repository's current admitted implementation of `TARGET-WHAT-0001` revision 2 and `TARGET-HOW-0001` revision 4. The Product realization is the source/build/install/test configuration under the following owner surfaces as they exist at verified candidate revision `4a7c1d5b2bea939ebd508dcce19c4e93f58149cf`:

- `src/MiniDoc/`
- `scripts/build.ps1`
- `scripts/verify-windows.ps1`
- `install/`
- `tests/MiniDoc.Checks/`
- `tests/fixtures/`
- `.github/workflows/windows-ci.yml`

The prior admitted Product Baseline was `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`. The EPOCH-001 repair candidate changed only four Product source/test files within the declared Product surfaces. Subsequent repository commits between `4a7c...` and admission changed Work/Knowledge/Evidence owners only; integration precheck found no Product-surface drift. The admitted Product configuration is therefore exactly the configuration verified as `4a7c...`, even though repository history continued for owner write-back.

## Admitted capability boundary

Within Target WHAT revision 2, the Product contains:

- first-party WPF Ribbon application shell;
- bounded basic DOCX editing with direct text/paragraph formatting and simple rectangular tables;
- structure-bounded Find/Replace;
- fail-closed read-only compatibility handling for richer DOCX semantics;
- local read-only PDF rendering/navigation/zoom through Windows PDF APIs;
- explicit Save/Save As filesystem boundary;
- self-contained `win-x64` build;
- per-machine install/uninstall with finite declared owner state;
- reproducible semantic and Windows lifecycle verification assets.

The admitted repair revision additionally preserves these mechanism invariants without enlarging the feature boundary:

- a replacement document/session is admitted before the prior active session is dismantled;
- stale asynchronous PDF render completion cannot mutate current presentation;
- simple-table row/cell width and WPF column descriptors remain coherent across add/delete operations;
- if both direct save and recovery fail, target integrity is reported as unknown instead of being represented as an ordinary recoverable save failure.

The exact supported/non-goal boundary is owned by `TARGET-WHAT-0001` and `docs/COMPATIBILITY.md`; this Product owner does not enlarge it.

## Admission/reliance envelope

Admission means this is the current repository Product Baseline. It does **not** mean:

- released to a distribution channel;
- deployed or installed on the user's workstation;
- outcome-validated on the user's documents;
- pixel/layout compatible with Microsoft Word;
- qualified by interactive human acceptance on Windows 11.

`VERIFICATION-E001-01` supports the four repair claims within its declared source/state-machine evidence limits and re-establishes Windows build, semantic checks, dependency boundary, packaging, install/PDF process-exit/uninstall lifecycle behavior on exact candidate `4a7c...`. Windows 11 interactive/visual qualification, broad real-document population coverage, timing stress for PDF races, malformed-file interactive replacement tests, and injected dual filesystem-failure tests remain bounded evidence gaps rather than admitted Product guarantees.

## Reproducible build output

Verified workflow run `34972485636` produced artifact `10397718020` (`MiniDoc-0.1.0-win-x64`) with GitHub-reported ZIP digest:

`sha256:8f75b3584f93ee8ddf9073a6371f6d365fb1fba23b7c6fa5c678817a5788fbb4`

Artifact retention is temporary. The durable Product owner is repository source plus build/install mechanisms; `EV-E001-01` preserves the execution evidence route after artifact expiry.

## Lineage and predecessor disposition

`b2caf...` remains a reproducible historical predecessor and retains its historical verification/evidence records. It is no longer the current Product Baseline. The EPOCH-001 candidate `4a7c...` is no longer merely a candidate: it is the admitted current Product configuration through `INTEGRATION-E001-01`.

No release, deployment, local installation, external-system, or user-document effect occurred as part of Product admission.

## Currentness and reopen triggers

Any material change to Product source, build/install/test configuration, dependency boundary, supported format semantics, Windows target, or lifecycle behavior creates a new Product candidate and invalidates affected verification claims until re-established.

A later reproducer showing replacement-session destruction, stale PDF UI mutation, table descriptor divergence, hidden save-integrity uncertainty, failed Windows lifecycle behavior, or failed Windows 11 local acceptance reopens only the affected verification/implementation responsibility rather than silently changing this owner.
