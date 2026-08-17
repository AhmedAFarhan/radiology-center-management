using System.Text;
using Microsoft.Extensions.Logging;

namespace RadiologyCenter.Desktop.Services;

public sealed class SimpleFileLoggerProvider : ILoggerProvider
{
    private const long MaxFileBytes = 2 * 1024 * 1024;

    private readonly object _lock = new();
    private readonly string _dir;
    private readonly int _maxFiles;
    private StreamWriter? _writer;
    private string _currentFile = string.Empty;
    private long _currentSize;

    public SimpleFileLoggerProvider(string? directory = null, int maxFiles = 5)
    {
        _dir = directory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EGcare", "logs");
        _maxFiles = Math.Max(1, maxFiles);
        Directory.CreateDirectory(_dir);
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(this, categoryName);

    public void Write(string category, LogLevel level, string message)
    {
        lock (_lock)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {category}: {message}";
            var file = Path.Combine(_dir, $"app-{DateTime.Now:yyyyMMdd}.log");
            if (_currentFile != file || _currentSize + line.Length > MaxFileBytes)
                RotateTo(file);

            _writer!.WriteLine(line);
            _writer.Flush();
            _currentSize += line.Length;
        }
    }

    private void RotateTo(string file)
    {
        _writer?.Dispose();
        _currentFile = file;
        _currentSize = 0;
        _writer = new StreamWriter(
            new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.Read),
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        Prune();
    }

    private void Prune()
    {
        var files = Directory.GetFiles(_dir, "app-*.log").OrderByDescending(f => f).ToList();
        foreach (var extra in files.Skip(_maxFiles))
        {
            try { File.Delete(extra); } catch { /* locked or in use */ }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _writer?.Dispose();
            _writer = null;
        }
    }

    private sealed class FileLogger : ILogger
    {
        private readonly SimpleFileLoggerProvider _provider;
        private readonly string _category;

        public FileLogger(SimpleFileLoggerProvider provider, string category)
        {
            _provider = provider;
            _category = category;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            if (exception is not null)
                message += Environment.NewLine + exception;
            _provider.Write(_category, logLevel, message);
        }
    }
}