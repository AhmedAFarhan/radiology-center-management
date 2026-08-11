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
        _logger.LogInformation("[SMS] to {PhoneNumber}: {Message}", phoneNumber, message);
        return Task.FromResult(Result.Success());
    }
}