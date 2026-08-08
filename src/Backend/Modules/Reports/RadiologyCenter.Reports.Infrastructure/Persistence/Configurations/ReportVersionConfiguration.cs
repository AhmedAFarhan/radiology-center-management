using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Reports.Domain.Entities;

namespace RadiologyCenter.Reports.Infrastructure.Persistence.Configurations;

public class ReportVersionConfiguration : IEntityTypeConfiguration<ReportVersion>
{
    public void Configure(EntityTypeBuilder<ReportVersion> builder)
    {
        builder.ToTable("ReportVersions");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedNever();

        builder.Property(v => v.ReportId).IsRequired();
        builder.Property(v => v.VersionNumber).IsRequired();
        builder.Property(v => v.AmendmentReason).HasMaxLength(1000);
        builder.Property(v => v.CreatedAt).IsRequired();

        builder.HasIndex(v => new { v.ReportId, v.VersionNumber }).IsUnique();

        builder.HasMany(v => v.Sections)
            .WithOne()
            .HasForeignKey(s => s.ReportVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Findings)
            .WithOne()
            .HasForeignKey(f => f.ReportVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(v => v.Sections).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(v => v.Findings).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}