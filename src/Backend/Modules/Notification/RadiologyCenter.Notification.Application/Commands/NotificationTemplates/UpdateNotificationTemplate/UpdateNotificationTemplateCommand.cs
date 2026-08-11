namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.UpdateNotificationTemplate;

public record UpdateNotificationTemplateCommand(
    Guid Id,
    string Code,
    string Name,
    string Subject,
    string Body) : ICommand;