using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Infrastructure.Persistence.Configurations;

public class ExaminationTypeConfiguration : IEntityTypeConfiguration<ExaminationType>
{
    public void Configure(EntityTypeBuilder<ExaminationType> builder)
    {
        builder.ToTable("ExaminationTypes");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Code).IsRequired().HasMaxLength(20);
        builder.HasIndex(t => t.Code).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Modality)
            .HasConversion(m => m.Value, v => Modality.FromValue<Modality>(v));
        builder.Property(t => t.BodyPart).IsRequired().HasMaxLength(200);
        builder.Property(t => t.StandardDurationMinutes).IsRequired();
        builder.Property(t => t.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(t => t.RequiresPreparation).IsRequired();
        builder.Property(t => t.RequiresConsent).IsRequired();
        builder.Property(t => t.IsActive).IsRequired();

        builder.Ignore(t => t.RequiresContrast);

        builder.HasMany(t => t.Items)
            .WithOne()
            .HasForeignKey(i => i.ExaminationTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(t => t.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
