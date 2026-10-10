using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Xunit;

namespace RadiologyCenter.Clinic.IntegrationTests;

[Collection("Direct SQL")]
public sealed class ProductionLoggingTests(DirectSqlFixture fixture)
{
    [Fact]
    public async Task InvalidBusinessCommandsDoNotLogPasswordsOrPatientFields()
    {
        var capture = new CaptureProvider();
        using var logs = LoggerFactory.Create(builder => builder.AddProvider(capture));
        await using var runtime = await ClinicRuntime.StartAsync(DirectSqlFixture.Configuration(fixture.AdminConnection), loggerFactory: logs);
        var patientMarker = "PrivatePatient_" + Guid.NewGuid().ToString("N");
        var password = "NeverLogPassword1!" + Guid.NewGuid().ToString("N");
        var response = await runtime.Dispatcher.DispatchAsync(new("POST", "users", JsonSerializer.Serialize(new
        {
            userName = "", email = "synthetic@clinic.test", firstName = patientMarker, lastName = "Test",
            phoneNumber = "01012345678", password, roleIds = Array.Empty<Guid>()
        })));
        Assert.Equal(400, response.StatusCode);
        var diagnosticText = string.Join('\n', capture.Messages);
        Assert.True(!diagnosticText.Contains(patientMarker) && !diagnosticText.Contains(password),
            "Sensitive values reached log categories: " + string.Join(", ", capture.Messages.Where(m => m.Contains(patientMarker) || m.Contains(password)).Select(m => m.Split('|')[0]).Distinct()));
    }

    private sealed class CaptureProvider : ILoggerProvider
    {
        public ConcurrentQueue<string> Messages { get; } = new();
        public ILogger CreateLogger(string categoryName) => new CaptureLogger(Messages, categoryName);
        public void Dispose() { }
        private sealed class CaptureLogger(ConcurrentQueue<string> messages, string category) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
            public bool IsEnabled(LogLevel level) => true;
            public void Log<TState>(LogLevel level, EventId id, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
                => messages.Enqueue(category + "|" + formatter(state, exception) + exception);
        }
    }
}
