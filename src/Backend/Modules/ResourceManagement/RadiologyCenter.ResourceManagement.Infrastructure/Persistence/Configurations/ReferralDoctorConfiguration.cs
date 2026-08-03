using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.ResourceManagement.Domain.Entities;

namespace RadiologyCenter.ResourceManagement.Infrastructure.Persistence.Configurations;

public class ReferralDoctorConfiguration : IEntityTypeConfiguration<ReferralDoctor>
{
    public void Configure(EntityTypeBuilder<ReferralDoctor> builder)
    {
        builder.ToTable("ReferralDoctors");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Phone).IsRequired().HasMaxLength(30);
        builder.Property(r => r.Email).HasMaxLength(200);
        builder.Property(r => r.Specialization).HasMaxLength(200);
        builder.Property(r => r.Hospital).HasMaxLength(200);
    }
}
