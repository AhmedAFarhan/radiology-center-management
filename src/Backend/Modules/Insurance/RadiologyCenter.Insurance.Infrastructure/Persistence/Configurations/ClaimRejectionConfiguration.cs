using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Infrastructure.Persistence.Configurations;

public class ClaimRejectionConfiguration : IEntityTypeConfiguration<ClaimRejection>
{
    public void Configure(EntityTypeBuilder<ClaimRejection> builder)
    {
        builder.ToTable("ClaimRejections");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.ClaimId).IsRequired();
        builder.Property(r => r.Code)
            .HasConversion(c => c.Value, v => ClaimRejectionCode.FromValue<ClaimRejectionCode>(v))
            .IsRequired();
        builder.Property(r => r.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(r => r.RejectedAt).IsRequired();

        builder.HasIndex(r => r.ClaimId);
    }
}