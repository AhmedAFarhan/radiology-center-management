using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class PacsSyncService : IAsyncDisposable
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(15);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TokenStorage _tokenStorage;
    private readonly BackendStatusService _backendStatus;
    private readonly HashSet<string> _knownStudies = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _gate = new(1, 1);

    private CancellationTokenSource? _cts;
    private Task? _loop;

    public PacsSyncService(
        IServiceScopeFactory scopeFactory,
        TokenStorage tokenStorage,
        BackendStatusService backendStatus)
    {
        _scopeFactory = scopeFactory;
        _tokenStorage = tokenStorage;
        _backendStatus = backendStatus;
    }

    public void Start()
    {
        if (_loop is not null)
            return;
        _cts = new CancellationTokenSource();
        _loop = Task.Run(() => LoopAsync(_cts.Token));
    }

    public async Task StopAsync()
    {
        if (_cts is null)
            return;
        _cts.Cancel();
        try { await _loop; }
        catch { /* cancellation */ }
        finally
        {
            _cts.Dispose();
            _cts = null;
            _loop = null;
        }
    }

    public ValueTask DisposeAsync() => new(StopAsync());

    private async Task LoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try { await ReconcileAsync(ct); }
            catch { /* transient, retry next tick */ }

            try { await Task.Delay(Interval, ct); }
            catch (OperationCanceledException) { return; }
        }
    }

    /// <summary>Links newly-arrived Orthanc studies to their examinations via PatientID == PatientCode.</summary>
    public async Task ReconcileAsync(CancellationToken ct = default)
    {
        if (!await _gate.WaitAsync(0, ct))
            return;

        try
        {
            if (_tokenStorage.GetTokens() is null)
                return;
            if (!_backendStatus.IsReady)
                return;
            if (PacsService.Instance is not { } pacs || !pacs.IsReady)
                return;

            IReadOnlyList<PacsService.PacsStudy> studies;
            try { studies = await pacs.GetStudiesAsync(ct); }
            catch { return; }

            var fresh = studies.Where(s => !_knownStudies.Contains(s.StudyInstanceUid)).ToList();
            if (fresh.Count == 0)
                return;

            foreach (var study in fresh)
                _knownStudies.Add(study.StudyInstanceUid);

            using var scope = _scopeFactory.CreateScope();

            var patients = await scope.ServiceProvider
                .GetRequiredService<PatientService>()
                .GetPagedAsync(null, null, false, 1, 1000, ct);

            var exams = await scope.ServiceProvider
                .GetRequiredService<ExaminationService>()
                .GetPagedAsync(null, null, false, 1, 1000, ct);

            var patientByCode = patients.Items
                .Where(p => !string.IsNullOrWhiteSpace(p.PatientCode))
                .GroupBy(p => p.PatientCode, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var examService = scope.ServiceProvider.GetRequiredService<ExaminationService>();

            foreach (var study in fresh)
            {
                if (string.IsNullOrWhiteSpace(study.PatientId))
                    continue;
                if (!patientByCode.TryGetValue(study.PatientId, out var patient))
                    continue;

                var target = exams.Items
                    .Where(e => e.PatientId == patient.Id && string.IsNullOrEmpty(e.StudyInstanceUID))
                    .OrderByDescending(e => e.CompletedAt ?? DateTime.MinValue)
                    .FirstOrDefault();
                if (target is null)
                    continue;

                try
                {
                    await examService.RecordPacsImagesAsync(target.Id, study.StudyInstanceUid, study.AccessionNumber, ct);
                }
                catch
                {
                    // keep reconciling the remaining studies
                }
            }
        }
        finally
        {
            _gate.Release();
        }
    }
}