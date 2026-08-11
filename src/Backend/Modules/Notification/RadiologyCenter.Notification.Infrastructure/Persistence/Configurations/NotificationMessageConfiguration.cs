using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RadiologyCenter.Notification.Domain.Entities;
using RadiologyCenter.Notification.Domain.Enumerations;

namespace RadiologyCenter.Notification.Infrastructure.Persistence.Configurations;

public class NotificationMessageConfiguration : IEntityTypeConfiguration<NotificationMessage>
{
    public void Configure(EntityTypeBuilder<NotificationMessage> builder)
    {
        builder.ToTable("NotificationMessages");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.Recipient).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Channel)
            .HasConversion(c => c.Value, v => NotificationChannel.FromValue<NotificationChannel>(v))
            .IsRequired();
        builder.Property(m => m.Status)
            .HasConversion(s => s.Value, v => NotificationStatus.FromValue<NotificationStatus>(v))
            .IsRequired();
        builder.Property(m => m.Subject).HasMaxLength(400).IsRequired();
        builder.Property(m => m.Body).IsRequired();
        builder.Property(m => m.TemplateCode).HasMaxLength(100);
        builder.Property(m => m.ReferenceId).HasMaxLength(100);
        builder.Property(m => m.FailureReason).HasMaxLength(1000);

        builder.HasIndex(m => m.Channel);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.TemplateCode);
        builder.HasIndex(m => m.ReferenceId);
    }
}