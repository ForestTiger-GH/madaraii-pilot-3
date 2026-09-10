using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MiniDoc.Editor;

internal sealed record TableContext(Table Table, TableRowGroup Group, TableRow Row, TableCell Cell, int ColumnIndex);

internal static class TableEditor
{
    internal static Table InsertTable(FlowDocument document, TextPointer caret, int rows = 3, int columns = 3)
    {
        if (rows < 1 || rows > 50 || columns < 1 || columns > 20)
            throw new ArgumentOutOfRangeException(nameof(rows), "MiniDoc tables are limited to 1–50 rows and 1–20 columns per insertion.");
        if (caret.Paragraph?.Parent is TableCell)
            throw new InvalidOperationException("Nested tables are outside MiniDoc's editable subset.");

        var table = CreateTable(rows, columns);
        var anchor = caret.Paragraph;
        if (anchor is not null && anchor.Parent == document)
            document.Blocks.InsertAfter(anchor, table);
        else
            document.Blocks.Add(table);
        return table;
    }

    internal static TableContext? GetContext(TextPointer pointer)
    {
        var paragraph = pointer.Paragraph;
        if (paragraph?.Parent is not TableCell cell) return null;
        if (cell.Parent is not TableRow row || row.Parent is not TableRowGroup group || group.Parent is not Table table) return null;
        var cells = row.Cells.Cast<TableCell>().ToList();
        var column = cells.IndexOf(cell);
        return column < 0 ? null : new TableContext(table, group, row, cell, column);
    }

    internal static void AddRow(TableContext context)
    {
        var columns = context.Row.Cells.Count;
        if (columns < 1) throw new InvalidOperationException("The current table has no columns.");
        var row = new TableRow();
        for (var i = 0; i < columns; i++) row.Cells.Add(CreateCell());
        context.Group.Rows.Add(row);
    }

    internal static void DeleteCurrentRow(TableContext context, FlowDocument document)
    {
        if (context.Group.Rows.Count <= 1)
        {
            document.Blocks.Remove(context.Table);
            return;
        }
        context.Group.Rows.Remove(context.Row);
    }

    internal static void AddColumn(TableContext context)
    {
        foreach (var row in context.Group.Rows)
            row.Cells.Add(CreateCell());
    }

    internal static void DeleteCurrentColumn(TableContext context, FlowDocument document)
    {
        var width = context.Row.Cells.Count;
        if (width <= 1)
        {
            document.Blocks.Remove(context.Table);
            return;
        }

        foreach (var row in context.Group.Rows)
        {
            var cells = row.Cells.Cast<TableCell>().ToList();
            if (context.ColumnIndex >= cells.Count)
                throw new InvalidOperationException("The table is no longer rectangular. The operation was refused.");
            row.Cells.Remove(cells[context.ColumnIndex]);
        }
    }

    internal static void SetCurrentCellFill(TableContext context, Brush brush)
    {
        if (brush is not SolidColorBrush solid || solid.Color.A != 255)
            throw new InvalidOperationException("Cell fill must be an opaque solid RGB colour.");
        context.Cell.Background = brush;
    }

    internal static bool IsRectangular(Table table)
    {
        var widths = table.RowGroups.SelectMany(g => g.Rows.Cast<TableRow>()).Select(r => r.Cells.Count).Distinct().ToList();
        return widths.Count <= 1 && widths.FirstOrDefault() > 0;
    }

    private static Table CreateTable(int rows, int columns)
    {
        var table = new Table { CellSpacing = 0, Margin = new Thickness(0, 8, 0, 8) };
        for (var i = 0; i < columns; i++) table.Columns.Add(new TableColumn());
        var group = new TableRowGroup();
        table.RowGroups.Add(group);
        for (var r = 0; r < rows; r++)
        {
            var row = new TableRow();
            for (var c = 0; c < columns; c++) row.Cells.Add(CreateCell());
            group.Rows.Add(row);
        }
        return table;
    }

    internal static TableCell CreateCell() => new(new Paragraph(new Run(string.Empty)))
    {
        BorderBrush = new SolidColorBrush(Color.FromRgb(0xC8, 0xC8, 0xC8)),
        BorderThickness = new Thickness(0.75),
        Padding = new Thickness(5, 3, 5, 3)
    };
}
