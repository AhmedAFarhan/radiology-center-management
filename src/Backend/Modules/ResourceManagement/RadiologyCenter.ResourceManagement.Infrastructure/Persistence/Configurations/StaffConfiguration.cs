using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.ResourceManagement.Domain.Entities;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence.Configurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.EmployeeNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(s => s.EmployeeNumber).IsUnique();
        builder.Property(s => s.PhoneNumber).IsRequired().HasMaxLength(30);
        builder.Property(s => s.Department).HasMaxLength(200);
        builder.Property(s => s.Specialization).HasMaxLength(200);
        builder.Property(s => s.LicenseNumber).HasMaxLength(100);
        builder.Property(s => s.HireDate).IsRequired();

        builder.Property(s => s.Position)
            .HasConversion(p => p.Value, v => StaffPosition.FromValue<StaffPosition>(v));
    }
}
