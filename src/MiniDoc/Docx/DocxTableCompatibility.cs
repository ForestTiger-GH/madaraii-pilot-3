using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static class DocxTableCompatibility
{
    internal static void Analyze(XElement table, XNamespace word, Action<string> add)
    {
        XElement? properties = null;
        XElement? grid = null;
        var rows = new List<XElement>();
        foreach (var child in table.Elements())
        {
            if (child.Name == word + "tblPr" && properties is null) properties = child;
            else if (child.Name == word + "tblGrid" && grid is null) grid = child;
            else if (child.Name == word + "tr") rows.Add(child);
            else add($"unsupported table element '{child.Name.LocalName}'");
        }

        if (properties is not null) AnalyzeProperties(properties, word, add);
        if (grid is not null)
        {
            foreach (var child in grid.Elements())
            {
                if (child.Name != word + "gridCol" || child.Attributes().Any(a => !a.IsNamespaceDeclaration))
                    add("table grid contains unsupported explicit column sizing or markup");
            }
        }

        if (rows.Count == 0) { add("editable tables require at least one row"); return; }
        int? expectedCells = null;
        foreach (var row in rows)
        {
            if (row.Elements(word + "trPr").Any()) add("row properties are outside the editable table subset");
            if (row.Elements().Any(e => e.Name != word + "trPr" && e.Name != word + "tc"))
                add("table row contains unsupported children");
            var cells = row.Elements(word + "tc").ToList();
            if (cells.Count == 0) add("editable table rows require at least one cell");
            expectedCells ??= cells.Count;
            if (expectedCells != cells.Count) add("table is not rectangular");
            foreach (var cell in cells) AnalyzeCell(cell, word, add);
        }

        if (grid is not null && expectedCells.HasValue && grid.Elements(word + "gridCol").Count() != expectedCells.Value)
            add("table grid column count does not match row cell count");
    }

    private static void AnalyzeProperties(XElement properties, XNamespace word, Action<string> add)
    {
        foreach (var property in properties.Elements())
        {
            if (property.Name == word + "tblW")
            {
                var type = DocxTextCompatibility.Value(property, word, "type") ?? "auto";
                var width = DocxTextCompatibility.Value(property, word, "w") ?? "0";
                if (type != "auto" || width != "0" || property.Attributes().Any(a => !a.IsNamespaceDeclaration && a.Name != word + "type" && a.Name != word + "w"))
                    add("table width exceeds MiniDoc's automatic-width subset");
            }
            else if (property.Name == word + "tblBorders") AnalyzeBorders(property, word, add);
            else add($"unsupported table property '{property.Name.LocalName}'");
        }
    }

    private static void AnalyzeBorders(XElement borders, XNamespace word, Action<string> add)
    {
        var allowedNames = new HashSet<string>(StringComparer.Ordinal) { "top", "left", "bottom", "right", "insideH", "insideV" };
        foreach (var border in borders.Elements())
        {
            if (!allowedNames.Contains(border.Name.LocalName)) { add($"unsupported table border '{border.Name.LocalName}'"); continue; }
            var value = DocxTextCompatibility.Value(border, word) ?? "single";
            if (value is not ("single" or "nil" or "none")) add("table border style exceeds the simple subset");
            foreach (var attribute in border.Attributes().Where(a => !a.IsNamespaceDeclaration))
            {
                if (attribute.Name == word + "val" || attribute.Name == word + "sz" || attribute.Name == word + "space" || attribute.Name == word + "color") continue;
                add("table border carries unsupported metadata");
            }
        }
    }

    private static void AnalyzeCell(XElement cell, XNamespace word, Action<string> add)
    {
        XElement? properties = null;
        var paragraphs = new List<XElement>();
        foreach (var child in cell.Elements())
        {
            if (child.Name == word + "tcPr" && properties is null) properties = child;
            else if (child.Name == word + "p") paragraphs.Add(child);
            else if (child.Name == word + "tbl") add("nested tables are outside the editable subset");
            else add($"unsupported table-cell element '{child.Name.LocalName}'");
        }

        if (properties is not null)
        {
            foreach (var property in properties.Elements())
            {
                if (property.Name == word + "shd") DocxTextCompatibility.AnalyzeShading(property, word, add, "cell");
                else add($"unsupported table-cell property '{property.Name.LocalName}'");
            }
        }
        if (paragraphs.Count == 0) add("editable table cells require paragraph content");
        foreach (var paragraph in paragraphs) DocxTextCompatibility.AnalyzeParagraph(paragraph, word, add);
    }
}
