namespace RadiologyCenter.Notification.Application.Abstractions;

public interface IEmailProvider
{
    Task<Result> SendAsync(string to, string subject, string body, CancellationToken ct = default);
}