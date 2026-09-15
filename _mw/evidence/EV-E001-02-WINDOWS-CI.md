# EV-E001-02 — Second Jester repair Windows CI evidence

**Evidence Subject:** Product candidate `1df3b56528ac04b4f0d0a043593365b575007b93`  
**Workflow:** Windows CI  
**Workflow run:** `34976593344`  
**Job:** `104405752141` (`build-verify-lifecycle`)  
**Execution date:** 2026-09-15  
**Conclusion:** workflow/job `success`

## Exact candidate

The workflow executed against exact head SHA `1df3b56528ac04b4f0d0a043593365b575007b93`.

This candidate is the bounded second post-Jester repair established by `IMPLEMENTATION-E001-02`: Open intent/content-generation authority fencing, stale-candidate rejection/disposal at the shell admission boundary, and an installed PDF verification path whose success requires a successful first-page render and process exit code `0`.

The immediately preceding producer attempt `2578a32599aa451b31d3db213c3e4f481911d655` failed at the build/semantic-check stage because the new check project lacked `using MiniDoc;`. It is retained as failed producer evidence and is not the subject of this evidence record.

A compare after candidate freeze showed only Work Plane artifacts added after `1df3b565...`; no Product source/configuration surface changed before verification/integration processing.

## Observed CI outcomes

The exact candidate completed successfully through all Windows CI gates:

- `Build, semantic checks, publish, package` — success;
- `Check product dependency boundary` — success;
- `Verify install, PDF normal-close process exit, and uninstall residue` — success;
- `Upload install package` — success.

The semantic-check executable includes deterministic tests for the Open transition guard in addition to the previously established search, DOCX round-trip, simple-table geometry, unsupported-drawing fail-closed behavior, and Windows PDF rendering checks.

The installed lifecycle gate now starts packaged `MiniDoc.exe` with `--verify-close-after-open`, waits for process termination, refreshes the process object, and requires `ExitCode == 0`. Product startup returns success only after the selected PDF is admitted and its first page is successfully rendered; verification-mode failures suppress modal error blocking and exit non-zero. Existing install ownership, process-exit, uninstall cleanup, AppData/Temp residue, and user-document sentinel checks remain in the same gate.

## Install-package artifact

- Artifact ID: `10399488049`
- Name: `MiniDoc-0.1.0-win-x64`
- Size: `71,595,383` bytes
- GitHub digest: `sha256:87bc5994131f217d81c58cce9e6eb74eb9376fe03c366d6993dc3b11ba94e294`
- Expiry reported by GitHub: 2026-12-14

Artifact retention is temporary. The durable reproducibility path remains repository source plus build/install/verification mechanisms and this evidence record.

## Evidence boundaries

Dynamic CI directly supports build/publish/package, semantic checks, first-party dependency boundary, installed PDF success-path execution, normal process exit, install/uninstall ownership and tested residue constraints on the workflow environment.

The race/authority repair additionally relies on exact source/state-machine reconstruction in `VERIFICATION-E001-02`: deterministic generation tests prove the local guard semantics, while shell source inspection proves that those generations are checked immediately before DOCX/PDF admission and that stale PDF candidates are disposed.

No probabilistic WPF timing-stress harness, broad real-document population qualification, interactive Windows 11 visual acceptance, or future editor-mutation-path claim is made.
