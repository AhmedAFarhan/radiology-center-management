using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Configurations;

public class ExaminationHistoryConfiguration : IEntityTypeConfiguration<ExaminationHistory>
{
    public void Configure(EntityTypeBuilder<ExaminationHistory> builder)
    {
        builder.ToTable("ExaminationHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.ExaminationTypeId).IsRequired();
        builder.Property(h => h.TypeCode).IsRequired().HasMaxLength(20);
        builder.Property(h => h.TypeName).IsRequired().HasMaxLength(200);
        builder.Property(h => h.TypeModality)
            .HasConversion(m => m.Value, v => Modality.FromValue<Modality>(v));
        builder.Property(h => h.TypeBodyPart).IsRequired().HasMaxLength(200);
        builder.Property(h => h.TypePrice).HasPrecision(18, 2).IsRequired();
        builder.Property(h => h.TypeStandardDurationMinutes).IsRequired();
        builder.Property(h => h.ReferralDoctorId);
        builder.Property(h => h.RadiologistId).IsRequired();
        builder.Property(h => h.TechnicianId).IsRequired();
        builder.Property(h => h.ClinicalIndication).IsRequired().HasMaxLength(1000);
        builder.Property(h => h.Priority)
            .HasConversion(p => p.Value, v => ExaminationPriority.FromValue<ExaminationPriority>(v));
        builder.Property(h => h.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(h => h.Discount).HasPrecision(18, 2).IsRequired();
        builder.Property(h => h.IsDiscountPercentage).IsRequired();
        builder.Property(h => h.Paid).HasPrecision(18, 2).IsRequired();
        builder.Property(h => h.Remaining).HasPrecision(18, 2).IsRequired();
        builder.Property(h => h.ScheduledAt);
        builder.Property(h => h.StartedAt);
        builder.Property(h => h.CompletedAt);
        builder.Property(h => h.PerformedByUserId);
        builder.Property(h => h.Notes).HasMaxLength(500);
        builder.Property(h => h.CancellationReason).HasMaxLength(500);

        builder.HasIndex(h => h.ExaminationTypeId);
        builder.HasIndex(h => h.CompletedAt);

        builder.HasMany(h => h.Items)
            .WithOne()
            .HasForeignKey(i => i.ExaminationHistoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(h => h.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
