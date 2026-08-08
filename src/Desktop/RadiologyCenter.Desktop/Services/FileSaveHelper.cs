namespace RadiologyCenter.Desktop.Services;

public static class FileSaveHelper
{
    public static async Task SaveAsync(byte[] content, string fileName, CancellationToken ct = default)
    {
        var downloads = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var safeName = Path.GetFileNameWithoutExtension(fileName)
            + "_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss")
            + Path.GetExtension(fileName);

        var path = Path.Combine(downloads, safeName);
        await File.WriteAllBytesAsync(path, content, ct);
    }
}