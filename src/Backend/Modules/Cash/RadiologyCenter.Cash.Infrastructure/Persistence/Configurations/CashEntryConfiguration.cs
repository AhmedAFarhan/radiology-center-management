using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Cash.Domain.Entities;
using RadiologyCenter.Cash.Domain.Enumerations;

namespace RadiologyCenter.Cash.Infrastructure.Persistence.Configurations;

public class CashEntryConfiguration : IEntityTypeConfiguration<CashEntry>
{
    public void Configure(EntityTypeBuilder<CashEntry> builder)
    {
        builder.ToTable("CashEntries");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.CashSessionId).IsRequired();
        builder.Property(e => e.Direction)
            .HasConversion(e => e.Value, v => CashEntryDirection.FromValue<CashEntryDirection>(v))
            .IsRequired();
        builder.Property(e => e.Reason)
            .HasConversion(e => e.Value, v => CashEntryReason.FromValue<CashEntryReason>(v))
            .IsRequired();
        builder.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.ReferenceId).HasMaxLength(100);
        builder.Property(e => e.OccurredAt).IsRequired();

        builder.HasIndex(e => e.CashSessionId);
    }
}