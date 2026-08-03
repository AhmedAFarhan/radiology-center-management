using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Payroll.Domain.Entities;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence;

public class PayrollDbContext : AppDbContext
{
    public DbSet<ExaminationFee> ExaminationFees => Set<ExaminationFee>();
    public DbSet<ReferralFee> ReferralFees => Set<ReferralFee>();

    public PayrollDbContext(DbContextOptions<PayrollDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Payroll");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PayrollDbContext).Assembly);
    }
}