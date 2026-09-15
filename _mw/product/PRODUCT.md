# PRODUCT-0001 — MiniDoc 0.1 Current Product

**Status:** admitted current repository Product Baseline  
**Admission date:** 2026-09-15  
**Version:** `0.1.0`  
**Verified Product source/configuration revision:** `1df3b56528ac04b4f0d0a043593365b575007b93`  
**Verification:** `VERIFICATION-E001-02`  
**Evidence:** `EV-E001-02`  
**Integration:** `INTEGRATION-E001-02`

## Current Product identity

MiniDoc 0.1 is the repository's current admitted implementation of `TARGET-WHAT-0001` revision 3 and `TARGET-HOW-0001` revision 5. The Product realization is the source/build/install/test configuration under the following owner surfaces as they exist at verified candidate revision `1df3b56528ac04b4f0d0a043593365b575007b93`:

- `src/MiniDoc/`
- `scripts/build.ps1`
- `scripts/verify-windows.ps1`
- `install/`
- `tests/MiniDoc.Checks/`
- `tests/fixtures/`
- `.github/workflows/windows-ci.yml`

The prior admitted Product Baseline was `4a7c1d5b2bea939ebd508dcce19c4e93f58149cf`. Commits after `1df3b565...` and before admission affected Work/Evidence/Product-owner records only; integration precheck found no Product-source/configuration drift. The admitted Product configuration is therefore exactly the configuration verified as `1df3b565...`, even though repository history continues for owner write-back.

## Admitted capability boundary

Within Target WHAT revision 3, the Product contains:

- first-party WPF Ribbon application shell;
- bounded basic DOCX editing with direct text/paragraph formatting and simple rectangular tables;
- structure-bounded Find/Replace;
- fail-closed read-only compatibility handling for richer DOCX semantics using extracted text and explicit markers/placeholders rather than claiming unsupported object previews;
- local read-only PDF rendering/navigation/zoom through Windows PDF APIs;
- explicit Save/Save As filesystem boundary;
- self-contained `win-x64` build;
- per-machine install/uninstall with finite declared owner state;
- reproducible semantic and Windows lifecycle verification assets.

The admitted Product preserves the earlier repair invariants:

- replacement document/session admission is candidate-first;
- stale asynchronous PDF render completion cannot mutate current presentation;
- simple-table row/cell width and WPF column descriptors remain coherent across add/delete operations;
- if both direct save and recovery fail, target integrity is reported as unknown.

The second repair additionally admits these mechanism invariants without enlarging the feature boundary:

- unresolved Open authority is superseded by a later Open or New intent;
- replacement authority is also bound to the current document-content generation, so a later editable mutation invalidates an older Open ticket even when its intent is still latest;
- DOCX/PDF replacement checks ticket currentness at the admission boundary, and a stale PDF candidate is disposed rather than admitted;
- verification-mode success is returned only after successful selected-document admission and successful initial PDF render;
- verification-mode failure is noninteractive and non-zero so the Windows lifecycle gate has a truthful success oracle.

The exact supported/non-goal boundary is owned by `TARGET-WHAT-0001` and `docs/COMPATIBILITY.md`; this Product owner does not enlarge it.

## Admission/reliance envelope

Admission means this is the current repository Product Baseline. It does **not** mean:

- released to a distribution channel;
- deployed or installed on the user's workstation;
- outcome-validated on the user's documents;
- pixel/layout compatible with Microsoft Word;
- qualified by interactive human acceptance on Windows 11.

`VERIFICATION-E001-02` supports the second repair claims through exact source/state-machine reconstruction, deterministic transition-guard checks, and Windows CI execution on exact candidate `1df3b565...`. Windows workflow run `34976593344` re-established build, semantic checks, dependency boundary, packaging, installed PDF success-path execution, normal process exit, install/uninstall ownership and residue behavior on the tested environment.

A probabilistic live WPF timing-stress harness was not introduced. The current authority guarantee is therefore bounded to the present admission/mutation mechanisms represented by the exact candidate. A future editable mutation path that bypasses the document-content generation reopens the affected claim.

Earlier bounded evidence limitations remain where not superseded: broad real-document population qualification, interactive Windows 11 visual acceptance, malformed-file interactive replacement populations, and injected dual filesystem-failure testing are not implied by admission.

## Reproducible build output

Verified workflow run `34976593344` produced artifact `10399488049` (`MiniDoc-0.1.0-win-x64`) with GitHub-reported ZIP digest:

`sha256:87bc5994131f217d81c58cce9e6eb74eb9376fe03c366d6993dc3b11ba94e294`

Artifact retention is temporary. The durable Product owner is repository source plus build/install mechanisms; `EV-E001-02` preserves the execution evidence route after artifact expiry.

## Lineage and predecessor disposition

`b2cafabba970245b7ccb24f6f01a3ca2e2ba9681` and `4a7c1d5b2bea939ebd508dcce19c4e93f58149cf` remain reproducible historical predecessors with their historical verification/evidence records. They are no longer the current Product Baseline.

Producer attempt `2578a32599aa451b31d3db213c3e4f481911d655` failed before verification and was never admitted. Candidate `1df3b56528ac04b4f0d0a043593365b575007b93` is no longer merely a candidate: it is the admitted current Product configuration through `INTEGRATION-E001-02`.

No release, deployment, local workstation installation, external-system, network, persistent-runtime-state, or user-document effect occurred as part of Product admission.

## Currentness and reopen triggers

Any material change to Product source, build/install/test configuration, dependency boundary, supported format semantics, Windows target, lifecycle behavior, Open admission fencing, document-content freshness propagation, or verification-mode success oracle creates a new Product candidate and invalidates affected verification claims until re-established.

A reproducer showing replacement-session destruction, stale Open admission, stale PDF UI mutation, table descriptor divergence, hidden save-integrity uncertainty, failed installed PDF success oracle/lifecycle behavior, a mutation path bypassing content-generation fencing, or failed Windows 11 local acceptance reopens only the affected responsibility rather than silently changing this owner.
