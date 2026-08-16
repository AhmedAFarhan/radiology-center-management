using RadiologyCenter.Notification.Application.Localization;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Application.DTOs;

namespace RadiologyCenter.Notification.Application.Queries.NotificationMessages.PreviewNotification;

public static class PreviewNotificationQueryHandler
{
    public static async Task<Result<NotificationMessageDto>> HandleAsync(
        PreviewNotificationCommand query,
        INotificationTemplateRepository templateRepository,
        CancellationToken ct)
    {
        var command = query.Command;

        string subject;
        string body;

        if (!string.IsNullOrWhiteSpace(command.TemplateCode))
        {
            var template = await templateRepository.GetByCodeAsync(command.TemplateCode, ct);
            if (template is null)
                return Result.Failure<NotificationMessageDto>(Error.NotFound(ErrorCodes.TemplateNotFound, "NotificationTemplate", command.TemplateCode));

            subject = string.IsNullOrWhiteSpace(command.Subject) ? template.Subject : command.Subject;
            body = string.IsNullOrWhiteSpace(command.Body) ? template.Body : command.Body;
        }
        else
        {
            subject = command.Subject ?? string.Empty;
            body = command.Body ?? string.Empty;
        }

        subject = ApplyPlaceholders(subject, command.Placeholders);
        body = ApplyPlaceholders(body, command.Placeholders);

        var preview = new NotificationMessageDto(
            Guid.Empty,
            command.Recipient,
            command.Channel,
            "Preview",
            subject,
            body,
            command.TemplateCode,
            command.ReferenceId,
            0,
            null,
            null,
            DateTime.UtcNow);

        return Result.Success(preview);
    }

    private static string ApplyPlaceholders(string text, Dictionary<string, string>? placeholders)
    {
        if (placeholders is null || placeholders.Count == 0)
            return text;

        return placeholders.Aggregate(text, (current, kvp) =>
            current.Replace($"{{{kvp.Key}}}", kvp.Value, StringComparison.Ordinal));
    }
}