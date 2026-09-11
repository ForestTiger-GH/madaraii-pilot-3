# VERIFICATION-0001 — MiniDoc 0.1 Implementation Verification

**Work kind:** `IMPLEMENTATION_VERIFICATION` / MADARAII-32  
**Verification Subject:** unchanged `CANDIDATE-0001` = `b2cafabba970245b7ccb24f6f01a3ca2e2ba9681`  
**Baselines:** `WORK-0001`; `PLAN-0001` revision 2; `TARGET-WHAT-0001` revision 2; `TARGET-HOW-0001` revision 2; `PA-0001` revision 2  
**Primary evidence:** `EV-0001`  
**Verification cutoff:** 2026-09-11

## Independence and method

Expected behavior and protected non-goals were reconstructed from the admitted Target/Plan before classifying the implementation observations. Verification uses exact-source inspection plus an isolated GitHub-hosted Windows execution of the exact candidate. The workflow and semantic test harness were authored inside the same development contour, so common-mode test/oracle bias remains possible; the disposable Windows runner provides execution isolation, not fully independent test authorship.

No Product/source repair is performed in this Verification Result. All compile/test defects discovered in earlier runs were repaired before the frozen candidate and therefore belong to implementation history, not this verdict.

## Typed claim conclusions

| Claim | Criteria / evidence | Verdict |
|---|---|---|
| Product builds as .NET 10 WPF `win-x64` candidate | exact candidate Release build on Windows; 0 warnings / 0 errors | `CONFORMS` |
| Product project contains no third-party/NuGet runtime package dependency | csproj check plus `dotnet list ... --include-transitive` returned no packages | `CONFORMS` |
| Supported search/replace can operate across adjacent runs without flattening structural lanes | `MiniDoc.Checks` representative search/replace fixture | `CONFORMS` for tested behavior |
| Supported paragraph/direct formatting + simple rectangular table can serialize to DOCX and re-open with editable authority | generated fixture → SaveToBytes → reclassification → table/text assertions | `CONFORMS` for tested supported fixture |
| Representative unsupported drawing does not gain editable/save authority | mutated DOCX drawing fixture re-opened read-only with compatibility message | `CONFORMS` for representative drawing case |
| PDF can be locally loaded/rendered through first-party Windows PDF API | real one-page PDF fixture produced non-empty rendered bitmap | `CONFORMS` on tested Windows runner |
| Self-contained install package is produced | publish/package build plus uploaded artifact `10178912526` | `CONFORMS` |
| Declared per-machine owner state can be installed and removed | `verify-windows.ps1` asserted Program Files, shortcuts, HKLM owner, then uninstall cleanup | `CONFORMS` on tested Windows runner |
| Normal close after installed PDF use terminates MiniDoc process | installed Product opened fixture through normal PDF shell path, awaited render, invoked normal `Close()`, process-exit assertion passed | `CONFORMS` on tested Windows runner |
| Uninstall preserves unowned user documents | sentinel created outside installer ownership survived uninstall | `CONFORMS` on tested Windows runner |
| Covered MiniDoc AppData/Temp residue is absent after lifecycle test | verification-script residue assertions passed | `CONFORMS` within the script's declared observation surface |
| Native shapes/charts/SmartArt editing, footnote authoring, TOC/field recalculation | explicitly outside Target 0.1, compatibility/non-goal path only | `CONFORMS` as non-goal boundary; no native-function claim exists |
| Windows 11 x64 end-user qualification and Ribbon visual/usability acceptance | CI runner was Windows Server 2025 10.0.26100; no interactive Windows 11 human visual acceptance was executed | `EVIDENCE_GAP` |
| Exhaustive compatibility for all supported/unsupported DOCX variants and broad PDF population | only representative fixtures and source/static challenge were executed | `EVIDENCE_GAP` for exhaustive/generalized claim; no such universal compatibility is admitted |
| All named compatibility classes (footnotes/endnotes/fields/TOC/chart/SmartArt/shape preview variants) have dedicated dynamic fixtures | current dynamic negative fixture covers drawing; other classes rely on bounded source inspection/compatibility contract | `EVIDENCE_GAP` for class-by-class dynamic coverage |
| Crash/power-failure atomic overwrite | Target explicitly excludes this guarantee | outside verification claim / accepted limitation |

No `IMPLEMENTATION_DEFECT`, `DESIGN_GAP`, `STALE_DESIGN_OR_PLAN`, `SCOPE_OR_AUTHORITY_GAP`, `MIGRATION_INCOMPLETE`, or `BLOCKED_BY_EXPLICIT_DEPENDENCY` finding remains for the implemented commissioned core.

## Expected-to-actual delta and protected behavior

The exact candidate contains the planned Ribbon shell, editor operations, enlarged DOCX subset, PDF/storage mechanisms, build/install/uninstall scripts, documentation and verification assets. The second-inbox scope was reconciled before implementation rather than smuggled into local choices.

Protected boundaries remain intact in observed/source state:

- no Product `PackageReference`;
- PDF remains view-only;
- unsupported representative drawing remains fail-closed;
- no service/updater/background resident mechanism is installed;
- explicit user document remains outside uninstall ownership;
- native Word object/field authoring remains outside the release rather than being silently flattened into editable semantics.

## Evidence limitations and strongest plausible common modes

Material residual common modes are concentrated in format breadth and UI/environment representativeness: a small generated DOCX fixture cannot prove every Word-produced document within the nominal subset; a Windows Server 2025 runner cannot prove Windows 11 desktop visual behavior; tests authored during the same contour may share assumptions with implementation. These are bounded as reliance limits instead of being converted into universal PASS claims.

A user-local Windows 11 acceptance run should therefore execute the procedure in `docs/BUILD-INSTALL-VERIFY.md`, inspect the Ribbon interactively, and exercise representative real DOCX/PDF documents before broad personal reliance.

## Supported admission envelope

Verification supports admission of `CANDIDATE-0001` as the current repository Product implementation for MiniDoc 0.1, with the following boundary:

- build, representative document semantics, PDF rendering, dependency boundary and tested install/process/uninstall mechanisms are evidence-backed;
- Product is not claimed released/deployed to a user machine;
- Windows 11 interactive qualification and broader real-document compatibility remain explicit evidence gaps;
- future Product-source/install/test changes invalidate this verdict and require re-verification of affected claims.

The unchanged candidate may proceed to MADARAII-33 within this envelope.
