using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal sealed record DocxPackageData(byte[] Bytes, string MainPartPath, XDocument MainDocument, bool HasSignature, bool HasStylesPart);

internal static class DocxPackageReader
{
    internal static DocxPackageData ReadFile(string path)
    {
        var info = new FileInfo(path);
        if (!info.Exists) throw new FileNotFoundException("DOCX file was not found.", path);
        if (info.Length > DocxFormat.MaxPackageBytes) throw new InvalidDataException("DOCX exceeds MiniDoc's 64 MiB package limit.");
        return ReadBytes(File.ReadAllBytes(path));
    }

    internal static DocxPackageData ReadBytes(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.LongLength > DocxFormat.MaxPackageBytes) throw new InvalidDataException("DOCX exceeds MiniDoc's 64 MiB package limit.");

        using var memory = new MemoryStream(bytes, writable: false);
        using var archive = OpenArchive(memory);
        ValidateArchive(archive);

        var mainPath = ResolveMainPartPath(archive);
        var mainEntry = archive.GetEntry(mainPath) ?? throw new InvalidDataException($"DOCX package is missing main part '{mainPath}'.");
        if (mainEntry.Length > DocxFormat.MaxMainXmlBytes) throw new InvalidDataException("DOCX main document XML exceeds MiniDoc's 16 MiB limit.");

        XDocument main;
        using (var stream = mainEntry.Open()) main = DocxXml.Load(stream, DocxFormat.MaxMainXmlBytes);

        return new DocxPackageData(
            bytes.ToArray(),
            mainPath,
            main,
            archive.Entries.Any(e => e.FullName.StartsWith("_xmlsignatures/", StringComparison.OrdinalIgnoreCase)),
            archive.Entries.Any(e => e.FullName.EndsWith("/styles.xml", StringComparison.OrdinalIgnoreCase)));
    }

    private static ZipArchive OpenArchive(Stream stream)
    {
        try { return new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false, entryNameEncoding: Encoding.UTF8); }
        catch (InvalidDataException ex) { throw new InvalidDataException("The file is not a readable unencrypted DOCX ZIP package.", ex); }
    }

    private static void ValidateArchive(ZipArchive archive)
    {
        if (archive.Entries.Count > DocxFormat.MaxEntries)
            throw new InvalidDataException($"DOCX has more than MiniDoc's {DocxFormat.MaxEntries} entry limit.");

        var names = new HashSet<string>(StringComparer.Ordinal);
        long aggregate = 0;
        foreach (var entry in archive.Entries)
        {
            if (!names.Add(entry.FullName)) throw new InvalidDataException($"DOCX contains duplicate package entry '{entry.FullName}'.");
            aggregate = checked(aggregate + entry.Length);
            if (aggregate > DocxFormat.MaxUncompressedBytes) throw new InvalidDataException("DOCX exceeds MiniDoc's 256 MiB aggregate uncompressed limit.");
        }
    }

    private static string ResolveMainPartPath(ZipArchive archive)
    {
        var entry = archive.GetEntry("_rels/.rels") ?? throw new InvalidDataException("DOCX package is missing root relationships.");
        if (entry.Length > DocxFormat.MaxRelationshipXmlBytes) throw new InvalidDataException("DOCX root relationships part is unexpectedly large.");

        XDocument relationships;
        using (var stream = entry.Open()) relationships = DocxXml.Load(stream, DocxFormat.MaxRelationshipXmlBytes);

        var relation = relationships.Root?.Elements(DocxFormat.Relationships + "Relationship")
            .FirstOrDefault(r =>
            {
                var type = (string?)r.Attribute("Type");
                var external = string.Equals((string?)r.Attribute("TargetMode"), "External", StringComparison.OrdinalIgnoreCase);
                return !external && (type == DocxFormat.TransitionalOfficeDocumentRelationship || type == DocxFormat.StrictOfficeDocumentRelationship);
            });

        var target = (string?)relation?.Attribute("Target");
        if (string.IsNullOrWhiteSpace(target)) throw new InvalidDataException("DOCX package has no package-local main WordprocessingML relationship.");
        return NormalizeRootTarget(target);
    }

    private static string NormalizeRootTarget(string target)
    {
        target = Uri.UnescapeDataString(target).Replace('\\', '/').TrimStart('/');
        var parts = new List<string>();
        foreach (var segment in target.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (segment == ".") continue;
            if (segment == ".." || segment.Contains(':')) throw new InvalidDataException("DOCX main relationship target escapes the package root.");
            parts.Add(segment);
        }
        if (parts.Count == 0) throw new InvalidDataException("DOCX main relationship target is empty.");
        return string.Join('/', parts);
    }
}
