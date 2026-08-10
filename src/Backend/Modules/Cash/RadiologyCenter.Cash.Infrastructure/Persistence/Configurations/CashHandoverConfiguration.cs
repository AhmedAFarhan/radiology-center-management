using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Cash.Domain.Entities;

namespace RadiologyCenter.Cash.Infrastructure.Persistence.Configurations;

public class CashHandoverConfiguration : IEntityTypeConfiguration<CashHandover>
{
    public void Configure(EntityTypeBuilder<CashHandover> builder)
    {
        builder.ToTable("CashHandovers");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        builder.Property(h => h.CashSessionId).IsRequired();
        builder.Property(h => h.ExpectedTotal).HasPrecision(18, 2).IsRequired();
        builder.Property(h => h.CountedTotal).HasPrecision(18, 2).IsRequired();
        builder.Property(h => h.OverShortAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(h => h.ClosedAt).IsRequired();
        builder.Property(h => h.ClosedByUserId).IsRequired();
        builder.Property(h => h.ApprovedByUserId).IsRequired(false);
        builder.Property(h => h.ApprovedAt).IsRequired(false);
        builder.Property(h => h.ReceivingCashSessionId).IsRequired(false);
        builder.Property(h => h.Notes).HasMaxLength(1000);

        builder.HasIndex(h => h.CashSessionId).IsUnique();
    }
}