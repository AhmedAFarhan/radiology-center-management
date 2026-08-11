using RadiologyCenter.Notification.Domain.Entities;

namespace RadiologyCenter.Notification.Application.DTOs;

internal static class NotificationMapper
{
    public static NotificationTemplateDto ToDto(this NotificationTemplate template) =>
        new(
            template.Id,
            template.Code,
            template.Name,
            template.Subject,
            template.Body,
            template.IsActive);

    public static NotificationMessageDto ToDto(this NotificationMessage message) =>
        new(
            message.Id,
            message.Recipient,
            message.Channel.Name,
            message.Status.Name,
            message.Subject,
            message.Body,
            message.TemplateCode,
            message.ReferenceId,
            message.Attempts,
            message.SentAtUtc,
            message.FailureReason,
            message.CreatedAt);
}