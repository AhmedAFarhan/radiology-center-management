using RadiologyCenter.BuildingBlocks.Domain.Auditing;

namespace RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;

public abstract class SoftDeletableAggregateRoot<TId> : AuditableAggregateRoot<TId>, ISoftDeletable
    where TId : notnull
{
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public string? DeletedBy { get; protected set; }

    protected SoftDeletableAggregateRoot(TId id) : base(id) { }

    protected SoftDeletableAggregateRoot() { }

    public void Delete(string? by)
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = by;
    }

    public void Restore()
    {
        if (!IsDeleted) return;
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
