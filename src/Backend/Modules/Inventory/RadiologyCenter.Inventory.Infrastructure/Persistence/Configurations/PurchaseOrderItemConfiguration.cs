using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Inventory.Domain.Entities;

namespace RadiologyCenter.Inventory.Infrastructure.Persistence.Configurations;

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.QuantityOrdered).IsRequired();
        builder.Property(i => i.UnitCost).HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.QuantityReceived).IsRequired();

        builder.HasIndex(i => i.PurchaseOrderId);
    }
}
