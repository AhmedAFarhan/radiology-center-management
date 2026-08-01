using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Examinations.Domain.Entities;

namespace RadiologyCenter.Examinations.Infrastructure.Persistence;

public class ExaminationsDbContext : AppDbContext
{
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Examination> Examinations => Set<Examination>();
    public DbSet<ExaminationItem> ExaminationItems => Set<ExaminationItem>();
    public DbSet<ExaminationType> ExaminationTypes => Set<ExaminationType>();
    public DbSet<ExaminationTypeItem> ExaminationTypeItems => Set<ExaminationTypeItem>();

    public ExaminationsDbContext(DbContextOptions<ExaminationsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Examinations");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExaminationsDbContext).Assembly);
    }
}
