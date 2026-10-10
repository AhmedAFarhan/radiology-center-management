using Microsoft.Extensions.Logging;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Notification.Application.Abstractions;

namespace RadiologyCenter.Notification.Infrastructure.Services;

public class LogSmsProvider : ISmsProvider
{
    private readonly ILogger<LogSmsProvider> _logger;

    public LogSmsProvider(ILogger<LogSmsProvider> logger)
    {
        _logger = logger;
    }

    public Task<Result> SendAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        _logger.LogWarning("SMS delivery is unavailable: no delivery provider is installed.");
        return Task.FromResult(Result.Failure(Error.Conflict("Notification.ProviderNotConfigured", "SMS delivery is not configured for this clinic. The message was not delivered.")));
    }
}