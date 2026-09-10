using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static class DocxFlowReader
{
    internal static FlowDocument ToEditable(XDocument document, XNamespace word)
    {
        var flow = NewFlowDocument();
        var body = document.Root!.Element(word + "body")!;
        foreach (var xmlParagraph in body.Elements(word + "p"))
        {
            var paragraph = new Paragraph { Margin = new Thickness(0, 0, 0, 7) };
            var pPr = xmlParagraph.Element(word + "pPr");
            paragraph.Tag = new ParagraphSource(
                xmlParagraph.Attributes().Select(a => new XAttribute(a)).ToArray(),
                pPr is null ? null : new XElement(pPr));
            ApplyParagraphFormatting(paragraph, pPr, word);

            foreach (var xmlRun in xmlParagraph.Elements(word + "r")) AddRunContent(paragraph, xmlRun, word);
            flow.Blocks.Add(paragraph);
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
            var text = ExtractText(block);
            if (block.Name.LocalName == "tbl") text = "[Table — read-only compatibility view]\n" + text;
            flow.Blocks.Add(new Paragraph(new Run(text)) { Margin = new Thickness(0, 0, 0, 8) });
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

    private static void ApplyParagraphFormatting(Paragraph paragraph, XElement? properties, XNamespace word)
    {
        var value = (string?)properties?.Element(word + "jc")?.Attribute(word + "val")
                    ?? (string?)properties?.Element(word + "jc")?.Attribute("val");
        paragraph.TextAlignment = value switch
        {
            "center" => TextAlignment.Center,
            "right" => TextAlignment.Right,
            "both" or "justify" => TextAlignment.Justify,
            _ => TextAlignment.Left
        };
    }

    private static void AddRunContent(Paragraph paragraph, XElement xmlRun, XNamespace word)
    {
        var properties = xmlRun.Element(word + "rPr");
        var metadata = new RunSource(
            xmlRun.Attributes().Select(a => new XAttribute(a)).ToArray(),
            properties is null ? null : new XElement(properties));

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
        var underlineValue = (string?)underline?.Attribute(word + "val") ?? (string?)underline?.Attribute("val");
        if (underline is not null && underlineValue != "none") inline.TextDecorations = TextDecorations.Underline;

        var fonts = properties.Element(word + "rFonts");
        var family = (string?)fonts?.Attribute(word + "ascii")
                     ?? (string?)fonts?.Attribute(word + "hAnsi")
                     ?? (string?)fonts?.Attribute(word + "eastAsia")
                     ?? (string?)fonts?.Attribute(word + "cs");
        if (!string.IsNullOrWhiteSpace(family)) inline.FontFamily = new FontFamily(family);

        var sizeText = (string?)properties.Element(word + "sz")?.Attribute(word + "val")
                       ?? (string?)properties.Element(word + "sz")?.Attribute("val");
        if (int.TryParse(sizeText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var halfPoints))
            inline.FontSize = halfPoints / 2.0 * 96.0 / 72.0;

        var colorText = (string?)properties.Element(word + "color")?.Attribute(word + "val")
                        ?? (string?)properties.Element(word + "color")?.Attribute("val");
        if (!string.IsNullOrWhiteSpace(colorText))
        {
            var color = (Color)ColorConverter.ConvertFromString("#" + colorText)!;
            inline.Foreground = new SolidColorBrush(color);
        }
    }

    private static bool ReadOnOff(XElement? element, XNamespace word)
    {
        if (element is null) return false;
        var value = (string?)element.Attribute(word + "val") ?? (string?)element.Attribute("val");
        return value is not ("0" or "false" or "off");
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
                case "br":
                case "cr": builder.AppendLine(); break;
                case "tc":
                    Append(child, builder);
                    builder.Append(" | ");
                    break;
                case "tr":
                    Append(child, builder);
                    builder.AppendLine();
                    break;
                default: Append(child, builder); break;
            }
        }
    }
}
