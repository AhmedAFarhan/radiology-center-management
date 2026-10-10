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
        _logger.LogWarning("Push delivery is unavailable: no delivery provider is installed.");
        return Task.FromResult(Result.Failure(Error.Conflict("Notification.ProviderNotConfigured", "Push delivery is not configured for this clinic. The message was not delivered.")));
    }
}