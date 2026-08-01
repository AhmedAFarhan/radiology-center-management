using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Configurations;

public class ExaminationConfiguration : IEntityTypeConfiguration<Examination>
{
    public void Configure(EntityTypeBuilder<Examination> builder)
    {
        builder.ToTable("Examinations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.VisitId).IsRequired();
        builder.Property(e => e.ExaminationTypeId).IsRequired();
        builder.Property(e => e.ReferringDoctor).IsRequired().HasMaxLength(200);
        builder.Property(e => e.ClinicalIndication).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.Priority)
            .HasConversion(p => p.Value, v => ExaminationPriority.FromValue<ExaminationPriority>(v));
        builder.Property(e => e.Status)
            .HasConversion(s => s.Value, v => ExaminationStatus.FromValue<ExaminationStatus>(v));
        builder.Property(e => e.ScheduledAt);
        builder.Property(e => e.StartedAt);
        builder.Property(e => e.CompletedAt);
        builder.Property(e => e.PerformedByUserId);
        builder.Property(e => e.Notes).HasMaxLength(500);
        builder.Property(e => e.CancellationReason).HasMaxLength(500);

        builder.HasIndex(e => e.VisitId);
        builder.HasIndex(e => e.ExaminationTypeId);
        builder.HasIndex(e => e.Status);

        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(i => i.ExaminationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
