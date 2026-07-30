using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Idnetity.Domain;
using RadiologyCenter.Idnetity.Domain.Entities;

namespace RadiologyCenter.Idnetity.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("AspNetPermissions");

        builder.Property(p => p.Code).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Group).HasMaxLength(100);

        builder.HasIndex(p => p.Code).IsUnique();

        builder.HasData(Permissions.All);
    }
}
