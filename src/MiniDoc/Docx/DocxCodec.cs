using System.Windows.Documents;

namespace MiniDoc.Docx;

public static class DocxCodec
{
    public static DocxOpenResult Open(string path) => BuildResult(DocxPackageReader.ReadFile(path));

    public static DocxOpenResult LoadBytes(byte[] bytes) => BuildResult(DocxPackageReader.ReadBytes(bytes));

    public static DocxOpenResult CreateNew() => LoadBytes(DocxPackageWriter.CreateMinimalPackage());

    public static byte[] SaveToBytes(DocxSession session, FlowDocument document)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(document);
        var main = DocxFlowWriter.SerializeMain(session, document);
        var package = DocxPackageWriter.ReplaceMainPart(session, main);
        var check = LoadBytes(package);
        if (!check.IsEditable || check.Session is null)
            throw new InvalidDataException("Generated DOCX did not pass MiniDoc's own editable compatibility check.");
        return package;
    }

    private static DocxOpenResult BuildResult(DocxPackageData package)
    {
        var word = package.MainDocument.Root?.Name.Namespace ?? throw new InvalidDataException("DOCX main document has no root element.");
        var reasons = DocxCompatibility.Analyze(package);
        if (reasons.Count > 0)
        {
            var message = "Read-only compatibility mode. MiniDoc will not save changes because: " + string.Join("; ", reasons.Take(8));
            return new DocxOpenResult(false, DocxFlowReader.ToReadOnly(package.MainDocument, word), null, message);
        }

        var body = package.MainDocument.Root!.Element(word + "body")!;
        var section = body.Elements(word + "sectPr").LastOrDefault();
        var session = new DocxSession(
            package.Bytes,
            package.MainPartPath,
            new System.Xml.Linq.XDocument(package.MainDocument),
            word,
            section is null ? null : new System.Xml.Linq.XElement(section));

        return new DocxOpenResult(true, DocxFlowReader.ToEditable(package.MainDocument, word), session, "Editable DOCX");
    }
}
