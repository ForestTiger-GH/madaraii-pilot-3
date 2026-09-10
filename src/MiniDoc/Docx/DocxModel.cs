using System.Windows.Documents;
using System.Xml.Linq;

namespace MiniDoc.Docx;

public sealed class DocxSession
{
    internal DocxSession(byte[] originalBytes, string mainPartPath, XDocument mainDocument, XNamespace wordNamespace, XElement? sectionProperties)
    {
        OriginalBytes = originalBytes;
        MainPartPath = mainPartPath;
        MainDocument = mainDocument;
        WordNamespace = wordNamespace;
        SectionProperties = sectionProperties;
    }

    internal byte[] OriginalBytes { get; }
    internal string MainPartPath { get; }
    internal XDocument MainDocument { get; }
    internal XNamespace WordNamespace { get; }
    internal XElement? SectionProperties { get; }
}

public sealed class DocxOpenResult
{
    internal DocxOpenResult(bool isEditable, FlowDocument document, DocxSession? session, string message)
    {
        IsEditable = isEditable;
        Document = document;
        Session = session;
        Message = message;
    }

    public bool IsEditable { get; }
    public FlowDocument Document { get; }
    public DocxSession? Session { get; }
    public string Message { get; }
}

internal sealed record ParagraphSource(XAttribute[] Attributes, XElement? Properties);
internal sealed record RunSource(XAttribute[] Attributes, XElement? Properties);
internal sealed record TableSource(XAttribute[] Attributes, XElement? Properties);
internal sealed record TableRowSource(XAttribute[] Attributes);
internal sealed record TableCellSource(XAttribute[] Attributes, XElement? Properties);
