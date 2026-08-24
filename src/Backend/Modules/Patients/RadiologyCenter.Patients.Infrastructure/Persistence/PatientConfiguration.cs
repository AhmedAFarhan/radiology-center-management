using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Patients.Domain.Entities;
using RadiologyCenter.Patients.Domain.Enumerations;

namespace RadiologyCenter.Patients.Infrastructure.Persistence;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PatientCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(p => p.PatientCode)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.MiddleName).HasMaxLength(100);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(30);
        builder.Property(p => p.Email).HasMaxLength(200);
        builder.Property(p => p.Address).HasMaxLength(300);
        builder.Property(p => p.NationalId).HasMaxLength(50);
        builder.Property(p => p.Allergies).HasMaxLength(1000);
        builder.Property(p => p.MedicalHistory).HasMaxLength(2000);

        builder.Property(p => p.Gender)
            .HasConversion(g => g.Value, v => Gender.FromValue<Gender>(v));

        builder.Property(p => p.BloodType)
            .HasConversion(
                b => b == null ? (int?)null : b.Value,
                v => v.HasValue ? BloodType.FromValue<BloodType>(v.Value) : null);

        builder.Property(p => p.Age);
    }
}
