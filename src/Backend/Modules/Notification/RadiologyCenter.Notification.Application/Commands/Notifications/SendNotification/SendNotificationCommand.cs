namespace RadiologyCenter.Notification.Application.Commands.Notifications.SendNotification;

public record SendNotificationCommand(
    string Recipient,
    string Channel,
    string? TemplateCode = null,
    string? Subject = null,
    string? Body = null,
    Dictionary<string, string>? Placeholders = null,
    string? ReferenceId = null) : ICommand;