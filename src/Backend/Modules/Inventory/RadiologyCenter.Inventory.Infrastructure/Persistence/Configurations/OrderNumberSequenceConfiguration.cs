using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Inventory.Infrastructure.Persistence;

namespace RadiologyCenter.Inventory.Infrastructure.Persistence.Configurations;

public class OrderNumberSequenceConfiguration : IEntityTypeConfiguration<OrderNumberSequence>
{
    public void Configure(EntityTypeBuilder<OrderNumberSequence> builder)
    {
        builder.ToTable("OrderNumberSequences");
        builder.HasKey(s => s.Year);
        builder.Property(s => s.Year).ValueGeneratedNever();
        builder.Property(s => s.LastNumber).IsRequired();
    }
}
