using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Infrastructure.Persistence.Configurations;

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.ExaminationId).IsRequired();
        builder.Property(c => c.PatientId).IsRequired();
        builder.Property(c => c.PolicyId).IsRequired();
        builder.Property(c => c.PreAuthorizationId);
        builder.Property(c => c.BilledAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.PayerShare).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.PatientShare).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.CopayApplied).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.Status)
            .HasConversion(s => s.Value, v => ClaimStatus.FromValue<ClaimStatus>(v))
            .IsRequired();
        builder.Property(c => c.SubmittedAt);
        builder.Property(c => c.ApprovedAt);
        builder.Property(c => c.PaidAt);

        builder.HasIndex(c => c.ExaminationId).IsUnique();
        builder.HasIndex(c => c.PatientId);
        builder.HasIndex(c => c.PolicyId);

        builder.HasMany(c => c.Settlements)
            .WithOne()
            .HasForeignKey(s => s.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Rejections)
            .WithOne()
            .HasForeignKey(r => r.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Settlements).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(c => c.Rejections).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}