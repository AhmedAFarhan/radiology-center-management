namespace RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    string? DeletedBy { get; }

    void Delete(string? by);
    void Restore();
}
