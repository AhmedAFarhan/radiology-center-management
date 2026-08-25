using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Payroll.Domain.Entities;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence.Configurations;

public class ReferralFeeStatementConfiguration : IEntityTypeConfiguration<ReferralFeeStatement>
{
    public void Configure(EntityTypeBuilder<ReferralFeeStatement> builder)
    {
        builder.ToTable("ReferralFeeStatements");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.PayRunId).IsRequired();
        builder.Property(s => s.ReferralDoctorId).IsRequired();
        builder.Property(s => s.TotalFee).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.ExamCount).IsRequired();

        builder.HasIndex(s => new { s.PayRunId, s.ReferralDoctorId }).IsUnique();
    }
}
