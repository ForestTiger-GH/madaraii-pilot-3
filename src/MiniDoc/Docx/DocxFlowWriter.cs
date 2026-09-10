using System.Windows.Documents;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static partial class DocxFlowWriter
{
    internal static XDocument SerializeMain(DocxSession session, FlowDocument flow)
    {
        var word = session.WordNamespace;
        var output = new XDocument(session.MainDocument);
        var body = output.Root?.Element(word + "body") ?? throw new InvalidDataException("DOCX session lost its main body.");
        body.RemoveNodes();

        foreach (var block in flow.Blocks)
        {
            if (block is Paragraph paragraph) body.Add(SerializeParagraph(paragraph, word));
            else if (block is Table table) body.Add(SerializeTable(table, word));
            else throw new InvalidOperationException($"The editor contains unsupported block '{block.GetType().Name}'. Save was refused.");
        }

        if (session.SectionProperties is not null) body.Add(new XElement(session.SectionProperties));
        return output;
    }

    internal static XElement SerializeParagraph(Paragraph paragraph, XNamespace word)
    {
        var source = paragraph.Tag as ParagraphSource;
        var xml = new XElement(word + "p");
        if (source is not null) foreach (var attribute in source.Attributes) xml.Add(new XAttribute(attribute));
        xml.Add(BuildParagraphProperties(paragraph, source?.Properties, word));
        foreach (var inline in paragraph.Inlines) SerializeInline(inline, xml, word);
        return xml;
    }

    private static void SerializeInline(Inline inline, XElement paragraph, XNamespace word)
    {
        if (inline is Hyperlink)
            throw new InvalidOperationException("Hyperlinks are outside MiniDoc's editable DOCX subset. Save was refused.");
        if (inline is Run run) { paragraph.Add(SerializeRun(run, word)); return; }
        if (inline is LineBreak) { paragraph.Add(new XElement(word + "r", new XElement(word + "br"))); return; }
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
        if (source is not null) foreach (var attribute in source.Attributes) xml.Add(new XAttribute(attribute));
        xml.Add(BuildRunProperties(run, source?.Properties, word));
        AddTextTokens(xml, run.Text ?? string.Empty, word);
        return xml;
    }
}
