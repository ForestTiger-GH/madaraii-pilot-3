using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static class DocxXml
{
    internal static XDocument Load(Stream stream, long maxCharacters)
    {
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = maxCharacters,
            IgnoreWhitespace = false,
            CloseInput = false
        };
        using var reader = XmlReader.Create(stream, settings);
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
    }

    internal static byte[] Save(XDocument document)
    {
        using var memory = new MemoryStream();
        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            Indent = false,
            OmitXmlDeclaration = false,
            CloseOutput = false
        };
        using (var writer = XmlWriter.Create(memory, settings)) document.Save(writer);
        return memory.ToArray();
    }
}
