using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence.Configurations;

public class PayRunConfiguration : IEntityTypeConfiguration<PayRun>
{
    public void Configure(EntityTypeBuilder<PayRun> builder)
    {
        builder.ToTable("PayRuns");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.RunFrom).IsRequired();
        builder.Property(p => p.RunTo).IsRequired();
        builder.Property(p => p.Status)
            .HasConversion(s => s.Value, v => PayRunStatus.FromValue<PayRunStatus>(v))
            .IsRequired();
        builder.Property(p => p.ProcessedBy).HasMaxLength(200).IsRequired(false);
        builder.Property(p => p.ProcessedAt).IsRequired(false);
        builder.Property(p => p.Notes).HasMaxLength(1000).IsRequired(false);

        builder.HasMany(p => p.Payslips)
            .WithOne()
            .HasForeignKey(ps => ps.PayRunId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}