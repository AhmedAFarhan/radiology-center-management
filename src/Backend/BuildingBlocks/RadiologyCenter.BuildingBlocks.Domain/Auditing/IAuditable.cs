namespace RadiologyCenter.BuildingBlocks.Domain.Auditing;

public interface IAuditable
{
    DateTime CreatedAt { get; }
    string? CreatedBy { get; }
    DateTime? LastModifiedAt { get; }
    string? LastModifiedBy { get; }

    void SetCreated(string? by, DateTime at);
    void SetModified(string? by, DateTime at);
}
