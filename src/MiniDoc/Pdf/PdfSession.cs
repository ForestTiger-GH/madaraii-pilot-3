using System.IO;
using System.Windows.Media.Imaging;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MiniDoc.Pdf;

public sealed class PdfSession : IDisposable
{
    private PdfDocument? _document;

    private PdfSession(PdfDocument document)
    {
        _document = document;
    }

    public uint PageCount => _document?.PageCount ?? 0;

    public static async Task<PdfSession> OpenAsync(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var file = await StorageFile.GetFileFromPathAsync(fullPath);
        PdfDocument document;
        try
        {
            document = await PdfDocument.LoadFromFileAsync(file);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("MiniDoc could not open this PDF. Password-protected and malformed PDFs are outside the v0.1 supported boundary.", ex);
        }

        if (document.IsPasswordProtected)
            throw new NotSupportedException("Password-protected PDFs are outside MiniDoc 0.1 support.");
        if (document.PageCount == 0)
            throw new InvalidDataException("The PDF contains no readable pages.");
        return new PdfSession(document);
    }

    public async Task<BitmapImage> RenderPageAsync(uint pageIndex, double zoom)
    {
        var document = _document ?? throw new ObjectDisposedException(nameof(PdfSession));
        if (pageIndex >= document.PageCount) throw new ArgumentOutOfRangeException(nameof(pageIndex));
        if (zoom < 0.5 || zoom > 4.0) throw new ArgumentOutOfRangeException(nameof(zoom));

        using var page = document.GetPage(pageIndex);
        using var output = new InMemoryRandomAccessStream();
        var width = checked((uint)Math.Max(1, Math.Round(page.Size.Width * zoom)));
        var options = new PdfPageRenderOptions { DestinationWidth = width };
        await page.RenderToStreamAsync(output, options);

        if (output.Size > int.MaxValue) throw new InvalidDataException("Rendered PDF page is too large for MiniDoc's in-memory viewer.");
        output.Seek(0);
        using var input = output.GetInputStreamAt(0);
        using var reader = new DataReader(input);
        var length = (uint)output.Size;
        await reader.LoadAsync(length);
        var bytes = new byte[(int)length];
        reader.ReadBytes(bytes);

        using var memory = new MemoryStream(bytes, writable: false);
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = memory;
        image.EndInit();
        image.Freeze();
        return image;
    }

    public void Dispose()
    {
        _document = null;
    }
}
