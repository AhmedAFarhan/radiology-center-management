namespace RadiologyCenter.Notification.Application.Abstractions;

public interface IPushProvider
{
    Task<Result> SendAsync(string deviceToken, string title, string body, CancellationToken ct = default);
}