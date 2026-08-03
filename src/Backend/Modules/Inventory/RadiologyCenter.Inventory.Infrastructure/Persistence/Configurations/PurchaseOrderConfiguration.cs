using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Infrastructure.Persistence.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.OrderNumber).IsRequired().HasMaxLength(20);
        builder.HasIndex(p => p.OrderNumber).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.Property(p => p.Status)
            .HasConversion(s => s.Value, v => PurchaseOrderStatus.FromValue<PurchaseOrderStatus>(v));

        builder.Property(p => p.Notes).HasMaxLength(500);

        builder.HasIndex(p => p.SupplierId);

        builder.HasMany(p => p.Items)
            .WithOne()
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
