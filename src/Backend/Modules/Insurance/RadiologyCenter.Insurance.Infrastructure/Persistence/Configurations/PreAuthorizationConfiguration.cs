using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Infrastructure.Persistence.Configurations;

public class PreAuthorizationConfiguration : IEntityTypeConfiguration<PreAuthorization>
{
    public void Configure(EntityTypeBuilder<PreAuthorization> builder)
    {
        builder.ToTable("PreAuthorizations");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.ExaminationId).IsRequired();
        builder.Property(p => p.PatientId).IsRequired();
        builder.Property(p => p.PolicyId).IsRequired();
        builder.Property(p => p.EstimatedAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.Status)
            .HasConversion(s => s.Value, v => PreAuthorizationStatus.FromValue<PreAuthorizationStatus>(v))
            .IsRequired();
        builder.Property(p => p.RequestedAt).IsRequired();
        builder.Property(p => p.DecidedAt);
        builder.Property(p => p.ApprovedAmount).HasPrecision(18, 2);
        builder.Property(p => p.RejectionReason).HasMaxLength(1000);

        builder.HasIndex(p => p.ExaminationId).IsUnique();
        builder.HasIndex(p => p.PatientId);
    }
}