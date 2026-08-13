using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Inventory.Domain.Entities;

namespace RadiologyCenter.Inventory.Infrastructure.Persistence.Configurations;

public class StockBatchConfiguration : IEntityTypeConfiguration<StockBatch>
{
    public void Configure(EntityTypeBuilder<StockBatch> builder)
    {
        builder.ToTable("StockBatches", t => t.HasCheckConstraint("CK_StockBatches_QuantityRemaining_NonNegative", "[QuantityRemaining] >= 0"));

        builder.HasKey(b => b.Id);

        builder.Property(b => b.LotNumber).IsRequired().HasMaxLength(100);
        builder.Property(b => b.ExpiryDate);
        builder.Property(b => b.QuantityReceived).IsRequired();
        builder.Property(b => b.QuantityRemaining).IsRequired();
        builder.Property(b => b.RowVersion).IsRowVersion();

        builder.HasIndex(b => b.ItemId);
        builder.HasIndex(b => new { b.ItemId, b.QuantityRemaining });
    }
}
