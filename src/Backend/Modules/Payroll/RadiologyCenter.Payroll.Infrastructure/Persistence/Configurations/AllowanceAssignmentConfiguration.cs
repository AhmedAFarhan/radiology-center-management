using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence.Configurations;

public class AllowanceAssignmentConfiguration : IEntityTypeConfiguration<AllowanceAssignment>
{
    public void Configure(EntityTypeBuilder<AllowanceAssignment> builder)
    {
        builder.ToTable("AllowanceAssignments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.StaffId).IsRequired();
        builder.Property(a => a.SalaryComponentId).IsRequired(false);
        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(a => a.Frequency)
            .HasConversion(f => f!.Value, v => Frequency.FromValue<Frequency>(v))
            .IsRequired(false);
        builder.Property(a => a.IsPerWorkDay).IsRequired();
        builder.Property(a => a.EffectiveDate).IsRequired();
        builder.Property(a => a.EndDate).IsRequired(false);
        builder.Property(a => a.IsActive).IsRequired();

        builder.HasIndex(a => new { a.StaffId, a.IsActive }).HasFilter("[IsDeleted] = 0");
    }
}