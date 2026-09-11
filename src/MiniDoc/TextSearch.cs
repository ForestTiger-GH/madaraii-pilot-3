using System.Windows.Documents;

namespace MiniDoc;

internal static class TextSearch
{
    internal static MiniDoc.Editor.TextMatch? FindNext(FlowDocument document, string query, bool caseSensitive, TextPointer from) =>
        MiniDoc.Editor.TextSearch.FindNext(document, query, caseSensitive, from);

    internal static int ReplaceAll(FlowDocument document, string query, string replacement, bool caseSensitive) =>
        MiniDoc.Editor.TextSearch.ReplaceAll(document, query, replacement, caseSensitive);
}
