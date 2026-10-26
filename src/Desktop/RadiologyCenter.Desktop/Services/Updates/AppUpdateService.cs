namespace RadiologyCenter.Desktop.Services.Updates;

/// <summary>Serializes updater operations and binds each offered/downloaded update to its original feed.</summary>
public sealed class AppUpdateService(
    Func<UpdateFeed> resolveFeed,
    Func<UpdateFeed, IUpdateClient> createClient,
    Func<Task> prepareRestart) : IAppUpdates
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private UpdateFeed? _feed;
    private IUpdateClient? _client;
    private UpdateOffer? _downloaded;
    public UpdateOffer? Available { get; private set; }
    public event Action? Changed;

    public async Task<UpdateOffer?> CheckAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var feed = resolveFeed();
            if (feed != _feed)
            {
                _client = createClient(feed); _feed = feed;
                Available = _downloaded = null;
            }
            if (!_client!.IsInstalled) throw new UpdateFailure(UpdateFailureReason.NotInstalled);
            var version = await Task.Run(() => _client.CheckAsync(cancellationToken), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            if (resolveFeed() != _feed) { Available = _downloaded = null; throw new UpdateFailure(UpdateFailureReason.SourceChanged); }
            if (version != Available?.Version)
            {
                Available = version is null ? null : new UpdateOffer(_client.CurrentVersion, version);
                _downloaded = null;
            }
            return Available;
        }
        finally { _gate.Release(); Changed?.Invoke(); }
    }

    public async Task DownloadAsync(UpdateOffer offer, Action<int>? progress, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            ValidateOffer(offer);
            _downloaded = null;
            await Task.Run(() => _client!.DownloadAsync(offer.Version, progress, cancellationToken), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            ValidateOffer(offer);
            _downloaded = offer;
        }
        finally { _gate.Release(); Changed?.Invoke(); }
    }

    public bool IsDownloaded(UpdateOffer offer) => ReferenceEquals(offer, _downloaded);

    public async Task RestartAsync(UpdateOffer offer, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            ValidateOffer(offer);
            if (!IsDownloaded(offer)) throw new UpdateFailure(UpdateFailureReason.NotDownloaded);
            cancellationToken.ThrowIfCancellationRequested();
            // Drain in-flight operations and release the database maintenance lease before exiting.
            await prepareRestart();
            _client!.ApplyAndRestart(offer.Version);
        }
        finally { _gate.Release(); }
    }

    private void ValidateOffer(UpdateOffer offer)
    {
        if (_client is null || !ReferenceEquals(offer, Available) || resolveFeed() != _feed)
            throw new UpdateFailure(UpdateFailureReason.SourceChanged);
    }
}
