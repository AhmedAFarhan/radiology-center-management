using Microsoft.EntityFrameworkCore;

namespace RadiologyCenter.Patients.Infrastructure.Persistence;

public class PatientNumberSequence
{
    public int Year { get; set; }
    public int LastNumber { get; set; }
}

public class PatientNumberSequenceConfiguration : IEntityTypeConfiguration<PatientNumberSequence>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<PatientNumberSequence> builder)
    {
        builder.ToTable("PatientNumberSequences");
        builder.HasKey(s => s.Year);
        builder.Property(s => s.Year).ValueGeneratedNever();
        builder.Property(s => s.LastNumber).IsRequired();
    }
}
