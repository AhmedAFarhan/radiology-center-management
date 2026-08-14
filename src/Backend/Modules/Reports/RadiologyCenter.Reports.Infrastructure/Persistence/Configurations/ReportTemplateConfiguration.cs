using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Catalog.Domain.Enumerations;
using RadiologyCenter.Reports.Domain.Entities;

namespace RadiologyCenter.Reports.Infrastructure.Persistence.Configurations;

public class ReportTemplateConfiguration : IEntityTypeConfiguration<ReportTemplate>
{
    public void Configure(EntityTypeBuilder<ReportTemplate> builder)
    {
        builder.ToTable("ReportTemplates");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(t => t.Name).IsUnique().HasFilter("[IsDeleted] = 0");

        builder.Property(t => t.Modality)
            .HasConversion(m => m.Value, v => Modality.FromValue<Modality>(v));
        builder.Property(t => t.BodyPart).HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.IsActive).IsRequired();
        builder.Property(t => t.IsSystem).IsRequired();
        builder.Property(t => t.UseCount).IsRequired();

        builder.HasMany(t => t.Sections)
            .WithOne()
            .HasForeignKey(s => s.ReportTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(t => t.Sections).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}