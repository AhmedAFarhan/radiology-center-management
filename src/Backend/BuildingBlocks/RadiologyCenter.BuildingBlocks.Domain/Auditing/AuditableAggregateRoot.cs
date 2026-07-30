using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.BuildingBlocks.Domain.Auditing;

public abstract class AuditableAggregateRoot<TId> : AggregateRoot<TId>, IAuditable
    where TId : notnull
{
    public DateTime CreatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public DateTime? LastModifiedAt { get; protected set; }
    public string? LastModifiedBy { get; protected set; }

    protected AuditableAggregateRoot(TId id) : base(id) { }

    protected AuditableAggregateRoot() { }

    public void SetCreated(string? by, DateTime at)
    {
        CreatedBy = by;
        CreatedAt = at;
    }

    public void SetModified(string? by, DateTime at)
    {
        LastModifiedBy = by;
        LastModifiedAt = at;
    }
}
