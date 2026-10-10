using Microsoft.Extensions.Logging;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Notification.Application.Abstractions;

namespace RadiologyCenter.Notification.Infrastructure.Services;

public class LogEmailProvider : IEmailProvider
{
    private readonly ILogger<LogEmailProvider> _logger;

    public LogEmailProvider(ILogger<LogEmailProvider> logger)
    {
        _logger = logger;
    }

    public Task<Result> SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        _logger.LogWarning("Email delivery is unavailable: no delivery provider is installed.");
        return Task.FromResult(Result.Failure(Error.Conflict("Notification.ProviderNotConfigured", "Email delivery is not configured for this clinic. The message was not delivered.")));
    }
}