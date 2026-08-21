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

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.PatientId).IsRequired();
        builder.Property(e => e.ExaminationTypeId).IsRequired();
        builder.Property(e => e.ReferralDoctorId);
        builder.Property(e => e.RadiologistId);
        builder.Property(e => e.TechnicianId);
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
        builder.Property(e => e.StudyInstanceUID).HasMaxLength(64);
        builder.Property(e => e.AccessionNumber).HasMaxLength(64);
        builder.Property(e => e.ImagesReceivedAt);
        builder.Property(e => e.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.Discount).HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.IsDiscountPercentage).IsRequired();
        builder.Property(e => e.Paid).HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.Remaining).HasPrecision(18, 2).IsRequired();

        builder.HasIndex(e => e.PatientId);
        builder.HasIndex(e => e.ExaminationTypeId);
        builder.HasIndex(e => e.Status);

        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(i => i.ExaminationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
