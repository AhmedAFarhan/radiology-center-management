using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence.Configurations;

public class ExaminationFeeConfiguration : IEntityTypeConfiguration<ExaminationFee>
{
    public void Configure(EntityTypeBuilder<ExaminationFee> builder)
    {
        builder.ToTable("ExaminationFees");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.ExaminationTypeId).IsRequired();
        builder.Property(e => e.Role)
            .HasConversion(r => r.Value, v => ExamFeeRole.FromValue<ExamFeeRole>(v))
            .IsRequired();
        builder.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.IsPercentage).IsRequired();
        builder.Property(e => e.IsActive).IsRequired();

        builder.HasIndex(e => new { e.ExaminationTypeId, e.Role }).HasFilter("[IsDeleted] = 0");
    }
}