using Velopack;
using Velopack.Sources;

namespace RadiologyCenter.Desktop.Services.Updates;

public sealed class VelopackUpdateClient : IUpdateClient
{
    private readonly UpdateManager _manager;
    private UpdateInfo? _candidate;
    public VelopackUpdateClient(UpdateFeed feed)
    {
        _manager = feed.IsDirectory
            ? new UpdateManager(new SimpleFileSource(new DirectoryInfo(feed.Location)))
            : new UpdateManager(new SimpleWebSource(feed.Location));
    }
    public bool IsInstalled => _manager.IsInstalled;
    public string CurrentVersion => _manager.CurrentVersion?.ToNormalizedString() ?? ReleaseInfo.Version;
    public async Task<string?> CheckAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var candidate = await _manager.CheckForUpdatesAsync();
        cancellationToken.ThrowIfCancellationRequested();
        _candidate = candidate;
        return candidate?.TargetFullRelease.Version.ToNormalizedString();
    }
    public Task DownloadAsync(string version, Action<int>? progress, CancellationToken cancellationToken)
        => _manager.DownloadUpdatesAsync(Candidate(version), progress, cancellationToken);
    public void ApplyAndRestart(string version) => _manager.ApplyUpdatesAndRestart(Candidate(version).TargetFullRelease);
    private UpdateInfo Candidate(string version) => _candidate?.TargetFullRelease.Version.ToNormalizedString() == version
        ? _candidate : throw new UpdateFailure(UpdateFailureReason.SourceChanged);
}
