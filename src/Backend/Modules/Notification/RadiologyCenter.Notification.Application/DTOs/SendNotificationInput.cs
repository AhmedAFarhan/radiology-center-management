namespace RadiologyCenter.Notification.Application.DTOs;

public record SendNotificationInput(
    string Recipient,
    string Channel,
    string? TemplateCode = null,
    string? Subject = null,
    string? Body = null,
    Dictionary<string, string>? Placeholders = null,
    string? ReferenceId = null);