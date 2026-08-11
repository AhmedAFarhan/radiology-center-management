namespace RadiologyCenter.Notification.Application.Abstractions;

public interface ISmsProvider
{
    Task<Result> SendAsync(string phoneNumber, string message, CancellationToken ct = default);
}