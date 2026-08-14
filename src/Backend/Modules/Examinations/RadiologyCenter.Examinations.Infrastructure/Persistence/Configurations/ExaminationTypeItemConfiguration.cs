using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Examinations.Domain.Entities;

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Configurations;

public class ExaminationTypeItemConfiguration : IEntityTypeConfiguration<ExaminationTypeItem>
{
    public void Configure(EntityTypeBuilder<ExaminationTypeItem> builder)
    {
        builder.ToTable("ExaminationTypeItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.ExaminationTypeId).IsRequired();
        builder.Property(i => i.ItemId).IsRequired();
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.IsContrast).IsRequired();
        builder.Property(i => i.IsRequired).IsRequired();
        builder.Property(i => i.Notes).HasMaxLength(500);

        builder.HasIndex(i => i.ExaminationTypeId);
        builder.HasIndex(i => i.ItemId);
    }
}