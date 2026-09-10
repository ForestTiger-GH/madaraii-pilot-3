using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static class DocxFormat
{
    internal const long MaxPackageBytes = 64L * 1024 * 1024;
    internal const int MaxEntries = 4096;
    internal const long MaxUncompressedBytes = 256L * 1024 * 1024;
    internal const long MaxMainXmlBytes = 16L * 1024 * 1024;
    internal const long MaxRelationshipXmlBytes = 2L * 1024 * 1024;

    internal static readonly XNamespace TransitionalWord = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    internal static readonly XNamespace StrictWord = "http://purl.oclc.org/ooxml/wordprocessingml/main";
    internal static readonly XNamespace Relationships = "http://schemas.openxmlformats.org/package/2006/relationships";

    internal const string TransitionalOfficeDocumentRelationship = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
    internal const string StrictOfficeDocumentRelationship = "http://purl.oclc.org/ooxml/officeDocument/relationships/officeDocument";
}
