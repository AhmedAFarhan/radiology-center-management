using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Payroll.Domain.Entities;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence.Configurations;

public class ReferralFeeConfiguration : IEntityTypeConfiguration<ReferralFee>
{
    public void Configure(EntityTypeBuilder<ReferralFee> builder)
    {
        builder.ToTable("ReferralFees");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.ReferralDoctorId).IsRequired();
        builder.Property(e => e.ExaminationTypeId).IsRequired();
        builder.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.IsPercentage).IsRequired();
        builder.Property(e => e.IsActive).IsRequired();

        builder.HasIndex(e => new { e.ReferralDoctorId, e.ExaminationTypeId }).HasFilter("[IsDeleted] = 0");
    }
}