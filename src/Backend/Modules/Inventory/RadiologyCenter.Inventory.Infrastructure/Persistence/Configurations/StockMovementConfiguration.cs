using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Infrastructure.Persistence.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Quantity).IsRequired();
        builder.Property(m => m.UnitCost).HasPrecision(18, 2);
        builder.Property(m => m.Reference).HasMaxLength(100);
        builder.Property(m => m.Notes).HasMaxLength(500);

        builder.Property(m => m.MovementType)
            .HasConversion(t => t.Value, v => StockMovementType.FromValue<StockMovementType>(v));

        builder.HasIndex(m => m.ItemId);
        builder.HasIndex(m => m.StockBatchId);
    }
}
