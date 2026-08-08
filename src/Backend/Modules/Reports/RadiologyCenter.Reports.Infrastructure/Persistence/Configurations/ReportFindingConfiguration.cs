using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Reports.Domain.Entities;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Infrastructure.Persistence.Configurations;

public class ReportFindingConfiguration : IEntityTypeConfiguration<ReportFinding>
{
    public void Configure(EntityTypeBuilder<ReportFinding> builder)
    {
        builder.ToTable("ReportFindings");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedNever();

        builder.Property(f => f.ReportVersionId).IsRequired();
        builder.Property(f => f.Region).IsRequired().HasMaxLength(200);
        builder.Property(f => f.Description).IsRequired().HasMaxLength(5000);
        builder.Property(f => f.Severity)
            .HasConversion(s => s.Value, v => FindingSeverity.FromValue<FindingSeverity>(v));
        builder.Property(f => f.Position).IsRequired();

        builder.HasIndex(f => f.ReportVersionId);
    }
}