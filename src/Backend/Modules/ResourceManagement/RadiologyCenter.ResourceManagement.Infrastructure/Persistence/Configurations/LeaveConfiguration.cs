using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.ResourceManagement.Domain.Entities;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence.Configurations;

public class LeaveConfiguration : IEntityTypeConfiguration<Leave>
{
    public void Configure(EntityTypeBuilder<Leave> builder)
    {
        builder.ToTable("Leaves");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.StaffId).IsRequired();
        builder.Property(l => l.StartDate).IsRequired();
        builder.Property(l => l.EndDate).IsRequired();
        builder.Property(l => l.Reason).HasMaxLength(500);

        builder.Property(l => l.LeaveType)
            .HasConversion(t => t.Value, v => LeaveType.FromValue<LeaveType>(v));

        builder.HasIndex(l => new { l.StaffId, l.StartDate });
    }
}
