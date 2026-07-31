using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Patients.Domain.Entities;

namespace RadiologyCenter.Patients.Infrastructure.Persistence;

public class PatientsDbContext : AppDbContext
{
    public DbSet<Patient> Patients => Set<Patient>();

    public PatientsDbContext(DbContextOptions<PatientsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Patients");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PatientsDbContext).Assembly);
    }
}
