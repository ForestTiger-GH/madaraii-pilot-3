using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static class DocxFlowWriter
{
    internal static XDocument SerializeMain(DocxSession session, FlowDocument flow)
    {
        var word = session.WordNamespace;
        var output = new XDocument(session.MainDocument);
        var body = output.Root?.Element(word + "body") ?? throw new InvalidDataException("DOCX session lost its main body.");
        body.RemoveNodes();

        foreach (var block in flow.Blocks)
        {
            if (block is not Paragraph paragraph)
                throw new InvalidOperationException($"The editor contains unsupported block '{block.GetType().Name}'. Save was refused.");
            body.Add(SerializeParagraph(paragraph, word));
        }

        if (session.SectionProperties is not null) body.Add(new XElement(session.SectionProperties));
        return output;
    }

    private static XElement SerializeParagraph(Paragraph paragraph, XNamespace word)
    {
        var source = paragraph.Tag as ParagraphSource;
        var xml = new XElement(word + "p");
        if (source is not null)
        {
            foreach (var attribute in source.Attributes) xml.Add(new XAttribute(attribute));
        }

        var properties = source?.Properties is null ? new XElement(word + "pPr") : new XElement(source.Properties);
        properties.Elements(word + "jc").Remove();
        var alignment = paragraph.TextAlignment switch
        {
            TextAlignment.Center => "center",
            TextAlignment.Right => "right",
            TextAlignment.Justify => "both",
            _ => "left"
        };
        properties.Add(new XElement(word + "jc", new XAttribute(word + "val", alignment)));
        xml.Add(properties);

        foreach (var inline in paragraph.Inlines) SerializeInline(inline, xml, word);
        return xml;
    }

    private static void SerializeInline(Inline inline, XElement paragraph, XNamespace word)
    {
        if (inline is Hyperlink)
            throw new InvalidOperationException("Hyperlinks are outside MiniDoc's editable DOCX subset. Save was refused.");

        if (inline is Run run)
        {
            paragraph.Add(SerializeRun(run, word));
            return;
        }

        if (inline is LineBreak)
        {
            paragraph.Add(new XElement(word + "r", new XElement(word + "br")));
            return;
        }

        if (inline is Span span)
        {
            foreach (var child in span.Inlines) SerializeInline(child, paragraph, word);
            return;
        }

        throw new InvalidOperationException($"The editor contains unsupported inline '{inline.GetType().Name}'. Save was refused.");
    }

    private static XElement SerializeRun(Run run, XNamespace word)
    {
        ValidateRunFormatting(run);
        var source = run.Tag as RunSource;
        var xml = new XElement(word + "r");
        if (source is not null)
        {
            foreach (var attribute in source.Attributes) xml.Add(new XAttribute(attribute));
        }

        xml.Add(BuildProperties(run, source?.Properties, word));
        AddTextTokens(xml, run.Text ?? string.Empty, word);
        return xml;
    }

    private static XElement BuildProperties(Run run, XElement? sourceProperties, XNamespace word)
    {
        var properties = sourceProperties is null ? new XElement(word + "rPr") : new XElement(sourceProperties);
        var owned = new[] { "rFonts", "b", "i", "u", "color", "sz", "szCs" };
        foreach (var local in owned) properties.Elements(word + local).Remove();

        var family = run.FontFamily?.Source;
        if (string.IsNullOrWhiteSpace(family)) family = "Segoe UI";
        properties.Add(new XElement(word + "rFonts",
            new XAttribute(word + "ascii", family),
            new XAttribute(word + "hAnsi", family),
            new XAttribute(word + "eastAsia", family),
            new XAttribute(word + "cs", family)));

        if (run.FontWeight.ToOpenTypeWeight() >= 600) properties.Add(new XElement(word + "b"));
        if (run.FontStyle == FontStyles.Italic) properties.Add(new XElement(word + "i"));
        if (HasUnderline(run)) properties.Add(new XElement(word + "u", new XAttribute(word + "val", "single")));

        var brush = (SolidColorBrush)run.Foreground;
        properties.Add(new XElement(word + "color", new XAttribute(word + "val", $"{brush.Color.R:X2}{brush.Color.G:X2}{brush.Color.B:X2}")));

        var halfPoints = (int)Math.Round(run.FontSize * 72.0 / 96.0 * 2.0, MidpointRounding.AwayFromZero);
        properties.Add(new XElement(word + "sz", new XAttribute(word + "val", halfPoints.ToString(CultureInfo.InvariantCulture))));
        properties.Add(new XElement(word + "szCs", new XAttribute(word + "val", halfPoints.ToString(CultureInfo.InvariantCulture))));
        return properties;
    }

    private static void ValidateRunFormatting(Run run)
    {
        if (run.Foreground is not SolidColorBrush brush || brush.Color.A != 255)
            throw new InvalidOperationException("Only opaque solid RGB text colors can be saved in MiniDoc 0.1.");
        if (!double.IsFinite(run.FontSize) || run.FontSize <= 0 || run.FontSize > 2048)
            throw new InvalidOperationException("The editor contains an unsupported font size.");
        if (run.TextDecorations is not null && run.TextDecorations.Any(d => d.Location != TextDecorationLocation.Underline))
            throw new InvalidOperationException("The editor contains text decoration outside the supported underline subset.");
    }

    private static bool HasUnderline(Run run) =>
        run.TextDecorations is not null && run.TextDecorations.Any(d => d.Location == TextDecorationLocation.Underline);

    private static void AddTextTokens(XElement run, string text, XNamespace word)
    {
        var buffer = new StringBuilder();
        void Flush()
        {
            if (buffer.Length == 0) return;
            run.Add(new XElement(word + "t", new XAttribute(XNamespace.Xml + "space", "preserve"), buffer.ToString()));
            buffer.Clear();
        }

        for (var index = 0; index < text.Length; index++)
        {
            var ch = text[index];
            if (ch == '\t')
            {
                Flush();
                run.Add(new XElement(word + "tab"));
            }
            else if (ch == '\r' || ch == '\n')
            {
                Flush();
                if (ch == '\r' && index + 1 < text.Length && text[index + 1] == '\n') index++;
                run.Add(new XElement(word + "br"));
            }
            else buffer.Append(ch);
        }
        Flush();
    }
}
