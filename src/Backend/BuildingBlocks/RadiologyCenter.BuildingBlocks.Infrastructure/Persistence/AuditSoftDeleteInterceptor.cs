using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RadiologyCenter.BuildingBlocks.Domain.Auditing;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;

public class AuditSoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        if (eventData.Context is not null)
        {
            ApplyAudit(eventData.Context);
            ApplySoftDelete(eventData.Context);
        }
        return base.SavingChangesAsync(eventData, result, ct);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            ApplyAudit(eventData.Context);
            ApplySoftDelete(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    private static void ApplyAudit(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<IAuditable>();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreated("system", DateTime.UtcNow);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetModified("system", DateTime.UtcNow);
                    entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false;
                    entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;
                    break;
            }
        }
    }

    private static void ApplySoftDelete(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<ISoftDeletable>();
        foreach (var entry in entries)
        {
            if (entry.State is EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.Delete("system");
            }
        }
    }
}
