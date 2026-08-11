namespace RadiologyCenter.Notification.Application.DTOs;

public record NotificationTemplateDto(
    Guid Id,
    string Code,
    string Name,
    string Subject,
    string Body,
    bool IsActive);