using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Notification.Domain.Enumerations;

namespace RadiologyCenter.Notification.Domain.Entities;

public sealed class NotificationMessage : SoftDeletableAggregateRoot<Guid>
{
    public string Recipient { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public NotificationStatus Status { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public string? TemplateCode { get; private set; }
    public string? ReferenceId { get; private set; }
    public int Attempts { get; private set; }
    public DateTime? SentAtUtc { get; private set; }
    public string? FailureReason { get; private set; }

    private NotificationMessage()
    {
        Recipient = null!;
        Channel = null!;
        Status = null!;
        Subject = null!;
        Body = null!;
    }

    public static NotificationMessage Create(
        string recipient,
        NotificationChannel channel,
        string subject,
        string body,
        string? templateCode = null,
        string? referenceId = null)
    {
        Guard.AgainstNullOrWhiteSpace(recipient, nameof(recipient));
        Guard.AgainstNull(channel, nameof(channel));

        return new NotificationMessage
        {
            Id = Guid.NewGuid(),
            Recipient = recipient.Trim(),
            Channel = channel,
            Status = NotificationStatus.Pending,
            Subject = subject.Trim(),
            Body = body.Trim(),
            TemplateCode = templateCode,
            ReferenceId = referenceId,
            Attempts = 0
        };
    }

    public void MarkQueued()
    {
        Status = NotificationStatus.Pending;
    }

    public void MarkSent(DateTime sentAtUtc)
    {
        Attempts++;
        Status = NotificationStatus.Sent;
        SentAtUtc = sentAtUtc;
        FailureReason = null;
    }

    public void MarkFailed(string reason)
    {
        Attempts++;
        Status = NotificationStatus.Failed;
        FailureReason = reason;
    }
}