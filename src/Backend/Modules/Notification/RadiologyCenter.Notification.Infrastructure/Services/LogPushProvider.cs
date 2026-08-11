using Microsoft.Extensions.Logging;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Notification.Application.Abstractions;

namespace RadiologyCenter.Notification.Infrastructure.Services;

public class LogPushProvider : IPushProvider
{
    private readonly ILogger<LogPushProvider> _logger;

    public LogPushProvider(ILogger<LogPushProvider> logger)
    {
        _logger = logger;
    }

    public Task<Result> SendAsync(string deviceToken, string title, string body, CancellationToken ct = default)
    {
        _logger.LogInformation("[PUSH] to {DeviceToken} | title: {Title} | body: {Body}", deviceToken, title, body);
        return Task.FromResult(Result.Success());
    }
}