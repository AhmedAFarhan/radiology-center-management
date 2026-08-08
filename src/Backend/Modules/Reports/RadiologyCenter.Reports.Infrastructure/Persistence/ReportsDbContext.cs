using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Reports.Domain.Entities;

namespace RadiologyCenter.Reports.Infrastructure.Persistence;

public class ReportsDbContext : AppDbContext
{
    public DbSet<RadiologyReport> RadiologyReports => Set<RadiologyReport>();
    public DbSet<ReportVersion> ReportVersions => Set<ReportVersion>();
    public DbSet<ReportSection> ReportSections => Set<ReportSection>();
    public DbSet<ReportFinding> ReportFindings => Set<ReportFinding>();
    public DbSet<ReportTemplate> ReportTemplates => Set<ReportTemplate>();
    public DbSet<ReportTemplateSection> ReportTemplateSections => Set<ReportTemplateSection>();

    public ReportsDbContext(DbContextOptions<ReportsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Reports");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportsDbContext).Assembly);
    }
}