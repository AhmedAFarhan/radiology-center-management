using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class PacsSyncService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TokenStorage _tokenStorage;
    private readonly BackendStatusService _backendStatus;

    public PacsSyncService(
        IServiceScopeFactory scopeFactory,
        TokenStorage tokenStorage,
        BackendStatusService backendStatus)
    {
        _scopeFactory = scopeFactory;
        _tokenStorage = tokenStorage;
        _backendStatus = backendStatus;
    }

    /// <summary>
    /// Links the best-matching Orthanc study to a single examination (manual, per-record).
    /// Returns the linked StudyInstanceUID, or null when no match was found or the services
    /// are not ready.
    /// </summary>
    public async Task<string?> LinkStudyToExamAsync(
        string examId,
        string patientCode,
        string? accessionNumber,
        string? patientName,
        CancellationToken ct = default)
    {
        if (_tokenStorage.GetTokens() is null)
            return null;
        if (!_backendStatus.IsReady)
            return null;
        if (PacsService.Instance is not { } pacs || !pacs.IsReady)
            return null;

        IReadOnlyList<PacsService.PacsStudy> candidates;
        try
        {
            candidates = await pacs.GetStudiesAsync(patientCode, ct);
        }
        catch
        {
            return null;
        }

        // Patient and accession identifiers must both match exactly.
        if (string.IsNullOrWhiteSpace(patientCode) || string.IsNullOrWhiteSpace(accessionNumber))
            return null;
        var matching = candidates.Where(s => !string.IsNullOrWhiteSpace(s.StudyInstanceUid)
            && string.Equals(s.PatientId.Trim(), patientCode.Trim(), StringComparison.OrdinalIgnoreCase)
            && AccessionsMatch(s.AccessionNumber, accessionNumber)).ToList();
        var best = matching.Count == 1 ? matching[0] : null;
        if (best is null)
            return null;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var examService = scope.ServiceProvider.GetRequiredService<ExaminationService>();
            await examService.RecordPacsImagesAsync(examId, best.StudyInstanceUid, best.AccessionNumber, ct);
        }
        catch
        {
            return null;
        }

        return best.StudyInstanceUid;
    }

    private static bool AccessionsMatch(string? a, string? b)
        => !string.IsNullOrWhiteSpace(a)
           && !string.IsNullOrWhiteSpace(b)
           && a.Trim().Equals(b.Trim(), StringComparison.OrdinalIgnoreCase);

}
