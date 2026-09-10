using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static partial class DocxFlowWriter
{
    private static XElement BuildParagraphProperties(Paragraph paragraph, XElement? sourceProperties, XNamespace word)
    {
        var properties = sourceProperties is null ? new XElement(word + "pPr") : new XElement(sourceProperties);
        foreach (var local in new[] { "jc", "ind", "spacing", "shd" }) properties.Elements(word + local).Remove();

        var alignment = paragraph.TextAlignment switch
        {
            TextAlignment.Center => "center", TextAlignment.Right => "right", TextAlignment.Justify => "both", _ => "left"
        };
        properties.Add(new XElement(word + "jc", new XAttribute(word + "val", alignment)));

        var left = DipToTwips(paragraph.Margin.Left);
        var right = DipToTwips(paragraph.Margin.Right);
        if (left > 0 || right > 0)
        {
            var ind = new XElement(word + "ind");
            if (left > 0) ind.Add(new XAttribute(word + "left", left.ToString(CultureInfo.InvariantCulture)));
            if (right > 0) ind.Add(new XAttribute(word + "right", right.ToString(CultureInfo.InvariantCulture)));
            properties.Add(ind);
        }

        var before = DipToTwips(paragraph.Margin.Top);
        var after = DipToTwips(paragraph.Margin.Bottom);
        if (before > 0 || after > 0)
        {
            var spacing = new XElement(word + "spacing");
            if (before > 0) spacing.Add(new XAttribute(word + "before", before.ToString(CultureInfo.InvariantCulture)));
            if (after > 0) spacing.Add(new XAttribute(word + "after", after.ToString(CultureInfo.InvariantCulture)));
            properties.Add(spacing);
        }
        AddDirectShading(properties, paragraph.Background, word);
        return properties;
    }

    private static XElement BuildRunProperties(Run run, XElement? sourceProperties, XNamespace word)
    {
        var properties = sourceProperties is null ? new XElement(word + "rPr") : new XElement(sourceProperties);
        foreach (var local in new[] { "rFonts", "b", "i", "u", "color", "sz", "szCs", "highlight" }) properties.Elements(word + local).Remove();

        var family = run.FontFamily?.Source;
        if (string.IsNullOrWhiteSpace(family)) family = "Segoe UI";
        properties.Add(new XElement(word + "rFonts",
            new XAttribute(word + "ascii", family), new XAttribute(word + "hAnsi", family),
            new XAttribute(word + "eastAsia", family), new XAttribute(word + "cs", family)));
        if (run.FontWeight.ToOpenTypeWeight() >= 600) properties.Add(new XElement(word + "b"));
        if (run.FontStyle == FontStyles.Italic) properties.Add(new XElement(word + "i"));
        if (HasUnderline(run)) properties.Add(new XElement(word + "u", new XAttribute(word + "val", "single")));

        var brush = run.Foreground as SolidColorBrush ?? throw new InvalidOperationException("Only solid RGB text colors can be saved in MiniDoc 0.1.");
        properties.Add(new XElement(word + "color", new XAttribute(word + "val", Rgb(brush.Color))));
        var halfPoints = (int)Math.Round(run.FontSize * 72.0 / 96.0 * 2.0, MidpointRounding.AwayFromZero);
        properties.Add(new XElement(word + "sz", new XAttribute(word + "val", halfPoints.ToString(CultureInfo.InvariantCulture))));
        properties.Add(new XElement(word + "szCs", new XAttribute(word + "val", halfPoints.ToString(CultureInfo.InvariantCulture))));

        var highlight = EffectiveInlineBackground(run);
        if (highlight is SolidColorBrush hb && hb.Color.A != 0)
            properties.Add(new XElement(word + "highlight", new XAttribute(word + "val", ToHighlightName(hb.Color))));
        return properties;
    }

    private static Brush? EffectiveInlineBackground(Run run)
    {
        FrameworkContentElement? current = run;
        while (current is Inline)
        {
            var local = current.ReadLocalValue(TextElement.BackgroundProperty);
            if (local != DependencyProperty.UnsetValue) return local as Brush;
            current = current.Parent as FrameworkContentElement;
        }
        return null;
    }

    private static string ToHighlightName(Color color)
    {
        if (color == Colors.Yellow) return "yellow";
        if (color == Colors.Lime || color == Colors.Green) return "green";
        if (color == Colors.Cyan) return "cyan";
        if (color == Colors.Magenta) return "magenta";
        if (color == Colors.Blue) return "blue";
        if (color == Colors.Red) return "red";
        if (color == Colors.Black) return "black";
        if (color == Colors.White) return "white";
        if (color == Colors.DarkBlue) return "darkBlue";
        if (color == Colors.DarkCyan) return "darkCyan";
        if (color == Colors.DarkGreen) return "darkGreen";
        if (color == Colors.DarkMagenta) return "darkMagenta";
        if (color == Colors.DarkRed) return "darkRed";
        if (color == Colors.Olive) return "darkYellow";
        if (color == Colors.DarkGray) return "darkGray";
        if (color == Colors.LightGray) return "lightGray";
        throw new InvalidOperationException("The selected highlight color is outside MiniDoc's WordprocessingML highlight subset.");
    }

    internal static void AddDirectShading(XElement properties, Brush? brush, XNamespace word)
    {
        if (brush is null) return;
        if (brush is not SolidColorBrush solid) throw new InvalidOperationException("Only solid RGB fill colors can be saved.");
        if (solid.Color.A == 0) return;
        if (solid.Color.A != 255) throw new InvalidOperationException("Only opaque RGB fill colors can be saved.");
        properties.Add(new XElement(word + "shd", new XAttribute(word + "val", "clear"),
            new XAttribute(word + "color", "auto"), new XAttribute(word + "fill", Rgb(solid.Color))));
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

    private static bool HasUnderline(Run run) => run.TextDecorations is not null && run.TextDecorations.Any(d => d.Location == TextDecorationLocation.Underline);
    private static int DipToTwips(double dip) => double.IsFinite(dip) ? Math.Clamp((int)Math.Round(Math.Max(0, dip) * 15.0), 0, 57600) : 0;
    private static string Rgb(Color color) => $"{color.R:X2}{color.G:X2}{color.B:X2}";

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
            if (ch == '\t') { Flush(); run.Add(new XElement(word + "tab")); }
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
