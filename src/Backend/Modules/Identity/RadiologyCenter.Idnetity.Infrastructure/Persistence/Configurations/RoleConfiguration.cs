using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Idnetity.Domain.Entities;

namespace RadiologyCenter.Idnetity.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("AspNetRoles");

        builder.Property(r => r.Description).HasMaxLength(500);
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity("RolePermissions");
        builder.Navigation(r => r.Permissions).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
