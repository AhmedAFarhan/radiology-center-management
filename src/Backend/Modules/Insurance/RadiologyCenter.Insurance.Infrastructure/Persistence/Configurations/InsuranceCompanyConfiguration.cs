using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Insurance.Domain.Entities;

namespace RadiologyCenter.Insurance.Infrastructure.Persistence.Configurations;

public class InsuranceCompanyConfiguration : IEntityTypeConfiguration<InsuranceCompany>
{
    public void Configure(EntityTypeBuilder<InsuranceCompany> builder)
    {
        builder.ToTable("InsuranceCompanies");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.TaxId).HasMaxLength(50);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.Phone).HasMaxLength(50);
        builder.Property(c => c.Email).HasMaxLength(200);

        builder.HasIndex(c => c.Name).IsUnique();
    }
}