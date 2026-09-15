# EV-E001-01 — Post-Jester repair Windows CI evidence

**Evidence Subject:** Product candidate `4a7c1d5b2bea939ebd508dcce19c4e93f58149cf`  
**Workflow:** Windows CI  
**Workflow run:** `34972485636`  
**Job:** `104391895477` (`build-verify-lifecycle`)  
**Execution date:** 2026-09-15  
**Conclusion:** workflow/job `success`

## Exact candidate and lineage

The workflow executed against exact head SHA `4a7c1d5b2bea939ebd508dcce19c4e93f58149cf`. That candidate contains the four Product repair slices established by `IMPLEMENTATION-E001-01`: transactional replacement-session admission, stale PDF-render fencing, table column-descriptor coherence, and explicit target-integrity uncertainty when both direct write and recovery fail.

A later compare from `4a7c...` to repository `main` at integration precheck showed only epoch Work/Result files changed after the candidate; no Product owner surface changed. This allows Product admission to preserve the exact verified Product configuration even though repository history continued for Work Plane write-back.

## Observed CI outcomes

The exact candidate completed successfully through all Windows CI gates:

- `Build, semantic checks, publish, package` — success;
- `Check product dependency boundary` — success;
- `Verify install, PDF normal-close process exit, and uninstall residue` — success;
- `Upload install package` — success.

`build.ps1` builds and executes `MiniDoc.Checks` before publish. The candidate extends those semantic checks with deterministic table-column metadata checks while retaining prior search, DOCX round-trip, fail-closed unsupported-drawing, and actual Windows PDF-render checks.

The lifecycle gate exercised the packaged Product through per-machine install, PDF open/render and normal close with process termination, uninstall, owner-state cleanup, and user-document sentinel preservation under the existing verification script.

## Install-package artifact

- Artifact ID: `10397718020`
- Name: `MiniDoc-0.1.0-win-x64`
- Size: `71,593,830` bytes
- GitHub digest: `sha256:8f75b3584f93ee8ddf9073a6371f6d365fb1fba23b7c6fa5c678817a5788fbb4`
- Expiry reported by GitHub: 2026-12-14

Artifact retention is temporary. Durable reproducibility remains repository source plus build/install/verification mechanisms; the artifact is execution evidence for the exact candidate, not the Product owner.

## Evidence boundaries

Dynamic CI directly supports build/publish/package, deterministic semantic checks, first-party dependency boundary, and tested install/PDF-process-exit/uninstall lifecycle behavior.

The following repair claims also rely on exact candidate source/state-machine inspection recorded by `VERIFICATION-E001-01` rather than dedicated timing/fault-injection harnesses:

- failed replacement open cannot dismantle the prior session before candidate success;
- stale PDF render completion lacks authority to mutate current presentation;
- dual direct-write plus recovery failure reports target integrity as unknown.

No stress-level race test, malformed-file interactive UI test, dual filesystem-failure injection, broad real-document population qualification, or interactive Windows 11 visual acceptance is claimed.
