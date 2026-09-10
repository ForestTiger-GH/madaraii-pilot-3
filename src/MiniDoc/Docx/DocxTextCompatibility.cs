using System.Globalization;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static class DocxTextCompatibility
{
    private static readonly HashSet<string> Highlights = new(StringComparer.Ordinal)
    {
        "black", "blue", "cyan", "green", "magenta", "red", "yellow", "white",
        "darkBlue", "darkCyan", "darkGreen", "darkMagenta", "darkRed", "darkYellow", "darkGray", "lightGray", "none"
    };

    internal static void AnalyzeParagraph(XElement paragraph, XNamespace word, Action<string> add)
    {
        var sawProperties = false;
        foreach (var child in paragraph.Elements())
        {
            if (child.Name == word + "pPr" && !sawProperties)
            {
                sawProperties = true;
                foreach (var property in child.Elements())
                {
                    if (property.Name == word + "jc")
                    {
                        var value = Value(property, word);
                        if (value is not ("left" or "center" or "right" or "both" or "justify"))
                            add($"unsupported paragraph alignment '{value}'");
                    }
                    else if (property.Name == word + "ind") AnalyzeNumericAttributes(property, word, add, "paragraph indentation", "left", "right");
                    else if (property.Name == word + "spacing") AnalyzeNumericAttributes(property, word, add, "paragraph spacing", "before", "after");
                    else if (property.Name == word + "shd") AnalyzeShading(property, word, add, "paragraph");
                    else add($"unsupported paragraph property '{property.Name.LocalName}'");
                }
            }
            else if (child.Name == word + "r") AnalyzeRun(child, word, add);
            else add($"unsupported paragraph element '{child.Name.LocalName}'");
        }
    }

    internal static void AnalyzeShading(XElement property, XNamespace word, Action<string> add, string scope)
    {
        var value = Value(property, word) ?? "clear";
        var fill = Value(property, word, "fill");
        if (value is not ("clear" or "nil")) add($"{scope} shading pattern exceeds the clear-fill subset");
        if (value == "clear" && !IsRgb(fill)) add($"{scope} shading is not a direct six-digit RGB fill");
        foreach (var attribute in property.Attributes().Where(a => !a.IsNamespaceDeclaration))
        {
            if (attribute.Name == word + "val" || attribute.Name == word + "fill" || attribute.Name == word + "color") continue;
            add($"{scope} shading carries unsupported metadata");
        }
    }

    internal static string? Value(XElement element, XNamespace word, string localName = "val") =>
        (string?)element.Attribute(word + localName) ?? (string?)element.Attribute(localName);

    private static void AnalyzeRun(XElement run, XNamespace word, Action<string> add)
    {
        var sawProperties = false;
        foreach (var child in run.Elements())
        {
            if (child.Name == word + "rPr" && !sawProperties)
            {
                sawProperties = true;
                AnalyzeRunProperties(child, word, add);
            }
            else if (child.Name == word + "t")
            {
                if (child.Attributes().Any(a => !a.IsNamespaceDeclaration && a.Name != XNamespace.Xml + "space"))
                    add("text carries unsupported attributes");
            }
            else if (child.Name == word + "tab" || child.Name == word + "cr") { }
            else if (child.Name == word + "br")
            {
                var type = Value(child, word, "type");
                var clear = Value(child, word, "clear");
                if (!string.IsNullOrEmpty(clear) || type is not (null or "textWrapping"))
                    add("page, column, or cleared line breaks are outside the editable subset");
            }
            else add($"unsupported run element '{child.Name.LocalName}'");
        }
    }

    private static void AnalyzeRunProperties(XElement properties, XNamespace word, Action<string> add)
    {
        foreach (var property in properties.Elements())
        {
            if (property.Name == word + "b" || property.Name == word + "i")
            {
                var value = Value(property, word);
                if (value is not (null or "1" or "true" or "on" or "0" or "false" or "off"))
                    add($"unsupported {property.Name.LocalName} value '{value}'");
            }
            else if (property.Name == word + "u")
            {
                var value = Value(property, word) ?? "single";
                if (value is not ("single" or "none")) add($"unsupported underline style '{value}'");
            }
            else if (property.Name == word + "rFonts") AnalyzeFonts(property, word, add);
            else if (property.Name == word + "sz" || property.Name == word + "szCs")
            {
                var value = Value(property, word);
                if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var halfPoints) || halfPoints <= 0 || halfPoints > 4096)
                    add($"unsupported font size '{value}'");
            }
            else if (property.Name == word + "color")
            {
                var value = Value(property, word);
                var extra = property.Attributes().Any(a => !a.IsNamespaceDeclaration && a.Name != word + "val" && a.Name.LocalName != "val");
                if (extra || !IsRgb(value)) add("run text color is not a direct six-digit RGB value");
            }
            else if (property.Name == word + "highlight")
            {
                var value = Value(property, word) ?? "none";
                if (!Highlights.Contains(value)) add($"unsupported text highlight '{value}'");
            }
            else add($"unsupported run property '{property.Name.LocalName}'");
        }
    }

    private static void AnalyzeFonts(XElement property, XNamespace word, Action<string> add)
    {
        var allowed = new HashSet<XName> { word + "ascii", word + "hAnsi", word + "eastAsia", word + "cs" };
        if (property.Attributes().Any(a => !a.IsNamespaceDeclaration && !allowed.Contains(a.Name)))
            add("theme or script font metadata exceeds the direct-font subset");
        var fonts = property.Attributes().Where(a => allowed.Contains(a.Name)).Select(a => a.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (fonts.Count > 1) add("run uses different fonts for different scripts");
    }

    private static void AnalyzeNumericAttributes(XElement property, XNamespace word, Action<string> add, string label, params string[] allowedNames)
    {
        foreach (var attribute in property.Attributes().Where(a => !a.IsNamespaceDeclaration))
        {
            if (!allowedNames.Any(name => attribute.Name == word + name)) { add($"{label} carries unsupported semantics"); continue; }
            if (!int.TryParse(attribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) || value < 0 || value > 57600)
                add($"{label} is outside MiniDoc's bounded range");
        }
    }

    private static bool IsRgb(string? value) =>
        value is not null && value.Length == 6 && int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
}
