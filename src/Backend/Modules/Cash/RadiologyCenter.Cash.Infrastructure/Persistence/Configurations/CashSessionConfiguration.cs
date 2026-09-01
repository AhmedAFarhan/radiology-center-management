using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Cash.Domain.Entities;
using RadiologyCenter.Cash.Domain.Enumerations;

namespace RadiologyCenter.Cash.Infrastructure.Persistence.Configurations;

public class CashSessionConfiguration : IEntityTypeConfiguration<CashSession>
{
    public void Configure(EntityTypeBuilder<CashSession> builder)
    {
        builder.ToTable("CashSessions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.WorkShiftId).IsRequired(false);
        builder.Property(s => s.OpeningFloat).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.Status)
            .HasConversion(s => s.Value, v => CashSessionStatus.FromValue<CashSessionStatus>(v))
            .IsRequired();
        builder.Property(s => s.OpenedAt).IsRequired();
        builder.Property(s => s.ClosedAt).IsRequired(false);
        builder.Property(s => s.Notes).HasMaxLength(1000);

        builder.Property(s => s.RowVersion).IsRowVersion();

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.Status);
    }
}