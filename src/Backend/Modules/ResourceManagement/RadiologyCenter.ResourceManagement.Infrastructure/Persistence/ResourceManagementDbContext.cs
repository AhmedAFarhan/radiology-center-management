using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.ResourceManagement.Domain.Entities;

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence;

public class ResourceManagementDbContext : AppDbContext
{
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<WorkShift> WorkShifts => Set<WorkShift>();
    public DbSet<Leave> Leaves => Set<Leave>();
    public DbSet<ReferralDoctor> ReferralDoctors => Set<ReferralDoctor>();

    public ResourceManagementDbContext(DbContextOptions<ResourceManagementDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ResourceManagement");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResourceManagementDbContext).Assembly);
    }
}
