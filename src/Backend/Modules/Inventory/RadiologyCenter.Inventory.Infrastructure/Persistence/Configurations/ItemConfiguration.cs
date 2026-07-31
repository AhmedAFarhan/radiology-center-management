using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(i => i.Name).IsUnique();

        builder.Property(i => i.Brand).HasMaxLength(200);
        builder.Property(i => i.ReorderLevel).IsRequired();
        builder.Property(i => i.ReorderQuantity).IsRequired();
        builder.Property(i => i.LotTracked).IsRequired();
        builder.Property(i => i.StorageInstructions).HasMaxLength(500);

        builder.Property(i => i.Category)
            .HasConversion(c => c.Value, v => ItemCategory.FromValue<ItemCategory>(v));

        builder.Property(i => i.Unit)
            .HasConversion(u => u.Value, v => UnitType.FromValue<UnitType>(v));
    }
}
