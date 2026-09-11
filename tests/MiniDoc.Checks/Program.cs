using MiniDoc.Docx;
using MiniDoc.Editor;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

internal static class Program
{
    [STAThread]
    private static int Main()
    {
        try
        {
            CheckSearchAcrossRuns();
            CheckEditableTableRoundTrip();
            CheckUnsupportedDrawingFailsClosed();
            Console.WriteLine("MiniDoc.Checks: PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("MiniDoc.Checks: FAIL");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void CheckSearchAcrossRuns()
    {
        var document = new FlowDocument();
        var paragraph = new Paragraph();
        paragraph.Inlines.Add(new Run("Hello "));
        paragraph.Inlines.Add(new Run("World"));
        document.Blocks.Add(paragraph);

        var matches = TextSearch.FindAll(document, "hello world", caseSensitive: false);
        Require(matches.Count == 1, "Search must find text spanning adjacent runs in one paragraph.");
        var replaced = TextSearch.ReplaceAll(document, "WORLD", "MiniDoc", caseSensitive: false);
        Require(replaced == 1, "ReplaceAll must replace the expected match.");
        Require(new TextRange(paragraph.ContentStart, paragraph.ContentEnd).Text.Contains("Hello MiniDoc", StringComparison.Ordinal),
            "ReplaceAll must preserve surrounding paragraph text.");
    }

    private static void CheckEditableTableRoundTrip()
    {
        var opened = DocxCodec.CreateNew();
        Require(opened.IsEditable && opened.Session is not null, "A new document must be editable.");
        var document = opened.Document;
        var firstParagraph = document.Blocks.OfType<Paragraph>().First();
        firstParagraph.Inlines.Clear();
        var highlighted = new Run("MiniDoc table fixture")
        {
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.DarkBlue,
            Background = Brushes.Yellow
        };
        firstParagraph.Inlines.Add(highlighted);
        firstParagraph.TextAlignment = TextAlignment.Center;
        firstParagraph.Margin = new Thickness(24, 8, 0, 12);

        var table = TableEditor.InsertTable(document, firstParagraph.ContentEnd, rows: 2, columns: 2);
        var rows = table.RowGroups[0].Rows;
        SetCellText(rows[0].Cells[0], "A1");
        SetCellText(rows[0].Cells[1], "B1");
        SetCellText(rows[1].Cells[0], "A2");
        SetCellText(rows[1].Cells[1], "B2");
        rows[0].Cells[0].Background = new SolidColorBrush(Color.FromRgb(0xDD, 0xEB, 0xF7));

        var bytes = DocxCodec.SaveToBytes(opened.Session!, document);
        var reloaded = DocxCodec.LoadBytes(bytes);
        Require(reloaded.IsEditable && reloaded.Session is not null, "Saved supported document must reopen editable.");
        Require(reloaded.Document.Blocks.OfType<Table>().Count() == 1, "Table must survive DOCX round trip.");
        var text = new TextRange(reloaded.Document.ContentStart, reloaded.Document.ContentEnd).Text;
        Require(text.Contains("MiniDoc table fixture", StringComparison.Ordinal), "Paragraph text must survive round trip.");
        Require(text.Contains("A1", StringComparison.Ordinal) && text.Contains("B2", StringComparison.Ordinal), "Table cell text must survive round trip.");
    }

    private static void CheckUnsupportedDrawingFailsClosed()
    {
        var package = DocxPackageWriter.CreateMinimalPackage();
        var word = DocxFormat.TransitionalWord;
        var document = new XDocument(
            new XElement(word + "document",
                new XAttribute(XNamespace.Xmlns + "w", word.NamespaceName),
                new XElement(word + "body",
                    new XElement(word + "p",
                        new XElement(word + "r",
                            new XElement(word + "drawing"))))));
        var mutated = ReplaceMain(package, document);
        var result = DocxCodec.LoadBytes(mutated);
        Require(!result.IsEditable && result.Session is null, "Unsupported drawing must never gain editable/save authority.");
        Require(result.Message.Contains("read-only", StringComparison.OrdinalIgnoreCase), "Unsupported drawing must report compatibility mode.");
    }

    private static byte[] ReplaceMain(byte[] original, XDocument main)
    {
        using var output = new MemoryStream(original.Length + 1024);
        output.Write(original, 0, original.Length);
        output.Position = 0;
        using (var archive = new ZipArchive(output, ZipArchiveMode.Update, leaveOpen: true, entryNameEncoding: Encoding.UTF8))
        {
            archive.GetEntry("word/document.xml")!.Delete();
            var entry = archive.CreateEntry("word/document.xml", CompressionLevel.Optimal);
            using var stream = entry.Open();
            main.Save(stream, SaveOptions.DisableFormatting);
        }
        return output.ToArray();
    }

    private static void SetCellText(TableCell cell, string value)
    {
        cell.Blocks.Clear();
        cell.Blocks.Add(new Paragraph(new Run(value)));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
