namespace RadiologyCenter.Insurance.Application.Abstractions;

public interface IInsuranceDocumentStorage
{
    string RootPath { get; }
    Task<string> SaveAsync(string relativeDirectory, string fileName, Stream content, CancellationToken ct = default);
    Task<Stream> OpenAsync(string storedPath, CancellationToken ct = default);
    bool Exists(string storedPath);
    void Delete(string storedPath);
}