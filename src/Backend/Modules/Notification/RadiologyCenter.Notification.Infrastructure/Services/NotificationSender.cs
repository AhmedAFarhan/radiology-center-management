using Microsoft.Extensions.Logging;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Notification.Application.Abstractions;
using RadiologyCenter.Notification.Domain.Enumerations;

namespace RadiologyCenter.Notification.Infrastructure.Services;

public class NotificationSender : INotificationSender
{
    private readonly ISmsProvider _smsProvider;
    private readonly IEmailProvider _emailProvider;
    private readonly IPushProvider _pushProvider;
    private readonly ILogger<NotificationSender> _logger;

    public NotificationSender(
        ISmsProvider smsProvider,
        IEmailProvider emailProvider,
        IPushProvider pushProvider,
        ILogger<NotificationSender> logger)
    {
        _smsProvider = smsProvider;
        _emailProvider = emailProvider;
        _pushProvider = pushProvider;
        _logger = logger;
    }

    public async Task<Result> SendAsync(
        NotificationChannel channel,
        string recipient,
        string subject,
        string body,
        CancellationToken ct = default)
    {
        try
        {
            return channel switch
            {
                _ when channel == NotificationChannel.Sms => await _smsProvider.SendAsync(recipient, body, ct),
                _ when channel == NotificationChannel.Email => await _emailProvider.SendAsync(recipient, subject, body, ct),
                _ when channel == NotificationChannel.Push => await _pushProvider.SendAsync(recipient, subject, body, ct),
                _ => Result.Failure(Error.Validation("InvalidChannel", $"Channel '{channel}' is not supported."))
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification via {Channel} to {Recipient}.", channel.Name, recipient);
            return Result.Failure(Error.Failure("Failed to send notification."));
        }
    }
}