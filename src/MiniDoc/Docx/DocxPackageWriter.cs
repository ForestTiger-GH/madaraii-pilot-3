using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace MiniDoc.Docx;

internal static class DocxPackageWriter
{
    internal static byte[] ReplaceMainPart(DocxSession session, XDocument updatedMain)
    {
        var mainBytes = DocxXml.Save(updatedMain);
        if (mainBytes.LongLength > DocxFormat.MaxMainXmlBytes)
            throw new InvalidDataException("Edited main document XML exceeds MiniDoc's 16 MiB limit.");

        using var output = new MemoryStream(session.OriginalBytes.Length + Math.Max(mainBytes.Length, 4096));
        output.Write(session.OriginalBytes, 0, session.OriginalBytes.Length);
        output.Position = 0;

        using (var archive = new ZipArchive(output, ZipArchiveMode.Update, leaveOpen: true, entryNameEncoding: Encoding.UTF8))
        {
            var previous = archive.GetEntry(session.MainPartPath)
                ?? throw new InvalidDataException("Main DOCX part disappeared from the session package.");
            previous.Delete();
            var replacement = archive.CreateEntry(session.MainPartPath, CompressionLevel.Optimal);
            using var stream = replacement.Open();
            stream.Write(mainBytes, 0, mainBytes.Length);
        }

        var result = output.ToArray();
        if (result.LongLength > DocxFormat.MaxPackageBytes)
            throw new InvalidDataException("Saved DOCX exceeds MiniDoc's 64 MiB package limit.");
        AssertNonMainPayloadsPreserved(session.OriginalBytes, result, session.MainPartPath);
        return result;
    }

    internal static byte[] CreateMinimalPackage()
    {
        using var memory = new MemoryStream();
        using (var archive = new ZipArchive(memory, ZipArchiveMode.Create, leaveOpen: true, entryNameEncoding: Encoding.UTF8))
        {
            WriteText(archive, "[Content_Types].xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
                  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
                  <Default Extension="xml" ContentType="application/xml"/>
                  <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"/>
                </Types>
                """);
            WriteText(archive, "_rels/.rels", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
                  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="word/document.xml"/>
                </Relationships>
                """);
            WriteText(archive, "word/document.xml", """
                <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
                <w:document xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
                  <w:body><w:p><w:r><w:t xml:space="preserve"></w:t></w:r></w:p></w:body>
                </w:document>
                """);
        }
        return memory.ToArray();
    }

    private static void WriteText(ZipArchive archive, string name, string text)
    {
        var entry = archive.CreateEntry(name, CompressionLevel.Optimal);
        using var stream = entry.Open();
        var bytes = new UTF8Encoding(false).GetBytes(text.Trim());
        stream.Write(bytes, 0, bytes.Length);
    }

    private static void AssertNonMainPayloadsPreserved(byte[] before, byte[] after, string mainPath)
    {
        var beforeHashes = HashProtectedEntries(before, mainPath);
        var afterHashes = HashProtectedEntries(after, mainPath);
        if (beforeHashes.Count != afterHashes.Count)
            throw new InvalidDataException("Saving changed the set of protected non-main DOCX package entries.");

        foreach (var pair in beforeHashes)
        {
            if (!afterHashes.TryGetValue(pair.Key, out var actual) || !CryptographicOperations.FixedTimeEquals(pair.Value, actual))
                throw new InvalidDataException($"Saving changed protected non-main DOCX part '{pair.Key}'.");
        }
    }

    private static Dictionary<string, byte[]> HashProtectedEntries(byte[] package, string mainPath)
    {
        using var memory = new MemoryStream(package, writable: false);
        using var archive = new ZipArchive(memory, ZipArchiveMode.Read, leaveOpen: false, entryNameEncoding: Encoding.UTF8);
        var result = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        foreach (var entry in archive.Entries)
        {
            if (entry.FullName == mainPath) continue;
            using var stream = entry.Open();
            result[entry.FullName] = SHA256.HashData(stream);
        }
        return result;
    }
}
