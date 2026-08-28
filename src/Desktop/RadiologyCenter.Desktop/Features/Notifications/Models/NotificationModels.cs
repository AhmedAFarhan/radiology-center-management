namespace RadiologyCenter.Desktop.Features.Notifications.Models;

public sealed record NotificationTemplateDto(
    string Id,
    string Code,
    string Name,
    string Subject,
    string Body,
    bool IsActive);

public sealed class NotificationTemplateInput
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public sealed record NotificationMessageDto(
    string Id,
    string Recipient,
    string Channel,
    string Status,
    string Subject,
    string Body,
    string? TemplateCode,
    string? ReferenceId,
    int Attempts,
    DateTime? SentAtUtc,
    string? FailureReason,
    DateTime CreatedAt,
    string ChannelKey = "",
    string StatusKey = "");

public sealed class SendNotificationInput
{
    public string Recipient { get; set; } = string.Empty;
    public string Channel { get; set; } = "Sms";
    public string? TemplateCode { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public Dictionary<string, string>? Placeholders { get; set; }
    public string? ReferenceId { get; set; }
}

public sealed record NotificationPreviewDto(
    string Id,
    string Recipient,
    string Channel,
    string Status,
    string Subject,
    string Body,
    string? TemplateCode,
    string? ReferenceId,
    int Attempts,
    DateTime? SentAtUtc,
    string? FailureReason,
    DateTime CreatedAt);