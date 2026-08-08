using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Domain.Enumerations;

namespace RadiologyCenter.Insurance.Infrastructure.Persistence.Configurations;

public class PreAuthorizationDocumentConfiguration : IEntityTypeConfiguration<PreAuthorizationDocument>
{
    public void Configure(EntityTypeBuilder<PreAuthorizationDocument> builder)
    {
        builder.ToTable("PreAuthorizationDocuments");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.Property(d => d.PreAuthorizationId).IsRequired();
        builder.Property(d => d.Type)
            .HasConversion(t => t.Value, v => DocumentType.FromValue<DocumentType>(v))
            .IsRequired();
        builder.Property(d => d.FileName).HasMaxLength(255).IsRequired();
        builder.Property(d => d.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(d => d.StoredPath).HasMaxLength(500).IsRequired();
        builder.Property(d => d.SizeInBytes).IsRequired();
        builder.Property(d => d.UploadedAt).IsRequired();

        builder.HasIndex(d => d.PreAuthorizationId);
    }
}