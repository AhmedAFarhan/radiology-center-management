using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Identity.Domain.Entities;

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Configurations;

public class PermissionTranslationConfiguration : IEntityTypeConfiguration<PermissionTranslation>
{
    public void Configure(EntityTypeBuilder<PermissionTranslation> builder)
    {
        builder.ToTable("PermissionTranslations");

        builder.Property(t => t.Language).HasMaxLength(10).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.Group).HasMaxLength(100);

        builder.HasIndex(t => new { t.PermissionId, t.Language }).IsUnique();
    }
}