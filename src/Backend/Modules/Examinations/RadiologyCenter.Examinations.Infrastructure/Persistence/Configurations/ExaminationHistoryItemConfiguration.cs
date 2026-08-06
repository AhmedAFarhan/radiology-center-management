using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Examinations.Domain.Entities;

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Configurations;

public class ExaminationHistoryItemConfiguration : IEntityTypeConfiguration<ExaminationHistoryItem>
{
    public void Configure(EntityTypeBuilder<ExaminationHistoryItem> builder)
    {
        builder.ToTable("ExaminationHistoryItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ExaminationHistoryId).IsRequired();
        builder.Property(i => i.ItemId).IsRequired();
        builder.Property(i => i.ItemName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.ItemCategory).IsRequired();
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.UnitCost).HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.IsContrast).IsRequired();
        builder.Property(i => i.IsRequired).IsRequired();
        builder.Property(i => i.Notes).HasMaxLength(500);

        builder.HasIndex(i => i.ExaminationHistoryId);
        builder.HasIndex(i => i.ItemId);
        builder.HasIndex(i => new { i.ExaminationHistoryId, i.ItemId }).IsUnique();
    }
}
