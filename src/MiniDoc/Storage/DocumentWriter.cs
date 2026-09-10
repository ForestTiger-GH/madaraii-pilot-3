namespace MiniDoc.Storage;

public static class DocumentWriter
{
    public static void WriteExplicitTarget(string path, byte[] bytes)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("A save target path is required.", nameof(path));
        ArgumentNullException.ThrowIfNull(bytes);

        var fullPath = Path.GetFullPath(path);
        var existed = File.Exists(fullPath);
        byte[]? previous = null;
        if (existed) previous = File.ReadAllBytes(fullPath);

        try
        {
            using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 64 * 1024, FileOptions.WriteThrough);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush(flushToDisk: true);
        }
        catch
        {
            try
            {
                if (existed && previous is not null)
                {
                    using var restore = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 64 * 1024, FileOptions.WriteThrough);
                    restore.Write(previous, 0, previous.Length);
                    restore.Flush(flushToDisk: true);
                }
                else if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch
            {
                // The original failure remains authoritative. Documentation states that power/process failure is not atomic.
            }
            throw;
        }
    }
}
