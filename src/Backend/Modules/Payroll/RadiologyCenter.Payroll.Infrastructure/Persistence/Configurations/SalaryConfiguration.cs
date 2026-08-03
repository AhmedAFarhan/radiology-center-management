using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence.Configurations;

public class SalaryConfiguration : IEntityTypeConfiguration<Salary>
{
    public void Configure(EntityTypeBuilder<Salary> builder)
    {
        builder.ToTable("Salaries");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.StaffId).IsRequired();
        builder.Property(s => s.BaseSalary).HasPrecision(18, 2).IsRequired();
        builder.Property(s => s.SalaryType)
            .HasConversion(t => t.Value, v => SalaryType.FromValue<SalaryType>(v))
            .IsRequired();
        builder.Property(s => s.EffectiveDate).IsRequired();
        builder.Property(s => s.IsActive).IsRequired();

        builder.HasIndex(s => new { s.StaffId, s.IsActive }).HasFilter("[IsDeleted] = 0");
    }
}