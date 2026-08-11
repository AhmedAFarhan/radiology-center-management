using RadiologyCenter.Notification.Domain.Enumerations;

namespace RadiologyCenter.Notification.Application.Abstractions;

public interface INotificationSender
{
    Task<Result> SendAsync(
        NotificationChannel channel,
        string recipient,
        string subject,
        string body,
        CancellationToken ct = default);
}