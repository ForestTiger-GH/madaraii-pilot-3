# RT-002 — Windows-native UI, PDF, and dependency boundary

**Question:** Which Windows-provided UI and PDF capabilities can satisfy the product without third-party runtime components, and what lifecycle risks exist?  
**Scope:** Windows desktop UI, rich-text editing surface, PDF page rendering, WinRT access from modern .NET, process lifecycle.  
**Non-goals:** cross-platform UI, PDF editing, custom PDF renderer, Windows App SDK adoption.

## Findings

WPF supplies a Windows desktop UI and a `RichTextBox` backed by `FlowDocument`. Microsoft documents formatted-text editing, standard cut/copy/paste behavior, undo/redo, and editing commands including bold, italic, underline, and paragraph/list operations. That is sufficient as the visual/editor surface for the selected small DOCX subset without adopting a third-party UI framework.

Modern .NET desktop projects can call Windows Runtime APIs by selecting a Windows-version-specific target framework moniker. Microsoft’s current guidance shows .NET 10 TFMs such as `net10.0-windows10.0.26100.0`/`22621.0`/`19041.0`. This avoids the legacy need to add a Windows SDK contracts NuGet package solely for WinRT projection.

Windows provides `Windows.Data.Pdf`. Its `PdfDocument` exposes page count/load operations; `PdfPage.RenderToStreamAsync` renders a page into a stream. `PdfPage.Size` is expressed in device-independent pixels. Microsoft recommends the stream-render path for C#/XAML scenarios and directs zoom above 400% toward a COM rendering route, so a 400% v0.1 zoom ceiling keeps the viewer inside the straightforward first-party API.

A PDF page can therefore be rendered one at a time into a Windows in-memory random-access stream, copied into an in-memory WPF bitmap, and displayed without writing a page cache to disk. The input file can be resolved as a `StorageFile` from its absolute path.

## Material lifecycle risk

Microsoft’s `microsoft/CsWinRT` repository has an open bug report (#1249, “PdfDocument spawns zombie process”) describing a WPF process remaining after `PdfPage.RenderToStreamAsync` and application shutdown in a .NET 6 / Windows 10 reproduction. The report is old and does not establish current .NET 10/Windows 11 behavior, but its open state means the project cannot infer the required “closed means closed” guarantee from nominal API disposal alone.

The design should therefore:

- dispose each `PdfPage` immediately after rendering;
- hold no disk cache and no background worker/service;
- release the document/session when switching files;
- after the main window has passed unsaved-document closing logic and is definitively closed, explicitly terminate the process (`Environment.Exit(0)`) as a containment measure;
- require a Windows-local smoke test that renders a PDF, closes the window, and confirms no `MiniDoc` process remains.

The explicit exit is a product design mitigation. Only the Windows-local process test can establish the actual guarantee for the shipped configuration.

## Dependency boundary

Permitted runtime/product code can consist of:

- project-owned C# and XAML;
- .NET 10 Windows Desktop/WPF assemblies supplied by Microsoft as part of a self-contained publish;
- Windows Runtime / OS APIs supplied by Windows, including `Windows.Data.Pdf` and `Windows.Storage`;
- .NET base libraries including compression/XML/filesystem APIs.

Excluded: Open XML SDK, PDFium, WebView-based PDF engines, Windows App SDK packages, third-party UI toolkits, NuGet application packages, browser helpers, Office automation, and any externally installed document engine.

## Assessment

A first-party-only implementation is feasible. WPF is the practical rich-text UI substrate; `Windows.Data.Pdf` is the practical first-party PDF renderer. The PDF lifecycle bug creates a verification obligation and supports explicit process-exit containment. It does not justify adding a third-party PDF engine.

## Sources

- Microsoft Learn, WPF `RichTextBox` overview: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/richtextbox-overview
- Microsoft Learn, calling Windows Runtime APIs in desktop apps: https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/winrt-apis-desktop-apps
- Microsoft Learn, `Windows.Data.Pdf` namespace: https://learn.microsoft.com/en-us/uwp/api/windows.data.pdf?view=winrt-26100
- Microsoft Learn, `PdfDocument`: https://learn.microsoft.com/en-us/uwp/api/windows.data.pdf.pdfdocument?view=winrt-26100
- Microsoft Learn, `PdfPage`: https://learn.microsoft.com/en-us/uwp/api/windows.data.pdf.pdfpage?view=winrt-26100
- Microsoft Learn, `StorageFile.GetFileFromPathAsync`: https://learn.microsoft.com/en-us/uwp/api/windows.storage.storagefile.getfilefrompathasync?view=winrt-28000
- Microsoft/CsWinRT issue #1249: https://github.com/microsoft/CsWinRT/issues/1249

**Evidence limit:** the open issue is negative risk evidence from a historical configuration. It is not proof that the bug reproduces on the target Windows 11/.NET 10 configuration.
