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
            if (element.Name == word + "p") DocxTextCompatibility.AnalyzeParagraph(element, word, Add);
            else if (element.Name == word + "tbl") DocxTableCompatibility.Analyze(element, word, Add);
            else if (element.Name == word + "sectPr" && index == bodyElements.Count - 1) { }
            else Add($"unsupported body element '{element.Name.LocalName}'");
        }
        return reasons;
    }
}
