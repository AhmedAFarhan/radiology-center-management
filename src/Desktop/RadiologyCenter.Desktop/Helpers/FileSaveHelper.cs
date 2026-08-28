namespace RadiologyCenter.Desktop.Helpers;

public static class FileSaveHelper
{
    private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

    public static async Task<string> SaveAsync(byte[] content, string fileName, CancellationToken ct = default)
    {
        var exportDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "EGcare Exports");
        Directory.CreateDirectory(exportDir);

        var safeName = Sanitize(Path.GetFileNameWithoutExtension(fileName))
            + "_"
            + DateTime.Now.ToString("yyyyMMdd_HHmmss")
            + Sanitize(Path.GetExtension(fileName));

        var path = UniquePath(Path.Combine(exportDir, safeName));
        await File.WriteAllBytesAsync(path, content, ct);
        return path;
    }

    private static string Sanitize(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "export";

        var cleaned = new string(name.Select(c => InvalidFileNameChars.Contains(c) ? '_' : c).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(cleaned) ? "export" : cleaned;
    }

    private static string UniquePath(string path)
    {
        if (!File.Exists(path))
            return path;

        var dir = Path.GetDirectoryName(path)!;
        var name = Path.GetFileNameWithoutExtension(path);
        var ext = Path.GetExtension(path);

        for (var i = 1; i < 1000; i++)
        {
            var candidate = Path.Combine(dir, $"{name} ({i}){ext}");
            if (!File.Exists(candidate))
                return candidate;
        }

        return path;
    }
}