using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Domain.Entities;

public sealed class PolicyDocument : Entity<Guid>
{
    public Guid PolicyId { get; private set; }
    public DocumentType Type { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public string StoredPath { get; private set; }
    public long SizeInBytes { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private PolicyDocument()
    {
        Type = null!;
        FileName = string.Empty;
        ContentType = string.Empty;
        StoredPath = string.Empty;
    }

    public static PolicyDocument Create(
        Guid policyId,
        DocumentType type,
        string fileName,
        string contentType,
        string storedPath,
        long sizeInBytes)
    {
        Guard.AgainstEmpty(policyId, nameof(policyId));
        Guard.AgainstNull(type, nameof(type));
        Guard.AgainstNullOrWhiteSpace(fileName, nameof(fileName));
        Guard.AgainstNullOrWhiteSpace(contentType, nameof(contentType));
        Guard.AgainstNullOrWhiteSpace(storedPath, nameof(storedPath));
        Guard.Against(sizeInBytes, s => s <= 0, "Document size must be greater than zero.");

        return new PolicyDocument
        {
            Id = Guid.NewGuid(),
            PolicyId = policyId,
            Type = type,
            FileName = fileName.Trim(),
            ContentType = contentType.Trim(),
            StoredPath = storedPath,
            SizeInBytes = sizeInBytes,
            UploadedAt = DateTime.UtcNow
        };
    }
}