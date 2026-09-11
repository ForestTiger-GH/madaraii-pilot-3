# EV-0001 — Windows CI and lifecycle evidence

**Evidence Subject:** `CANDIDATE-0001`  
**Candidate commit:** `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`  
**Workflow run:** `34545614992` — https://github.com/ForestTiger-GH/madaraii-pilot-3/actions/runs/34545614992  
**Job:** `103097451522` (`build-verify-lifecycle`)  
**Execution date:** 2026-09-11  
**Conclusion:** workflow/job `success`

## Exact environment

- GitHub-hosted Windows runner
- OS: Microsoft Windows Server 2025 Datacenter, `10.0.26100`
- runner image: `windows-2025-vs2026`
- image version: `20260907.229.1`
- .NET SDK: `10.0.401`
- .NET runtime installed by setup: `10.0.12`
- Product target: `net10.0-windows10.0.19041.0`, `win-x64`, self-contained

This environment is Windows execution evidence but is not identical to the commissioned Windows 11 x64 end-user environment.

## Observations

### Build and semantic checks

`./scripts/build.ps1 -Clean` completed successfully against the exact candidate.

Observed log facts:

- `Build succeeded.`
- `0 Warning(s)`
- `0 Error(s)`
- `MiniDoc.Checks: PASS`
- self-contained Product publish completed into `artifacts/publish`
- install source assembled at `artifacts/MiniDoc-0.1.0`

`MiniDoc.Checks` exercises:

- case-insensitive search spanning adjacent runs and text replacement;
- supported direct formatting plus simple rectangular table creation → DOCX serialization → MiniDoc re-open as editable → text/table survival;
- representative unsupported drawing markup remains read-only and receives no save authority;
- actual `Windows.Data.Pdf` load and page render for `tests/fixtures/sample.pdf`, requiring a non-empty bitmap.

### Product dependency boundary

The workflow checks `src/MiniDoc/MiniDoc.csproj` for a Product `PackageReference`, then executes:

```text
dotnet list src/MiniDoc/MiniDoc.csproj package --include-transitive
```

Observed output for `net10.0-windows10.0.19041.0`:

```text
No packages were found for this framework.
```

This supports the claim that the Product project carries no NuGet package dependency. Official .NET/Windows platform components and build tooling remain platform/build dependencies as declared by Target HOW.

### Installed lifecycle and process exit

`./scripts/verify-windows.ps1` completed with:

```text
MiniDoc 0.1.0 installed in C:\Program Files\MiniDoc
MiniDoc was uninstalled. User documents were not enumerated or removed.
MiniDoc Windows lifecycle verification: PASS
```

The verification script, on the exact packaged Product:

1. required a clean pre-install owner state;
2. installed into `%ProgramFiles%\MiniDoc`;
3. asserted the installed executable, all-users Desktop/Start Menu shortcuts and HKLM Uninstall owner;
4. asserted absence of MiniDoc AppData owner state;
5. created an unowned user-document sentinel;
6. started the installed Product with `tests/fixtures/sample.pdf` through `--verify-close-after-open`, which opens through the normal PDF shell path, awaits PDF render, and invokes normal main-window `Close()`;
7. waited for process termination and failed if MiniDoc remained resident;
8. uninstalled the Product;
9. asserted removal of declared installer-owned paths/registry state;
10. asserted preservation of the user sentinel and absence of covered MiniDoc-named AppData/Temp residue.

The passing step therefore supports the tested lifecycle/process-exit and bounded residue claims on this runner configuration.

## Install-package artifact

- Artifact ID: `10178912526`
- Name: `MiniDoc-0.1.0-win-x64`
- Workflow locator: https://github.com/ForestTiger-GH/madaraii-pilot-3/actions/runs/34545614992/artifacts/10178912526
- Artifact ZIP size: `71,593,323` bytes
- Uploaded file count reported by workflow: `405`
- GitHub artifact digest: `sha256:54c235f7e1b8cab23ab615e930128ed6c18873be7076ec8427e03aad83f86f73`
- Expiry reported by GitHub: 2026-12-10

The artifact is a CI output for the exact candidate. Artifact retention is time-bounded; source/build reproducibility in the repository is the durable route after expiry.

## Evidence limits

This evidence does not establish:

- visual/usability acceptance on an interactive Windows 11 desktop;
- pixel/layout parity with Microsoft Word;
- exhaustive compatibility across arbitrary DOCX/PDF populations;
- dynamic tests for every compatibility class named in Target WHAT, including every footnote/field/chart/SmartArt variant;
- password-protected PDF behavior beyond source-level explicit unsupported handling;
- crash/power-failure atomicity of direct DOCX overwrite, which Target WHAT explicitly excludes.

These limits bound reliance; they do not negate the specific observed claims above.
