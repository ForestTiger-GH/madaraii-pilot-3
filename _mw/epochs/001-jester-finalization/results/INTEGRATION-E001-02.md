# Integration Result — INTEGRATION-E001-02

**Instruction:** MADARAII-34  
**Integration Commission / Authority:** EPOCH-001 human Commission as specialized through `RECON-E001-0007` and the admitted `PLAN-E001-02` contour  
**Pre-integration Product:** `PRODUCT-0001 / 4a7c1d5b2bea939ebd508dcce19c4e93f58149cf`  
**Candidate:** `1df3b56528ac04b4f0d0a043593365b575007b93`  
**Verification:** `VERIFICATION-E001-02 = CONFORMS`  
**Evidence:** `EV-E001-02`  
**Target:** `TARGET-WHAT-0001` revision 3 / `TARGET-HOW-0001` revision 5

## Binding and eligibility

The exact candidate is a dependent successor repair to the currently admitted `4a7c...` Product, not an alternative Product variant. It carries only the second-Jester bounded shell/open-verification delta on top of the predecessor realization.

Immediately before admission, compare `1df3b565... -> main` showed four later commits affecting only epoch Implementation/Verification Results and Evidence owners. No Product source/build/install/test surface changed after the candidate freeze. Therefore the integrated Product configuration is byte/source-identical to the exact configuration verified by `VERIFICATION-E001-02`; no post-verification Product transformation or re-verification is required.

No competing candidate requires fan-in. Failed producer attempt `2578a325...` is terminal historical producer evidence and has no admission status.

## Candidate disposition

| Candidate / state | Disposition |
|---|---|
| `4a7c1d5...` | historical predecessor after owner transition; remains reproducible with `VERIFICATION-E001-01` / `EV-E001-01` |
| `2578a325...` | failed producer attempt; never verified/admitted |
| `1df3b565...` | verified unchanged candidate; admitted as the new current `PRODUCT-0001` Product Baseline |

## Admitted Product delta

The owner transition admits only the verified second-repair semantics:

1. **Open-intent freshness:** a later Open or New transition supersedes unresolved earlier Open authority.
2. **Document-content freshness:** an Open ticket is bound to the document-content generation at capture; a later editable mutation invalidates replacement authority even when the Open intent itself remains latest.
3. **Admission fencing:** DOCX/PDF candidate replacement occurs only while the captured ticket remains current; stale PDF candidates are disposed instead of becoming current.
4. **Installed PDF success oracle:** verification-mode success is returned only after successful selected-document admission and successful initial PDF render; verification failure is noninteractive and non-zero.
5. **Windows evidence strengthening:** lifecycle verification now requires installed process exit code zero in addition to the existing build/dependency/install/process-exit/uninstall/residue controls.

No user-facing feature boundary is enlarged.

## Identity of integrated configuration

The Product source/configuration owner surfaces remain:

- `src/MiniDoc/`
- `scripts/build.ps1`
- `scripts/verify-windows.ps1`
- `install/`
- `tests/MiniDoc.Checks/`
- `tests/fixtures/`
- `.github/workflows/windows-ci.yml`

Their admitted configuration is the exact state at `1df3b56528ac04b4f0d0a043593365b575007b93`. Later `_mw` owner write-back commits do not redefine that Product configuration.

## Effects and currentness

**Repository Product owner transition:** `4a7c1d5... -> 1df3b565...`.  
**Release:** not performed.  
**Deployment / workstation installation:** not performed by integration.  
**User-document effect:** none.  
**Network / external-system effect:** none.  
**Persistent runtime state:** unchanged; none introduced.  
**UNKNOWN external outcome:** none.

Target WHAT revision 3 and Target HOW revision 5 remain current and are now realized by the admitted Product within the verified repair boundary. `docs/COMPATIBILITY.md` already expresses the reconciled marker/text-only compatibility semantics and requires no integration-time mutation. Scientific Knowledge is unaffected.

No separate Current HOW owner exists in this workspace profile. Factual current realization for the admitted configuration is carried by the Product owner plus exact Product source and is consistent with Target HOW for this repaired slice; no unfinished Current HOW transition remains.

## Verification/reliance envelope after admission

Admission may rely on:

- deterministic guard checks for intent/content generations;
- exact source/control-flow reconstruction at the actual shell admission boundary;
- Windows CI run `34976593344` on exact candidate `1df3b565...`;
- installed artifact `10399488049`, GitHub digest `sha256:87bc5994131f217d81c58cce9e6eb74eb9376fe03c366d6993dc3b11ba94e294` while retained.

The absence of a probabilistic WPF timing-stress harness remains an explicit bounded evidence limitation, not a hidden guarantee. Future mutation paths that do not advance the content generation reopen the affected authority claim.

## Downstream reconciliation

- `PLAN-E001-02`: implementation/verification/integration contour complete; terminal after owner transition.
- `IMPLEMENTATION-E001-02`: historical producer Result; final candidate admitted.
- `VERIFICATION-E001-02`: current verification evidence for this Product repair slice.
- `EV-E001-02`: current Windows evidence route for the admitted second repair.
- Target WHAT/HOW: remain current, no reopen.
- Product predecessor `4a7c...`: historical, no longer current.
- Jester-2 acts `JR2-A1/A2/A4`: resolved through admitted repair; `JR2-A3` resolved through WHAT/HOW reconciliation; `JR2-A5/A6` remain terminal no-action.
- Epoch summaries, Development Report, closure audit and finalization become eligible after Work State reconciliation.

## Recovery and reopen

The prior Product is reproducible from `4a7c...`; no external or stateful effect requires rollback. Reopen the affected slice on Product source/configuration change, Target change, admission-boundary refactor, new mutation path bypassing content-generation fencing, Windows verification-oracle change, or evidence that a stale Open can replace newer authoritative state.

**Status:** integration/admission complete; exact Product Baseline `1df3b56528ac04b4f0d0a043593365b575007b93` is current once reflected by the Product owner. No release, deployment or outcome validation is implied.
