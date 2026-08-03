using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Payroll.Domain.Entities;

namespace RadiologyCenter.Payroll.Infrastructure.Persistence.Configurations;

public class PayslipComponentConfiguration : IEntityTypeConfiguration<PayslipComponent>
{
    public void Configure(EntityTypeBuilder<PayslipComponent> builder)
    {
        builder.ToTable("PayslipComponents");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.PayslipId).IsRequired();
        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.IsDeduction).IsRequired();
    }
}