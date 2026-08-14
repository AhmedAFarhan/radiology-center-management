using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Catalog.Domain.Entities;

namespace RadiologyCenter.Catalog.Infrastructure.Persistence;

public class CatalogDbContext : AppDbContext
{
    public DbSet<ExaminationType> ExaminationTypes => Set<ExaminationType>();
    public DbSet<ExaminationTypeItem> ExaminationTypeItems => Set<ExaminationTypeItem>();

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Catalog");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}