using RadiologyCenter.BuildingBlocks.Domain.Entities;

namespace RadiologyCenter.BuildingBlocks.Domain.Auditing;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditable
    where TId : notnull
{
    public DateTime CreatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public DateTime? LastModifiedAt { get; protected set; }
    public string? LastModifiedBy { get; protected set; }

    protected AuditableEntity(TId id) : base(id) { }

    protected AuditableEntity() { }

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
