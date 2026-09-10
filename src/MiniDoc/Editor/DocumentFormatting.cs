using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MiniDoc.Editor;

internal static class DocumentFormatting
{
    internal static void ApplyTextHighlight(RichTextBox editor, Brush brush)
    {
        EnsureOpaque(brush, "Text highlight");
        editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, brush);
    }

    internal static void ApplyParagraphFill(RichTextBox editor, Brush brush)
    {
        EnsureOpaque(brush, "Paragraph fill");
        foreach (var paragraph in SelectedParagraphs(editor)) paragraph.Background = brush;
    }

    internal static void ChangeIndent(RichTextBox editor, double deltaDip)
    {
        foreach (var paragraph in SelectedParagraphs(editor))
        {
            var margin = paragraph.Margin;
            paragraph.Margin = new Thickness(Math.Clamp(margin.Left + deltaDip, 0, 384), margin.Top,
                                             Math.Clamp(margin.Right, 0, 384), margin.Bottom);
        }
    }

    internal static void ChangeSpaceAfter(RichTextBox editor, double deltaDip)
    {
        foreach (var paragraph in SelectedParagraphs(editor))
        {
            var margin = paragraph.Margin;
            paragraph.Margin = new Thickness(margin.Left, margin.Top, margin.Right,
                                             Math.Clamp(margin.Bottom + deltaDip, 0, 192));
        }
    }

    internal static IReadOnlyList<Paragraph> SelectedParagraphs(RichTextBox editor)
    {
        var start = editor.Selection.Start;
        var end = editor.Selection.End;
        if (start.CompareTo(end) == 0 && start.Paragraph is not null)
            return [start.Paragraph];

        var result = new List<Paragraph>();
        foreach (var paragraph in EnumerateParagraphs(editor.Document.Blocks))
        {
            if (paragraph.ContentEnd.CompareTo(start) < 0 || paragraph.ContentStart.CompareTo(end) > 0) continue;
            result.Add(paragraph);
        }
        return result;
    }

    private static IEnumerable<Paragraph> EnumerateParagraphs(BlockCollection blocks)
    {
        foreach (var block in blocks)
        {
            if (block is Paragraph paragraph) yield return paragraph;
            else if (block is Table table)
                foreach (var group in table.RowGroups)
                foreach (var row in group.Rows)
                foreach (var cell in row.Cells)
                foreach (var nested in EnumerateParagraphs(cell.Blocks))
                    yield return nested;
        }
    }

    private static void EnsureOpaque(Brush brush, string name)
    {
        if (brush is not SolidColorBrush solid || solid.Color.A != 255)
            throw new InvalidOperationException($"{name} must be an opaque solid RGB colour.");
    }
}
