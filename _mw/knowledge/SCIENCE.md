# SCIENCE-0001 — Current Scientific Knowledge

**Status:** admitted for `WORK-0001` Target formation  
**Baseline:** `RP-0001` research corpus, cutoff 2026-09-11  
**Consumers:** Target WHAT/HOW formation, implementation planning, verification challenge

This maintained owner assimilates the completed Research Results. Research history remains under `_mw/research/` for source challenge; ordinary Target formation can rely on this owner without rereading it.

## S1 — DOCX is a package with several independent semantic surfaces

Open XML word-processing documents are package graphs made of parts and relationships. The main story commonly uses `document/body/p/r/t`, while headers, footers, comments, settings, styles, footnotes, endnotes, and other stories/parts can carry independent meaning. Therefore “read visible text and write a new DOCX” is not a safe round trip for arbitrary documents.

**Grounds:** ECMA-376; Microsoft WordprocessingML/package documentation.  
**Engineering implication:** compatibility admission must precede editable mode. Unowned package parts can be retained; unsupported semantics in the main story must block edited saving.

## S2 — A conservative editable subset is materially safer than partial arbitrary parsing

For a small editor, the defensible model is feature recognition plus refusal. A Transitional main story restricted to paragraphs/runs and explicitly supported direct formatting can be mapped to a rich-text model. Unknown material main-story nodes, styles-based semantics, tables, drawings, fields, change tracking, content controls, equations, hyperlinks, list semantics, signatures, Strict markup, or encrypted packages belong to read-only/unsupported routes unless separately implemented.

**Grounds:** ECMA-376’s broad vocabulary and markup-compatibility model; package/story inventory; `RT-001`.  
**Challenge:** this rejects many ordinary Word documents. That is an intentional compatibility limit, preferable to silent data loss in the commissioned v0.1.

## S3 — In-memory package transformation supports the cleanliness invariant

.NET’s base compression/XML/file APIs are sufficient to read ZIP entries, parse a bounded XML subset, rebuild only the owned main part, and generate an output package in memory. `ZipArchiveMode.Update` holds update content in memory until archive disposal. No application temp/cache file is inherently required by this mechanism.

**Grounds:** Microsoft .NET API documentation; `RT-001`.  
**Limit:** an in-memory package can consume substantial memory, so bounded package/entry/XML size limits are required.

## S4 — WPF supplies the small rich-text editing surface

WPF `RichTextBox` supports formatted flow content and standard editing/formatting commands. It can represent the selected paragraph/run/direct-formatting subset while allowing the product to block richer clipboard/FlowDocument structures from entering its save model.

**Grounds:** Microsoft WPF documentation; `RT-002`, `RT-004`.  
**Limit:** WPF’s visual/editor model is not WordprocessingML and does not prove Word layout fidelity.

## S5 — Windows supplies a first-party PDF-to-image rendering path

`Windows.Data.Pdf` loads PDFs, exposes pages, and renders a page to an in-memory random-access stream. This is enough for page-at-a-time view-only behavior. A 400% zoom ceiling stays within Microsoft’s straightforward C#/XAML rendering guidance.

**Grounds:** Microsoft Windows API documentation; `RT-002`.  
**Limit:** password-protected behavior and broad malformed-PDF compatibility are outside the v0.1 promise unless tested/implemented.

## S6 — PDF rendering has a material shutdown uncertainty in WPF

An open Microsoft/CsWinRT issue reports historical WPF process survival after `PdfPage.RenderToStreamAsync`. Its configuration differs from the target and it does not establish a current defect, but it defeats any assumption that normal disposal alone proves process termination.

**Grounds:** microsoft/CsWinRT issue #1249; `RT-002`.  
**Engineering implication:** explicit process-exit containment plus Windows-local process verification are required before claiming “closed means closed.”

## S7 — First-party-only deployment is feasible with modern .NET and Windows

A Windows-specific .NET 10 target can consume WinRT APIs directly and WPF is part of Windows Desktop .NET. A self-contained `win-x64` publish carries the required .NET runtime. No application `PackageReference` is required for the selected implementation.

**Grounds:** Microsoft modern desktop WinRT and .NET deployment documentation; `RT-002`, `RT-003`.  
**Validity:** target Windows 11 x64. Current Microsoft support documentation lists .NET 10 support on Windows 11 current servicing releases.

## S8 — Normal installation can have a finite auditable ownership surface

A per-machine Program Files payload, all-users shortcuts, and one HKLM Uninstall registration create a recognizable installed desktop application without an always-running helper. v0.1 can carry zero application-owned runtime data state. Uninstall can target only these declared resources and leave all user documents untouched.

**Grounds:** Microsoft uninstall registry/special-folder/PowerShell documentation; `RT-003`.  
**Limit:** appearance in Windows UI, UAC path, shortcut behavior, and residue are runtime system claims and need Windows execution evidence.

## S9 — Cleanliness and crash-atomic saving are competing concerns in a zero-temp design

A memory-staged direct replacement avoids temp artifacts, yet ordinary overwrite semantics truncate an existing target. Best-effort in-memory restoration can cover ordinary exceptions after process survival, while power/process failure during the final write remains non-atomic. A same-directory temporary/replace scheme would strengthen crash durability but would create a filesystem artifact and possible crash residue.

**Grounds:** .NET file semantics; `RT-001`, `RT-004`.  
**Engineering implication:** choose and disclose the trade-off explicitly. The v0.1 human priority strongly favors controlled zero-temp behavior; Save As is the safer route for high-value originals.

## Source registry

| ID | Source | Role |
|---|---|---|
| `SRC-ECMA-376` | https://ecma-international.org/publications-and-standards/standards/ecma-376/ | normative format architecture |
| `SRC-MS-WML` | https://learn.microsoft.com/en-us/office/open-xml/word/structure-of-a-wordprocessingml-document | package/story/main markup orientation |
| `SRC-MS-WPF` | https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/richtextbox-overview | editor capability |
| `SRC-MS-WINRT-DESKTOP` | https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/winrt-apis-desktop-apps | WinRT access from modern .NET |
| `SRC-MS-PDF` | https://learn.microsoft.com/en-us/uwp/api/windows.data.pdf?view=winrt-26100 | PDF API capability |
| `SRC-CSWINRT-1249` | https://github.com/microsoft/CsWinRT/issues/1249 | negative lifecycle evidence |
| `SRC-MS-DOTNET-DEPLOY` | https://learn.microsoft.com/en-us/dotnet/core/deploying/ | deployment capability |
| `SRC-MS-DOTNET-WINDOWS` | https://learn.microsoft.com/en-us/dotnet/core/install/windows | supported OS/current runtime context |
| `SRC-MS-UNINSTALL` | https://learn.microsoft.com/en-us/windows/win32/msi/uninstall-registry-key | uninstall registry convention |

## Completeness and freshness

The corpus is complete only for the declared v0.1 decisions. It makes no claim of full DOCX/PDF/Windows deployment coverage. Raw Research can be amputated from ordinary Target use because all material conclusions, challenges, limitations, and source routes required by downstream work are present here.

Reopen on a target OS/runtime change, a format-support expansion, contradictory Windows-local evidence, or new first-party API lifecycle evidence material to a protected claim.
