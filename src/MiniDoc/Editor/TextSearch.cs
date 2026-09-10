using System.Text;
using System.Windows.Documents;

namespace MiniDoc.Editor;

internal sealed record TextMatch(TextPointer Start, TextPointer End, string Text);

internal static class TextSearch
{
    internal static IReadOnlyList<TextMatch> FindAll(FlowDocument document, string query, bool caseSensitive)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (string.IsNullOrEmpty(query)) return Array.Empty<TextMatch>();

        var comparison = caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
        var matches = new List<TextMatch>();
        foreach (var paragraph in EnumerateParagraphs(document.Blocks))
        {
            foreach (var segment in BuildSegments(paragraph))
            {
                var offset = 0;
                while (offset <= segment.Text.Length - query.Length)
                {
                    var found = segment.Text.IndexOf(query, offset, comparison);
                    if (found < 0) break;
                    var endIndex = found + query.Length - 1;
                    matches.Add(new TextMatch(segment.Starts[found], segment.Ends[endIndex], segment.Text.Substring(found, query.Length)));
                    offset = found + Math.Max(1, query.Length);
                }
            }
        }
        return matches;
    }

    internal static TextMatch? FindNext(FlowDocument document, string query, bool caseSensitive, TextPointer from)
    {
        var matches = FindAll(document, query, caseSensitive);
        if (matches.Count == 0) return null;
        foreach (var match in matches)
            if (match.Start.CompareTo(from) >= 0) return match;
        return matches[0];
    }

    internal static int ReplaceAll(FlowDocument document, string query, string replacement, bool caseSensitive)
    {
        var matches = FindAll(document, query, caseSensitive);
        for (var i = matches.Count - 1; i >= 0; i--)
            new TextRange(matches[i].Start, matches[i].End).Text = replacement;
        return matches.Count;
    }

    private static IEnumerable<Paragraph> EnumerateParagraphs(BlockCollection blocks)
    {
        foreach (var block in blocks)
        {
            if (block is Paragraph paragraph)
            {
                yield return paragraph;
                continue;
            }

            if (block is Table table)
            {
                foreach (var rowGroup in table.RowGroups)
                foreach (var row in rowGroup.Rows)
                foreach (var cell in row.Cells)
                foreach (var nested in EnumerateParagraphs(cell.Blocks))
                    yield return nested;
            }
        }
    }

    private static IEnumerable<TextSegment> BuildSegments(Paragraph paragraph)
    {
        var text = new StringBuilder();
        var starts = new List<TextPointer>();
        var ends = new List<TextPointer>();
        var cursor = paragraph.ContentStart;

        TextSegment? Flush()
        {
            if (text.Length == 0) return null;
            var segment = new TextSegment(text.ToString(), starts.ToArray(), ends.ToArray());
            text.Clear(); starts.Clear(); ends.Clear();
            return segment;
        }

        while (cursor.CompareTo(paragraph.ContentEnd) < 0)
        {
            var context = cursor.GetPointerContext(System.Windows.LogicalDirection.Forward);
            if (context == TextPointerContext.Text)
            {
                var runText = cursor.GetTextInRun(System.Windows.LogicalDirection.Forward);
                for (var i = 0; i < runText.Length; i++)
                {
                    var start = cursor.GetPositionAtOffset(i, System.Windows.LogicalDirection.Forward);
                    var end = cursor.GetPositionAtOffset(i + 1, System.Windows.LogicalDirection.Forward);
                    if (start is null || end is null) break;
                    text.Append(runText[i]);
                    starts.Add(start);
                    ends.Add(end);
                }
                cursor = cursor.GetPositionAtOffset(runText.Length, System.Windows.LogicalDirection.Forward)
                         ?? cursor.GetNextContextPosition(System.Windows.LogicalDirection.Forward)
                         ?? paragraph.ContentEnd;
                continue;
            }

            if (context == TextPointerContext.ElementStart &&
                cursor.GetAdjacentElement(System.Windows.LogicalDirection.Forward) is LineBreak)
            {
                var segment = Flush();
                if (segment is not null) yield return segment;
            }

            cursor = cursor.GetNextContextPosition(System.Windows.LogicalDirection.Forward) ?? paragraph.ContentEnd;
        }

        var last = Flush();
        if (last is not null) yield return last;
    }

    private sealed record TextSegment(string Text, TextPointer[] Starts, TextPointer[] Ends);
}
