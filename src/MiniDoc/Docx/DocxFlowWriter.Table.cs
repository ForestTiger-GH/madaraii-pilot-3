using System.Globalization;
using System.Windows.Documents;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static partial class DocxFlowWriter
{
    private static XElement SerializeTable(Table table, XNamespace word)
    {
        var rows = table.RowGroups.Cast<TableRowGroup>().SelectMany(g => g.Rows.Cast<TableRow>()).ToList();
        if (rows.Count == 0) throw new InvalidOperationException("An editable table must contain at least one row.");
        var columns = rows[0].Cells.Count;
        if (columns == 0 || rows.Any(r => r.Cells.Count != columns))
            throw new InvalidOperationException("MiniDoc can save only rectangular non-empty tables.");

        var source = table.Tag as TableSource;
        var xml = new XElement(word + "tbl");
        if (source is not null) foreach (var attribute in source.Attributes) xml.Add(new XAttribute(attribute));
        xml.Add(source?.Properties is null ? BuildDefaultTableProperties(word) : new XElement(source.Properties));

        var grid = new XElement(word + "tblGrid");
        for (var i = 0; i < columns; i++) grid.Add(new XElement(word + "gridCol"));
        xml.Add(grid);

        foreach (var row in rows)
        {
            var rowSource = row.Tag as TableRowSource;
            var rowXml = new XElement(word + "tr");
            if (rowSource is not null) foreach (var attribute in rowSource.Attributes) rowXml.Add(new XAttribute(attribute));
            foreach (var cell in row.Cells) rowXml.Add(SerializeCell(cell, word));
            xml.Add(rowXml);
        }
        return xml;
    }

    private static XElement SerializeCell(TableCell cell, XNamespace word)
    {
        var source = cell.Tag as TableCellSource;
        var xml = new XElement(word + "tc");
        if (source is not null) foreach (var attribute in source.Attributes) xml.Add(new XAttribute(attribute));
        var properties = source?.Properties is null ? new XElement(word + "tcPr") : new XElement(source.Properties);
        properties.Elements(word + "shd").Remove();
        AddDirectShading(properties, cell.Background, word);
        if (properties.HasElements || properties.HasAttributes) xml.Add(properties);

        foreach (var block in cell.Blocks)
        {
            if (block is not Paragraph paragraph)
                throw new InvalidOperationException($"Table cell contains unsupported block '{block.GetType().Name}'. Save was refused.");
            xml.Add(SerializeParagraph(paragraph, word));
        }
        if (!xml.Elements(word + "p").Any()) xml.Add(SerializeParagraph(new Paragraph(), word));
        return xml;
    }

    private static XElement BuildDefaultTableProperties(XNamespace word)
    {
        var properties = new XElement(word + "tblPr",
            new XElement(word + "tblW", new XAttribute(word + "w", "0"), new XAttribute(word + "type", "auto")));
        var borders = new XElement(word + "tblBorders");
        foreach (var name in new[] { "top", "left", "bottom", "right", "insideH", "insideV" })
            borders.Add(new XElement(word + name,
                new XAttribute(word + "val", "single"),
                new XAttribute(word + "sz", "4"),
                new XAttribute(word + "space", "0"),
                new XAttribute(word + "color", "C8C8C8")));
        properties.Add(borders);
        return properties;
    }
}
