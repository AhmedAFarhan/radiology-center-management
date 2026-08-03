using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Payroll.Domain.Entities;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence.Configurations;

public class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
{
    public void Configure(EntityTypeBuilder<Payslip> builder)
    {
        builder.ToTable("Payslips");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.PayRunId).IsRequired();
        builder.Property(p => p.StaffId).IsRequired();
        builder.Property(p => p.GrossSalary).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.UnpaidLeaveDays).IsRequired();
        builder.Property(p => p.UnpaidLeaveDeduction).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.Notes).HasMaxLength(1000).IsRequired(false);

        builder.HasMany(p => p.Components)
            .WithOne()
            .HasForeignKey(c => c.PayslipId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.PayRunId, p.StaffId }).IsUnique();
    }
}