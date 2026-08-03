using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.ResourceManagement.Domain.Entities;

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence.Configurations;

public class WorkShiftConfiguration : IEntityTypeConfiguration<WorkShift>
{
    public void Configure(EntityTypeBuilder<WorkShift> builder)
    {
        builder.ToTable("WorkShifts");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.StaffId).IsRequired();
        builder.Property(w => w.Date).IsRequired();
        builder.Property(w => w.StartTime).IsRequired();
        builder.Property(w => w.EndTime).IsRequired();
        builder.Property(w => w.Notes).HasMaxLength(500);

        builder.HasIndex(w => new { w.StaffId, w.Date });
    }
}
