using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Persistence;
using RadiologyCenter.Payroll.Domain.Entities;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence;

public class PayrollDbContext : AppDbContext
{
    public DbSet<Salary> Salaries => Set<Salary>();
    public DbSet<SalaryComponent> SalaryComponents => Set<SalaryComponent>();
    public DbSet<AllowanceAssignment> AllowanceAssignments => Set<AllowanceAssignment>();
    public DbSet<ExaminationFee> ExaminationFees => Set<ExaminationFee>();
    public DbSet<ReferralFee> ReferralFees => Set<ReferralFee>();
    public DbSet<PayRun> PayRuns => Set<PayRun>();
    public DbSet<Payslip> Payslips => Set<Payslip>();
    public DbSet<PayslipComponent> PayslipComponents => Set<PayslipComponent>();
    public DbSet<ReferralFeeStatement> ReferralFeeStatements => Set<ReferralFeeStatement>();

    public PayrollDbContext(DbContextOptions<PayrollDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Payroll");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PayrollDbContext).Assembly);
    }
}
