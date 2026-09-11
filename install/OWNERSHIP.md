# MiniDoc installation ownership

MiniDoc 0.1 owns only the following machine-level technical state after installation:

1. `%ProgramFiles%\MiniDoc\` — the complete application payload, `uninstall.ps1`, and this ownership document.
2. `%Public%\Desktop\MiniDoc.lnk` — all-users desktop shortcut.
3. `%ProgramData%\Microsoft\Windows\Start Menu\Programs\MiniDoc.lnk` — all-users Start Menu shortcut.
4. `HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\MiniDoc` — uninstall registration.

The installer creates no file association, protocol handler, service, scheduled task, Run/RunOnce entry, updater, AppData directory, cache, telemetry/log location, or user-document directory.

User-created or user-selected `.docx`, `.pdf` and other documents are outside installation ownership regardless of whether MiniDoc opened or saved them. Uninstall must never enumerate document history because MiniDoc keeps no recent-file database.
