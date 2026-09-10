using MiniDoc.Docx;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace MiniDoc.SelfTest;

internal static class Program
{
    [STAThread]
    private static int Main()
    {
        try
        {
            RoundTripSupportedDocument();
            PreserveOpaquePackageEntry();
            RejectTableForEditing();
            RejectStylePartForEditing();
            RejectSignedPackageForEditing();
            Console.WriteLine("MiniDoc self-test: PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("MiniDoc self-test: FAIL");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void RoundTripSupportedDocument()
    {
        var opened = DocxCodec.CreateNew();
        Require(opened.IsEditable && opened.Session is not null, "new DOCX must be editable");

        opened.Document.Blocks.Clear();
        var paragraph = new Paragraph { TextAlignment = TextAlignment.Center };
        var run = new Run("Hello\tMiniDoc\nSecond line")
        {
            FontFamily = new FontFamily("Segoe UI"),
            FontSize = 14 * 96.0 / 72.0,
            FontWeight = FontWeights.Bold,
            FontStyle = FontStyles.Italic,
            TextDecorations = TextDecorations.Underline,
            Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0x57, 0xB8))
        };
        paragraph.Inlines.Add(run);
        opened.Document.Blocks.Add(paragraph);

        var saved = DocxCodec.SaveToBytes(opened.Session!, opened.Document);
        var reopened = DocxCodec.LoadBytes(saved);
        Require(reopened.IsEditable && reopened.Session is not null, "saved supported DOCX must remain editable");
        var text = new TextRange(reopened.Document.ContentStart, reopened.Document.ContentEnd).Text;
        Require(text.Contains("Hello", StringComparison.Ordinal) && text.Contains("Second line", StringComparison.Ordinal), "round-trip text was lost");
    }

    private static void PreserveOpaquePackageEntry()
    {
        var initial = DocxCodec.CreateNew();
        var withOpaque = Mutate(initial.Session!.OriginalBytesForTest(), archive =>
        {
            var entry = archive.CreateEntry("custom/preserve.bin");
            using var stream = entry.Open();
            stream.Write([1, 3, 3, 7, 9]);
        });

        var opened = DocxCodec.LoadBytes(withOpaque);
        Require(opened.IsEditable && opened.Session is not null, "opaque unrelated package entry must not block the supported main story");
        var before = ReadEntry(withOpaque, "custom/preserve.bin");
        var saved = DocxCodec.SaveToBytes(opened.Session!, opened.Document);
        var after = ReadEntry(saved, "custom/preserve.bin");
        Require(before.SequenceEqual(after), "opaque non-main package payload changed during save");
    }

    private static void RejectTableForEditing()
    {
        var initial = DocxCodec.CreateNew();
        var mutated = ReplaceMainXml(initial.Session!.OriginalBytesForTest(), """
            <?xml version="1.0" encoding="UTF-8"?>
            <w:document xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
              <w:body><w:tbl><w:tr><w:tc><w:p><w:r><w:t>cell</w:t></w:r></w:p></w:tc></w:tr></w:tbl></w:body>
            </w:document>
            """);
        Require(!DocxCodec.LoadBytes(mutated).IsEditable, "table document must be read-only");
    }

    private static void RejectStylePartForEditing()
    {
        var initial = DocxCodec.CreateNew();
        var mutated = Mutate(initial.Session!.OriginalBytesForTest(), archive =>
        {
            var entry = archive.CreateEntry("word/styles.xml");
            using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
            writer.Write("<w:styles xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\"/>");
        });
        Require(!DocxCodec.LoadBytes(mutated).IsEditable, "style-bearing package must be read-only in v0.1");
    }

    private static void RejectSignedPackageForEditing()
    {
        var initial = DocxCodec.CreateNew();
        var mutated = Mutate(initial.Session!.OriginalBytesForTest(), archive =>
        {
            var entry = archive.CreateEntry("_xmlsignatures/sig1.xml");
            using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
            writer.Write("<signature-test/>");
        });
        Require(!DocxCodec.LoadBytes(mutated).IsEditable, "signed package material must force read-only mode");
    }

    private static byte[] ReplaceMainXml(byte[] source, string xml) => Mutate(source, archive =>
    {
        var entry = archive.GetEntry("word/document.xml") ?? throw new InvalidDataException("test fixture has no main part");
        entry.Delete();
        var replacement = archive.CreateEntry("word/document.xml");
        using var writer = new StreamWriter(replacement.Open(), new UTF8Encoding(false));
        writer.Write(xml.Trim());
    });

    private static byte[] Mutate(byte[] source, Action<ZipArchive> action)
    {
        using var memory = new MemoryStream();
        memory.Write(source, 0, source.Length);
        memory.Position = 0;
        using (var archive = new ZipArchive(memory, ZipArchiveMode.Update, leaveOpen: true)) action(archive);
        return memory.ToArray();
    }

    private static byte[] ReadEntry(byte[] package, string name)
    {
        using var memory = new MemoryStream(package, writable: false);
        using var archive = new ZipArchive(memory, ZipArchiveMode.Read);
        using var source = (archive.GetEntry(name) ?? throw new InvalidDataException(name)).Open();
        using var output = new MemoryStream();
        source.CopyTo(output);
        return output.ToArray();
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

internal static class DocxSessionTestAccess
{
    internal static byte[] OriginalBytesForTest(this DocxSession session)
    {
        var result = DocxCodec.SaveToBytes(session, new FlowDocument(new Paragraph(new Run(string.Empty))));
        return result;
    }
}
