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
    private DateOnly _day;
    private bool _disposed;
    private bool _unavailable;

    public SimpleFileLoggerProvider(string? directory = null, int maxFiles = 5)
    {
        _dir = directory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EGcare", "logs");
        _maxFiles = Math.Max(1, maxFiles);
        try { Directory.CreateDirectory(_dir); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { _unavailable = true; }
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(this, categoryName);

    public void Write(string category, LogLevel level, string message)
    {
        lock (_lock)
        {
            if (_disposed || _unavailable) return;
            try
            {
                var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {category}: {message}";
                // Bound a single pathological diagnostic and count UTF-8 bytes, including the newline.
                if (line.Length > 64 * 1024) line = line[..(64 * 1024)] + " [truncated]";
                var bytes = Encoding.UTF8.GetByteCount(line + Environment.NewLine);
                var day = DateOnly.FromDateTime(DateTime.Now);
                if (_writer is null || _day != day || _currentSize + bytes > MaxFileBytes)
                {
                    _day = day;
                    RotateTo(Path.Combine(_dir, $"app-{DateTime.UtcNow:yyyyMMdd-HHmmss-fffffff}-{Environment.ProcessId}-{Guid.NewGuid():N}.log"));
                }

                _writer!.WriteLine(line);
                _writer.Flush();
                _currentSize += bytes;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                _unavailable = true;
                try { _writer?.Dispose(); } catch (IOException) { }
                _writer = null;
            }
        }
    }

    private void RotateTo(string file)
    {
        _writer?.Dispose();
        _currentFile = file;
        _currentSize = 0;
        _writer = new StreamWriter(
            new FileStream(file, FileMode.CreateNew, FileAccess.Write, FileShare.Read),
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        Prune();
    }

    private void Prune()
    {
        var files = Directory.GetFiles(_dir, "app-*.log").OrderByDescending(f => new FileInfo(f).LastWriteTimeUtc).ToList();
        foreach (var extra in files.Skip(_maxFiles))
        {
            try { File.Delete(extra); } catch { /* locked or in use */ }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _disposed = true;
            try { _writer?.Dispose(); } catch (IOException) { }
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

            var message = formatter(state, null);
            if (exception is not null)
                message += Environment.NewLine + exception.GetType().Name + Environment.NewLine + exception.StackTrace;
            _provider.Write(_category, logLevel, message);
        }
    }
}
