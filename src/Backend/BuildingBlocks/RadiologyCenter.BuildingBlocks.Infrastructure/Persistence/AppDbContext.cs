using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Domain.Auditing;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplySoftDeleteFilter(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        ApplyAudit();
        ApplySoftDelete();
        return await base.SaveChangesAsync(ct);
    }

    private void ApplyAudit()
    {
        var entries = ChangeTracker.Entries<IAuditable>();
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

    private void ApplySoftDelete()
    {
        var entries = ChangeTracker.Entries<ISoftDeletable>();
        foreach (var entry in entries)
        {
            if (entry.State is EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.Delete("system");
            }
        }
    }

    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, [modelBuilder]);
            }
        }
    }

    private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDeletable
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }
}
