using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Infrastructure.Persistence.Configurations;

public class InsurancePolicyConfiguration : IEntityTypeConfiguration<InsurancePolicy>
{
    public void Configure(EntityTypeBuilder<InsurancePolicy> builder)
    {
        builder.ToTable("InsurancePolicies");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.CompanyId).IsRequired();
        builder.Property(p => p.PatientId).IsRequired();
        builder.Property(p => p.PolicyNumber).IsRequired().HasMaxLength(100);
        builder.Property(p => p.CoveragePercent).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.EffectiveFrom).IsRequired();
        builder.Property(p => p.EffectiveTo);
        builder.Property(p => p.Status)
            .HasConversion(s => s.Value, v => PolicyStatus.FromValue<PolicyStatus>(v))
            .IsRequired();
        builder.Property(p => p.IsGovernment).IsRequired();

        builder.HasIndex(p => p.PolicyNumber).IsUnique();
        builder.HasIndex(p => p.PatientId);
        builder.HasIndex(p => p.CompanyId);

        builder.HasMany(p => p.Documents)
               .WithOne()
               .HasForeignKey(d => d.PolicyId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}