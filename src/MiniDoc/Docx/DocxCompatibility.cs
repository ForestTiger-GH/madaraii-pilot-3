using System.Globalization;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static class DocxCompatibility
{
    internal static IReadOnlyList<string> Analyze(DocxPackageData package)
    {
        var reasons = new List<string>();
        void Add(string value)
        {
            if (!reasons.Contains(value, StringComparer.Ordinal)) reasons.Add(value);
        }

        var document = package.MainDocument;
        var word = document.Root?.Name.Namespace ?? XNamespace.None;

        if (package.HasSignature) Add("the package contains digital-signature material");
        if (package.HasStylesPart) Add("the package contains a style-definition part whose cascade MiniDoc 0.1 does not safely edit");
        if (word == DocxFormat.StrictWord) Add("Strict OOXML is outside the editable subset");
        else if (word != DocxFormat.TransitionalWord) Add("the main document uses an unsupported WordprocessingML namespace");

        var root = document.Root;
        if (root is null || root.Name != word + "document")
        {
            Add("the main part is not a supported WordprocessingML document root");
            return reasons;
        }

        var body = root.Element(word + "body");
        if (body is null)
        {
            Add("the main document has no body");
            return reasons;
        }

        var bodyElements = body.Elements().ToList();
        for (var index = 0; index < bodyElements.Count; index++)
        {
            var element = bodyElements[index];
            if (element.Name == word + "p") AnalyzeParagraph(element, word, Add);
            else if (element.Name == word + "sectPr" && index == bodyElements.Count - 1) { }
            else Add($"unsupported body element '{element.Name.LocalName}'");
        }

        return reasons;
    }

    private static void AnalyzeParagraph(XElement paragraph, XNamespace word, Action<string> add)
    {
        var sawProperties = false;
        foreach (var child in paragraph.Elements())
        {
            if (child.Name == word + "pPr" && !sawProperties)
            {
                sawProperties = true;
                foreach (var property in child.Elements())
                {
                    if (property.Name != word + "jc")
                    {
                        add($"unsupported paragraph property '{property.Name.LocalName}'");
                        continue;
                    }

                    var value = Value(property, word);
                    if (value is not ("left" or "center" or "right" or "both" or "justify"))
                        add($"unsupported paragraph alignment '{value}'");
                }
            }
            else if (child.Name == word + "r") AnalyzeRun(child, word, add);
            else add($"unsupported paragraph element '{child.Name.LocalName}'");
        }
    }

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
                if (!IsOnOff(value)) add($"unsupported {property.Name.LocalName} value '{value}'");
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
                if (extra || value is null || value.Length != 6 || !int.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
                    add("run text color is not a direct six-digit RGB value");
            }
            else add($"unsupported run property '{property.Name.LocalName}'");
        }
    }

    private static void AnalyzeFonts(XElement property, XNamespace word, Action<string> add)
    {
        var allowed = new HashSet<XName> { word + "ascii", word + "hAnsi", word + "eastAsia", word + "cs" };
        if (property.Attributes().Any(a => !a.IsNamespaceDeclaration && !allowed.Contains(a.Name)))
            add("theme or script font metadata exceeds the direct-font subset");

        var fonts = property.Attributes()
            .Where(a => allowed.Contains(a.Name))
            .Select(a => a.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (fonts.Count > 1) add("run uses different fonts for different scripts");
    }

    private static string? Value(XElement element, XNamespace word, string localName = "val") =>
        (string?)element.Attribute(word + localName) ?? (string?)element.Attribute(localName);

    private static bool IsOnOff(string? value) => value is null or "1" or "true" or "on" or "0" or "false" or "off";
}
