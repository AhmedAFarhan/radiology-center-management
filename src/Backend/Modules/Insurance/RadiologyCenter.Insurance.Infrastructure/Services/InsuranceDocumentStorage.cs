using Microsoft.Extensions.Options;
using RadiologyCenter.Insurance.Application.Abstractions;

namespace RadiologyCenter.Insurance.Infrastructure.Services;

public sealed class InsuranceDocumentStorage : IInsuranceDocumentStorage
{
    private readonly string _rootPath;

    public InsuranceDocumentStorage(IOptions<DocumentStorageOptions> options)
    {
        _rootPath = Path.GetFullPath(options.Value.RootPath);
        Directory.CreateDirectory(_rootPath);
    }

    public string RootPath => _rootPath;

    public async Task<string> SaveAsync(string relativeDirectory, string fileName, Stream content, CancellationToken ct = default)
    {
        var directory = Path.Combine(_rootPath, relativeDirectory);
        Directory.CreateDirectory(directory);

        var storedName = $"{Guid.NewGuid():N}-{SanitizeFileName(fileName)}";
        var storedPath = Path.Combine(relativeDirectory, storedName);
        var fullPath = Path.Combine(_rootPath, storedPath);

        await using var file = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true);
        await content.CopyToAsync(file, ct);

        return storedPath;
    }

    public async Task<Stream> OpenAsync(string storedPath, CancellationToken ct = default)
    {
        var fullPath = GetFullPath(storedPath);
        return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
    }

    public bool Exists(string storedPath) => File.Exists(GetFullPath(storedPath));

    public void Delete(string storedPath)
    {
        var fullPath = GetFullPath(storedPath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private string GetFullPath(string storedPath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, storedPath));
        if (!fullPath.StartsWith(_rootPath, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Stored path escapes the storage root.");
        return fullPath;
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName).Trim();
        return string.IsNullOrWhiteSpace(name) ? "document" : name;
    }
}

public sealed class DocumentStorageOptions
{
    public string RootPath { get; set; } = string.Empty;
}