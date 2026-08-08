using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Reports.Domain.Entities;
using RadiologyCenter.Reports.Domain.Enumerations;

namespace RadiologyCenter.Reports.Infrastructure.Persistence.Configurations;

public class RadiologyReportConfiguration : IEntityTypeConfiguration<RadiologyReport>
{
    public void Configure(EntityTypeBuilder<RadiologyReport> builder)
    {
        builder.ToTable("RadiologyReports");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.ExaminationId).IsRequired();
        builder.Property(r => r.PatientId).IsRequired();
        builder.Property(r => r.RadiologistId).IsRequired();
        builder.Property(r => r.Status)
            .HasConversion(s => s.Value, v => ReportStatus.FromValue<ReportStatus>(v));
        builder.Property(r => r.CurrentVersionNumber).IsRequired();
        builder.Property(r => r.FinalizedAt);
        builder.Property(r => r.CancelReason).HasMaxLength(1000);

        builder.HasIndex(r => r.ExaminationId).IsUnique();

        builder.HasMany(r => r.Versions)
            .WithOne()
            .HasForeignKey(v => v.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(r => r.Versions).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}