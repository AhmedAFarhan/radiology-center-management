namespace RadiologyCenter.Notification.Application.Commands.NotificationTemplates.CreateNotificationTemplate;

public record CreateNotificationTemplateCommand(
    string Code,
    string Name,
    string Subject,
    string Body) : ICommand;