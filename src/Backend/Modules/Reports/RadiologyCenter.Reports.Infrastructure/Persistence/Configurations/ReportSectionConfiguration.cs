using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Reports.Domain.Entities;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Infrastructure.Persistence.Configurations;

public class ReportSectionConfiguration : IEntityTypeConfiguration<ReportSection>
{
    public void Configure(EntityTypeBuilder<ReportSection> builder)
    {
        builder.ToTable("ReportSections");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.ReportVersionId).IsRequired();
        builder.Property(s => s.SectionType)
            .HasConversion(t => t.Value, v => ReportSectionType.FromValue<ReportSectionType>(v));
        builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Body).IsRequired().HasMaxLength(10000);
        builder.Property(s => s.Position).IsRequired();
        builder.Property(s => s.IsLocked).IsRequired();

        builder.HasIndex(s => s.ReportVersionId);
    }
}