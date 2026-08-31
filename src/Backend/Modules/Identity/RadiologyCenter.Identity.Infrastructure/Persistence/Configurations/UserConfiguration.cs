using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Identity.Domain.Entities;

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("AspNetUsers");

        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.TimeZoneId).HasMaxLength(100).IsRequired().HasDefaultValue("Africa/Cairo");
        builder.Property(u => u.ProfilePictureUrl).HasMaxLength(500);
        builder.Property(u => u.CreatedAt).IsRequired();

        builder.OwnsMany(u => u.RefreshTokens, rt =>
        {
            rt.WithOwner().HasForeignKey("UserId");
            rt.Property(r => r.Token).HasMaxLength(256).IsRequired();
            rt.Property(r => r.ExpiresAtUtc).IsRequired();
            rt.Property(r => r.CreatedAtUtc).IsRequired();
            rt.HasKey("UserId", nameof(RefreshToken.Token));
        });
        builder.Navigation(u => u.RefreshTokens).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(u => u.Sessions, s =>
        {
            s.WithOwner().HasForeignKey("UserId");
            s.HasKey("UserId", nameof(UserSession.Id));
            s.Property(x => x.Id).ValueGeneratedNever();
            s.Property(x => x.RefreshToken).HasMaxLength(256).IsRequired();
            s.HasIndex(nameof(UserSession.RefreshToken)).IsUnique();
            s.Property(x => x.StartedAtUtc).IsRequired();
            s.Property(x => x.LastActivityAtUtc).IsRequired();
        });
        builder.Navigation(u => u.Sessions).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(u => u.AssignedRoles)
            .WithMany()
            .UsingEntity("AspNetUserRoles");
        builder.Navigation(u => u.AssignedRoles).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
