using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Configurations;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.ToTable("Visits");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.PatientId).IsRequired();
        builder.Property(v => v.AppointmentId);
        builder.Property(v => v.VisitedAt).IsRequired();
        builder.Property(v => v.Status)
            .HasConversion(s => s.Value, v => VisitStatus.FromValue<VisitStatus>(v));
        builder.Property(v => v.Notes).HasMaxLength(500);

        builder.HasIndex(v => v.PatientId);
        builder.HasIndex(v => v.VisitedAt);

        builder.HasMany(v => v.Examinations)
            .WithOne()
            .HasForeignKey(e => e.VisitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(v => v.Examinations).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
