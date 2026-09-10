# RT-003 — Build, installation, uninstall, and system cleanliness

**Question:** What build/install/uninstall model gives normal Windows installation while keeping owned artifacts explicit and removable?  
**Scope:** official build tooling, deployment shape, per-machine placement, shortcuts, uninstall registration, technical residue.  
**Non-goals:** Store distribution, enterprise MSI servicing, auto-update, file associations.

## Findings

The .NET SDK supports self-contained Windows publishing. In that mode the publish folder carries the target-specific executable, application files, and required .NET runtime, so a compatible runtime need not already exist on the target machine. A folder deployment is preferable to single-file packaging here because it keeps installed contents explicit and avoids adding extraction/runtime-bundling behavior merely for cosmetic file-count reduction.

A small project does not need MSI/MSIX to behave as an installed desktop program. A privileged PowerShell installer can perform a bounded, auditable per-machine installation:

1. copy the prepared publish payload to `%ProgramFiles%\MiniDoc`;
2. create an all-users Start Menu shortcut and desktop shortcut;
3. write one application-owned subkey under `HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\<ProductCode>` with display/install/uninstall metadata;
4. install an uninstaller script in the program directory whose `UninstallString` invokes PowerShell on that script.

Microsoft documents the Uninstall registry location and standard display metadata. .NET exposes the all-users Desktop/Programs special folders. Elevation can be requested through PowerShell `Start-Process -Verb RunAs`.

## Explicit ownership ledger

The installed product can have **zero application-owned runtime state**. It does not require preferences, recents, autosave, crash dumps, caches, telemetry, or updater state in v0.1. Its owned machine state is then finite:

- `%ProgramFiles%\MiniDoc\` publish payload plus installed uninstall script/manifest;
- one all-users desktop shortcut;
- one all-users Start Menu shortcut;
- one HKLM Uninstall registry subkey.

Ordinary user-selected `.docx`/`.pdf` files are outside application ownership. Files created by Save/Save As are user documents and survive uninstall.

Uninstall can delete the registry key and shortcuts, then remove the program directory. Because a running uninstall script cannot reliably delete its own directory before its process releases files, the launcher should copy the tiny uninstall script into a uniquely named file in the **same installed program directory**, invoke it? That still cannot delete the directory while the copied script runs. A cleaner design is for the uninstall command to execute a PowerShell command stored in the registry that removes all owned resources from outside the product directory, with the script text supplied as command arguments. No persistent external helper is required.

A practical alternative is an installed `uninstall.ps1` that, after deleting registry/shortcuts, spawns a short-lived elevated `cmd.exe`/PowerShell cleanup command to wait for the parent and remove the install directory. That creates no persistent helper file but does create a transient process. The human requirement rejects *background/persistent* helpers; a bounded uninstall child process is part of explicit uninstall execution. Its behavior must be documented and tested.

The strongest simple route is therefore: `UninstallString` calls the installed script; the script removes registry/shortcuts, then starts `cmd.exe /c` with a bounded delay/retry deleting `%ProgramFiles%\MiniDoc`, and exits. The child is visible only during uninstall and owns no persisted state.

## Build cleanliness

The .NET SDK itself can use NuGet/package and temp/cache locations while restoring Microsoft SDK/runtime packs. Those are build-tool effects, not Product runtime effects. The project can nevertheless make local builds cleaner by setting `DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH`, `TEMP`, and `TMP` to a repository-local `.build` directory for the build process and placing publish output under `artifacts/`. `build.ps1 -Clean` can remove project-local intermediate state.

A `PackageReference` census should be part of verification. The project file should contain no application package dependency.

## Update posture

v0.1 provides no updater. Re-running the installer with a newer prepared payload is the future update path: stop running MiniDoc first, replace the owned Program Files payload, refresh shortcuts/registry metadata, preserve all user documents. An automatic updater would add background/state complexity with no current driver.

## Residue verification

A Windows verification script can snapshot/check the exact owned locations before install, after install, after application use, and after uninstall. Runtime checks should also inspect `AppData`, `LocalAppData`, and Temp for paths/names attributable to MiniDoc. General Windows/.NET/OS activity outside app ownership cannot be promised absent; the product claim is that MiniDoc code intentionally creates no such application state.

## Sources

- Microsoft Learn, .NET publishing overview: https://learn.microsoft.com/en-us/dotnet/core/deploying/
- Microsoft Learn, `dotnet publish`: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish
- Microsoft Learn, Uninstall registry key: https://learn.microsoft.com/en-us/windows/win32/msi/uninstall-registry-key
- Microsoft Learn, `Environment.SpecialFolder`: https://learn.microsoft.com/en-us/dotnet/api/system.environment.specialfolder?view=net-10.0
- Microsoft Learn, PowerShell `Start-Process`: https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.management/start-process

**Evidence limit:** this result establishes a viable Windows installation model. Actual Installed Apps appearance, UAC behavior, shortcut placement, process cleanup, and residue state require execution on Windows.
