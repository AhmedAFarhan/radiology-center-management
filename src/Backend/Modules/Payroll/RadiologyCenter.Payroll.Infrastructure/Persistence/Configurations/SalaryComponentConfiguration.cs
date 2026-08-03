using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Domain.Enumerations;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence.Configurations;

public class SalaryComponentConfiguration : IEntityTypeConfiguration<SalaryComponent>
{
    public void Configure(EntityTypeBuilder<SalaryComponent> builder)
    {
        builder.ToTable("SalaryComponents");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Kind)
            .HasConversion(k => k.Value, v => ComponentKind.FromValue<ComponentKind>(v))
            .IsRequired();
        builder.Property(c => c.Frequency)
            .HasConversion(f => f!.Value, v => Frequency.FromValue<Frequency>(v))
            .IsRequired(false);
        builder.Property(c => c.IsPercentage).IsRequired();
        builder.Property(c => c.IsPerWorkDay).IsRequired();
        builder.Property(c => c.DefaultValue).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();
    }
}