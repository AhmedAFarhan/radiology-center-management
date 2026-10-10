using Microsoft.Extensions.Logging;

namespace RadiologyCenter.IntegrationTests.Shared;

internal sealed class TestErrorLogProvider(string path) : ILoggerProvider
{
    private readonly object _gate = new();
    private readonly string _path = path;
    public ILogger CreateLogger(string categoryName) => new Logger(this, categoryName);
    public void Dispose() { }
    private sealed class Logger(TestErrorLogProvider provider, string category) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Error;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            lock (provider._gate)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(provider._path)!);
                File.AppendAllText(provider._path, $"{DateTime.UtcNow:O} {category}: {formatter(state, exception)}\n{exception}\n");
            }
        }
    }
}
