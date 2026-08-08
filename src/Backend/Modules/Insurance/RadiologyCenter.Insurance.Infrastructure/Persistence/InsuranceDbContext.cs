using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Insurance.Domain.Entities;

namespace RadiologyCenter.Insurance.Infrastructure.Persistence;

public class InsuranceDbContext : AppDbContext
{
    public DbSet<InsuranceCompany> InsuranceCompanies => Set<InsuranceCompany>();
    public DbSet<InsurancePolicy> InsurancePolicies => Set<InsurancePolicy>();
    public DbSet<PreAuthorization> PreAuthorizations => Set<PreAuthorization>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<ClaimRejection> ClaimRejections => Set<ClaimRejection>();
    public DbSet<Settlement> Settlements => Set<Settlement>();

    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Insurance");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InsuranceDbContext).Assembly);
    }
}