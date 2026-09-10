# RT-006 — Word-like desktop UX, search and replace

**Question:** what first-party Windows/WPF UI organization can provide a Word-like editing experience, including find/replace, without a third-party Ribbon or persistent UI state?  
**Scope:** Windows 11 x64 WPF shell; no visual clone requirement and no persistent personalization in the current lifecycle model.  
**Status:** Research Result.

## Findings

WPF includes the first-party `System.Windows.Controls.Ribbon` family. `Ribbon` is explicitly intended to organize application features into tabs and can host an application menu, Quick Access Toolbar, tab groups, groups and standard command controls. Contextual tabs are part of the Ribbon model. This is sufficient to implement a familiar Word-like information architecture without importing a third-party Ribbon framework.

The useful analogue is organizational rather than cosmetic: persistent top-level document commands; a Home tab for clipboard/font/paragraph/editing; an Insert tab for tables and future object commands; a View tab for PDF/view controls; contextual table controls when a table is active. The product should not copy Microsoft icons, artwork or branding.

`RichTextBox` provides normal selection and editing behavior and WPF routed editing commands cover Cut/Copy/Paste/Undo/Redo/SelectAll. The WPF flow-document overview explicitly notes that search is not supplied as a normal built-in `RichTextBox` feature. Find/replace should therefore be product code operating over `TextPointer`/text ranges while preserving the existing document tree rather than exporting/reimporting plain text.

A find implementation must account for WPF text symbols: element boundaries and embedded elements exist between text positions. A safe v1 search can enumerate text runs and map character offsets back to `TextPointer` positions. Replace should operate only on text ranges, never across non-text object/table structural boundaries. Replace All should process matches from the end toward the beginning or recompute positions to avoid invalid offsets.

Word-like customization has two meanings: contextual/adaptive command presentation and user-persistent personalization. The first is compatible with the current zero-state invariant. The second requires persisted user state if choices must survive restarts. Since `D-0004` forbids such state, v1 can provide Ribbon minimization, contextual controls, and a fixed Quick Access set during the session, but should not promise persistent customized QAT/Ribbon layout.

## Recommended engineering envelope

- Replace the compact toolbar/menu shell with a bounded WPF Ribbon organization.
- Use application menu / File surface for New, Open, Save, Save As, Exit.
- Use Quick Access for Save, Undo, Redo.
- Home: Clipboard, Font, Paragraph, Editing (Find/Replace).
- Insert: simple Table; future object commands can remain disabled/absent until their semantics are admitted.
- View: PDF page/zoom controls when PDF mode is active; document status remains separate.
- Contextual table controls may expose row/column/cell operations only when a supported table is selected.
- No persisted user UI customization in v1; this keeps zero technical state intact.

## Strongest challenge

A Ribbon can easily make a small application look larger than its actual capability. The control inventory must therefore be driven by admitted functions. Empty groups, disabled future features and copied Word chrome would violate the small-product architecture.

## Sources

- Microsoft WPF Ribbon namespace: https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.ribbon?view=windowsdesktop-10.0
- Microsoft WPF `Ribbon` class: https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.ribbon.ribbon?view=windowsdesktop-10.0
- Microsoft WPF Flow Document overview: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/flow-document-overview
- Microsoft Windows Ribbon tab organization reference: https://learn.microsoft.com/en-us/windows/win32/windowsribbon/windowsribbon-controls-tab

## Assessment

**Confidence:** high that the required Word-like organization is available first-party.  
**Does not establish:** pixel-level Word similarity, persistent personalization, or correctness of local find/replace implementation.  
**Reopen:** Ribbon is unavailable in the exact .NET 10 build, accessibility/keyboard behavior fails verification, or persistent customization becomes an explicit requirement.
