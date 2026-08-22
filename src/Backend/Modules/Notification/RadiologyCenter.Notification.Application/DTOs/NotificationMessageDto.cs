namespace RadiologyCenter.Notification.Application.DTOs;

public record NotificationMessageDto(
    Guid Id,
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