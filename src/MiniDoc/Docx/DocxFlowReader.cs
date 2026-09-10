using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static class DocxFlowReader
{
    private static readonly Dictionary<string, Color> HighlightColors = new(StringComparer.Ordinal)
    {
        ["black"] = Colors.Black, ["blue"] = Colors.Blue, ["cyan"] = Colors.Cyan, ["green"] = Colors.Lime,
        ["magenta"] = Colors.Magenta, ["red"] = Colors.Red, ["yellow"] = Colors.Yellow, ["white"] = Colors.White,
        ["darkBlue"] = Colors.DarkBlue, ["darkCyan"] = Colors.DarkCyan, ["darkGreen"] = Colors.DarkGreen,
        ["darkMagenta"] = Colors.DarkMagenta, ["darkRed"] = Colors.DarkRed, ["darkYellow"] = Colors.Olive,
        ["darkGray"] = Colors.DarkGray, ["lightGray"] = Colors.LightGray
    };

    internal static FlowDocument ToEditable(XDocument document, XNamespace word)
    {
        var flow = NewFlowDocument();
        var body = document.Root!.Element(word + "body")!;
        foreach (var block in body.Elements())
        {
            if (block.Name == word + "p") flow.Blocks.Add(ConvertParagraph(block, word));
            else if (block.Name == word + "tbl") flow.Blocks.Add(ConvertTable(block, word));
        }
        if (flow.Blocks.Count == 0) flow.Blocks.Add(new Paragraph());
        return flow;
    }

    internal static FlowDocument ToReadOnly(XDocument document, XNamespace word)
    {
        var flow = NewFlowDocument();
        var body = document.Root?.Element(word + "body");
        if (body is null)
        {
            flow.Blocks.Add(new Paragraph(new Run("MiniDoc could not extract a WordprocessingML body.")));
            return flow;
        }

        foreach (var block in body.Elements())
        {
            if (block.Name.LocalName == "sectPr") continue;
            var prefix = block.Name.LocalName == "tbl" ? "[Table — compatibility view]\n" : string.Empty;
            var markers = DescribeUnsupportedObjects(block);
            var text = ExtractText(block);
            var combined = prefix + (markers.Length > 0 ? markers + "\n" : string.Empty) + text;
            flow.Blocks.Add(new Paragraph(new Run(combined.TrimEnd())) { Margin = new Thickness(0, 0, 0, 8) });
        }
        if (flow.Blocks.Count == 0) flow.Blocks.Add(new Paragraph());
        return flow;
    }

    private static FlowDocument NewFlowDocument() => new()
    {
        PagePadding = new Thickness(48),
        FontFamily = new FontFamily("Segoe UI"),
        FontSize = 11 * 96.0 / 72.0,
        Foreground = Brushes.Black
    };

    private static Paragraph ConvertParagraph(XElement xmlParagraph, XNamespace word)
    {
        var paragraph = new Paragraph { Margin = new Thickness(0) };
        var pPr = xmlParagraph.Element(word + "pPr");
        paragraph.Tag = new ParagraphSource(xmlParagraph.Attributes().Select(a => new XAttribute(a)).ToArray(), pPr is null ? null : new XElement(pPr));
        ApplyParagraphFormatting(paragraph, pPr, word);
        foreach (var xmlRun in xmlParagraph.Elements(word + "r")) AddRunContent(paragraph, xmlRun, word);
        return paragraph;
    }

    private static Table ConvertTable(XElement xmlTable, XNamespace word)
    {
        var table = new Table { CellSpacing = 0, Margin = new Thickness(0, 8, 0, 8) };
        var tblPr = xmlTable.Element(word + "tblPr");
        table.Tag = new TableSource(xmlTable.Attributes().Select(a => new XAttribute(a)).ToArray(), tblPr is null ? null : new XElement(tblPr));
        var rows = xmlTable.Elements(word + "tr").ToList();
        var columns = rows.Count == 0 ? 0 : rows[0].Elements(word + "tc").Count();
        for (var i = 0; i < columns; i++) table.Columns.Add(new TableColumn());

        var group = new TableRowGroup();
        table.RowGroups.Add(group);
        foreach (var xmlRow in rows)
        {
            var row = new TableRow { Tag = new TableRowSource(xmlRow.Attributes().Select(a => new XAttribute(a)).ToArray()) };
            foreach (var xmlCell in xmlRow.Elements(word + "tc"))
            {
                var tcPr = xmlCell.Element(word + "tcPr");
                var cell = new TableCell
                {
                    Tag = new TableCellSource(xmlCell.Attributes().Select(a => new XAttribute(a)).ToArray(), tcPr is null ? null : new XElement(tcPr)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0xC8, 0xC8, 0xC8)),
                    BorderThickness = new Thickness(0.75),
                    Padding = new Thickness(5, 3, 5, 3)
                };
                ApplyCellFormatting(cell, tcPr, word);
                foreach (var paragraph in xmlCell.Elements(word + "p")) cell.Blocks.Add(ConvertParagraph(paragraph, word));
                if (cell.Blocks.Count == 0) cell.Blocks.Add(new Paragraph());
                row.Cells.Add(cell);
            }
            group.Rows.Add(row);
        }
        return table;
    }

    private static void ApplyParagraphFormatting(Paragraph paragraph, XElement? properties, XNamespace word)
    {
        var alignment = Value(properties?.Element(word + "jc"), word);
        paragraph.TextAlignment = alignment switch
        {
            "center" => TextAlignment.Center,
            "right" => TextAlignment.Right,
            "both" or "justify" => TextAlignment.Justify,
            _ => TextAlignment.Left
        };

        var indent = properties?.Element(word + "ind");
        var spacing = properties?.Element(word + "spacing");
        paragraph.Margin = new Thickness(
            TwipsToDip(Value(indent, word, "left")),
            TwipsToDip(Value(spacing, word, "before")),
            TwipsToDip(Value(indent, word, "right")),
            TwipsToDip(Value(spacing, word, "after")));

        var fill = ReadDirectFill(properties?.Element(word + "shd"), word);
        if (fill.HasValue) paragraph.Background = new SolidColorBrush(fill.Value);
    }

    private static void ApplyCellFormatting(TableCell cell, XElement? properties, XNamespace word)
    {
        var fill = ReadDirectFill(properties?.Element(word + "shd"), word);
        if (fill.HasValue) cell.Background = new SolidColorBrush(fill.Value);
    }

    private static void AddRunContent(Paragraph paragraph, XElement xmlRun, XNamespace word)
    {
        var properties = xmlRun.Element(word + "rPr");
        var metadata = new RunSource(xmlRun.Attributes().Select(a => new XAttribute(a)).ToArray(), properties is null ? null : new XElement(properties));
        foreach (var child in xmlRun.Elements().Where(e => e.Name != word + "rPr"))
        {
            Inline inline = child.Name.LocalName switch
            {
                "t" => new Run(child.Value),
                "tab" => new Run("\t"),
                "br" or "cr" => new LineBreak(),
                _ => new Run()
            };
            inline.Tag = metadata;
            ApplyRunFormatting(inline, properties, word);
            paragraph.Inlines.Add(inline);
        }
    }

    private static void ApplyRunFormatting(Inline inline, XElement? properties, XNamespace word)
    {
        if (properties is null) return;
        if (ReadOnOff(properties.Element(word + "b"), word)) inline.FontWeight = FontWeights.Bold;
        if (ReadOnOff(properties.Element(word + "i"), word)) inline.FontStyle = FontStyles.Italic;
        var underline = properties.Element(word + "u");
        var underlineValue = Value(underline, word);
        if (underline is not null && underlineValue != "none") inline.TextDecorations = TextDecorations.Underline;

        var fonts = properties.Element(word + "rFonts");
        var family = Value(fonts, word, "ascii") ?? Value(fonts, word, "hAnsi") ?? Value(fonts, word, "eastAsia") ?? Value(fonts, word, "cs");
        if (!string.IsNullOrWhiteSpace(family)) inline.FontFamily = new FontFamily(family);

        var sizeText = Value(properties.Element(word + "sz"), word);
        if (int.TryParse(sizeText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var halfPoints))
            inline.FontSize = halfPoints / 2.0 * 96.0 / 72.0;

        var colorText = Value(properties.Element(word + "color"), word);
        if (!string.IsNullOrWhiteSpace(colorText)) inline.Foreground = new SolidColorBrush(ParseRgb(colorText));

        var highlight = Value(properties.Element(word + "highlight"), word);
        if (highlight is not null && highlight != "none" && HighlightColors.TryGetValue(highlight, out var highlightColor))
            inline.Background = new SolidColorBrush(highlightColor);
    }

    private static string DescribeUnsupportedObjects(XElement block)
    {
        var markers = new List<string>();
        void Add(string value) { if (!markers.Contains(value, StringComparer.Ordinal)) markers.Add(value); }
        foreach (var element in block.Descendants())
        {
            switch (element.Name.LocalName)
            {
                case "drawing": case "pict": case "object": case "AlternateContent": Add("[Graphic/chart/diagram object — preserved source, preview unavailable in this compatibility view]"); break;
                case "footnoteReference": Add("[Footnote reference — source is read-only]"); break;
                case "endnoteReference": Add("[Endnote reference — source is read-only]"); break;
                case "fldChar": case "instrText": case "fldSimple": Add("[Field/TOC content — cached result only; MiniDoc does not recalculate fields]"); break;
            }
        }
        return string.Join(" ", markers);
    }

    private static string ExtractText(XElement element)
    {
        var builder = new StringBuilder();
        Append(element, builder);
        return builder.ToString().TrimEnd();
    }

    private static void Append(XElement element, StringBuilder builder)
    {
        foreach (var node in element.Nodes())
        {
            if (node is not XElement child) continue;
            switch (child.Name.LocalName)
            {
                case "t": builder.Append(child.Value); break;
                case "tab": builder.Append('\t'); break;
                case "br": case "cr": builder.AppendLine(); break;
                case "tc": Append(child, builder); builder.Append(" | "); break;
                case "tr": Append(child, builder); builder.AppendLine(); break;
                case "instrText": break;
                default: Append(child, builder); break;
            }
        }
    }

    private static bool ReadOnOff(XElement? element, XNamespace word)
    {
        if (element is null) return false;
        var value = Value(element, word);
        return value is not ("0" or "false" or "off");
    }

    private static string? Value(XElement? element, XNamespace word, string localName = "val") =>
        element is null ? null : (string?)element.Attribute(word + localName) ?? (string?)element.Attribute(localName);

    private static double TwipsToDip(string? value) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var twips) ? Math.Clamp(twips / 15.0, 0, 3840) : 0;

    private static Color? ReadDirectFill(XElement? shading, XNamespace word)
    {
        if (shading is null) return null;
        var val = Value(shading, word) ?? "clear";
        if (val == "nil") return null;
        var fill = Value(shading, word, "fill");
        return string.IsNullOrWhiteSpace(fill) ? null : ParseRgb(fill);
    }

    private static Color ParseRgb(string rgb) => (Color)ColorConverter.ConvertFromString("#" + rgb)!;
}
