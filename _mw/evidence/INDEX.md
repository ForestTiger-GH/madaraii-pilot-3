# EVIDENCE-INDEX-0001 — MiniDoc 0.1

Current Evidence owner for the admitted MiniDoc 0.1 Product lineage.

| Evidence | Subject | Scope | Locator |
|---|---|---|---|
| `EV-0001` | `CANDIDATE-0001` / `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681` | original Windows build, semantic checks, dependency census, PDF render, install/process-exit/uninstall lifecycle, install artifact | [`EV-0001-WINDOWS-CI.md`](EV-0001-WINDOWS-CI.md) |
| `EV-E001-01` | first post-Jester repair candidate `4a7c1d5b2bea939ebd508dcce19c4e93f58149cf` | Windows build/semantic checks, dependency boundary, install/PDF process-exit/uninstall lifecycle, package artifact; bounded source/state evidence for first repair semantics | [`EV-E001-01-WINDOWS-CI.md`](EV-E001-01-WINDOWS-CI.md) |
| `EV-E001-02` | second post-Jester repair candidate `1df3b56528ac04b4f0d0a043593365b575007b93` | Windows build/semantic checks, dependency boundary, strengthened installed PDF open/render success oracle, process-exit/uninstall lifecycle, package artifact; bounded source/state evidence for Open-transition authority repair | [`EV-E001-02-WINDOWS-CI.md`](EV-E001-02-WINDOWS-CI.md) |

`EV-0001` and `EV-E001-01` remain historical evidence for predecessor admitted Product Baselines. `EV-E001-02` is the evidence set supporting the exact second-repair candidate once admitted through its Product integration transition.

External execution routes are additionally bound by their workflow/job/artifact identities. Artifact retention is temporary; repository source/scripts plus these evidence records preserve the durable evidence route after artifact expiry.

Evidence does not itself admit the Product. Typed conformance conclusions for the current second-repair candidate are owned by `_mw/epochs/001-jester-finalization/results/VERIFICATION-E001-02.md`.
