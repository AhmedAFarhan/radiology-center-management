using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Cash.Domain.Entities;

namespace RadiologyCenter.Cash.Infrastructure.Persistence;

public class CashDbContext : AppDbContext
{
    public DbSet<CashSession> CashSessions => Set<CashSession>();
    public DbSet<CashEntry> CashEntries => Set<CashEntry>();
    public DbSet<CashHandover> CashHandovers => Set<CashHandover>();

    public CashDbContext(DbContextOptions<CashDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Cash");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CashDbContext).Assembly);
    }
}