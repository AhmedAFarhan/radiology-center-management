using System.Text;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Domain.Enumerations;

namespace RadiologyCenter.Notification.Application.Commands.Notifications.SendNotification;

public static class SendNotificationCommandHandler
{
    public static async Task<Result> HandleAsync(
        SendNotificationCommand command,
        INotificationTemplateRepository templateRepository,
        INotificationMessageRepository messageRepository,
        INotificationUnitOfWork unitOfWork,
        INotificationSender sender,
        CancellationToken ct)
    {
        var channel = NotificationChannel.GetAll<NotificationChannel>().FirstOrDefault(c => c.Name == command.Channel);
        if (channel is null)
            return Result.Failure(Error.Validation("InvalidChannel", $"Channel '{command.Channel}' is not supported."));

        string subject;
        string body;

        if (!string.IsNullOrWhiteSpace(command.TemplateCode))
        {
            var template = await templateRepository.GetByCodeAsync(command.TemplateCode, ct);
            if (template is null)
                return Result.Failure(Error.NotFound("NotificationTemplate", command.TemplateCode));

            if (!template.IsActive)
                return Result.Failure(Error.Validation("InactiveTemplate", $"Template '{command.TemplateCode}' is inactive."));

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

        var message = NotificationMessage.Create(
            command.Recipient,
            channel,
            subject,
            body,
            command.TemplateCode,
            command.ReferenceId);

        await messageRepository.AddAsync(message, ct);

        var sendResult = await sender.SendAsync(channel, command.Recipient, subject, body, ct);

        if (sendResult.IsSuccess)
            message.MarkSent(DateTime.UtcNow);
        else
            message.MarkFailed(sendResult.Error?.Message ?? "Notification delivery failed.");

        await unitOfWork.SaveChangesAsync(ct);

        return sendResult.IsSuccess
            ? Result.Success()
            : Result.Failure(Error.Failure(sendResult.Error?.Message ?? "Notification delivery failed."));
    }

    private static string ApplyPlaceholders(string text, Dictionary<string, string>? placeholders)
    {
        if (placeholders is null || placeholders.Count == 0)
            return text;

        return placeholders.Aggregate(text, (current, kvp) =>
            current.Replace($"{{{kvp.Key}}}", kvp.Value, StringComparison.Ordinal));
    }
}