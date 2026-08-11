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
        _logger.LogInformation("[EMAIL] to {To} | subject: {Subject} | body: {Body}", to, subject, body);
        return Task.FromResult(Result.Success());
    }
}