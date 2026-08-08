using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Infrastructure.Persistence.Configurations;

public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
{
    public void Configure(EntityTypeBuilder<Settlement> builder)
    {
        builder.ToTable("Settlements");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.ClaimId).IsRequired();
        builder.Property(s => s.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.Method)
            .HasConversion(m => m.Value, v => SettlementMethod.FromValue<SettlementMethod>(v))
            .IsRequired();
        builder.Property(s => s.SettledAt).IsRequired();
        builder.Property(s => s.Reference).HasMaxLength(200);

        builder.HasIndex(s => s.ClaimId);
    }
}