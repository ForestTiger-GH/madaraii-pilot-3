# Questions

## Post-integration disposition

| Question | Disposition |
|---|---|
| Q-0001 Windows-native/official stack | resolved by `SCIENCE-0001`, D-0002/D-0007 and admitted `PRODUCT-0001` |
| Q-0002 safe DOCX subset/save model | resolved for v0.1 by `SCIENCE-0001`, Target revision 2 and representative semantic evidence in `EV-0001` |
| Q-0003 viable PDF subset | resolved for v0.1; real Windows PDF rendering and normal-close process termination conform on CI runner in `VERIFICATION-0001` |
| Q-0004 install/uninstall cleanliness | resolved for tested Windows runner; declared owner lifecycle, cleanup and user-sentinel preservation conform in `EV-0001` |
| Q-0005 verification split | resolved by `PLAN-0001` and typed `VERIFICATION-0001` |
| Q-0006 Windows 11 interactive/visual acceptance | terminal verification limitation for this contour, not an implementation blocker; local procedure exists in `docs/BUILD-INSTALL-VERIFY.md`; reopen only on failed or required Windows 11 qualification |
| Q-0007 broad real-document compatibility | terminal bounded uncertainty; Product promises a conservative subset and explicit compatibility mode, not universal DOCX/PDF support; reopen only when new format breadth is commissioned or evidence contradicts current boundary |

No human-blocking, implementation-blocking or integration-blocking Question remains open. `Q-0006`/`Q-0007` are explicit reliance limits with reopen triggers rather than hidden successor Work.
