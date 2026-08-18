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

        candidates = candidates.Where(s => !string.IsNullOrWhiteSpace(s.StudyInstanceUid)).ToList();

        if (candidates.Count == 0)
        {
            // Fall back to all studies matched by AccessionNumber or patient name.
            try
            {
                var all = await pacs.GetStudiesAsync(ct);
                candidates = all
                    .Where(s => !string.IsNullOrWhiteSpace(s.StudyInstanceUid))
                    .Where(s => AccessionsMatch(s.AccessionNumber, accessionNumber)
                                || NamesMatch(s.PatientName, patientName))
                    .ToList();
            }
            catch
            {
                return null;
            }
        }

        var best = SelectBestStudy(candidates, accessionNumber);
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

    private static PacsService.PacsStudy? SelectBestStudy(
        IReadOnlyList<PacsService.PacsStudy> candidates,
        string? accessionNumber)
    {
        var byAccession = candidates.FirstOrDefault(s =>
            AccessionsMatch(s.AccessionNumber, accessionNumber));
        if (byAccession is not null)
            return byAccession;

        return candidates
            .OrderByDescending(s => ParseDate(s.StudyDate))
            .ThenByDescending(s => s.StudyDate)
            .FirstOrDefault();
    }

    private static DateTime? ParseDate(string? value)
        => DateTime.TryParseExact(
            value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : null;

    private static bool AccessionsMatch(string? a, string? b)
        => !string.IsNullOrWhiteSpace(a)
           && !string.IsNullOrWhiteSpace(b)
           && a.Trim().Equals(b.Trim(), StringComparison.OrdinalIgnoreCase);

    private static bool NamesMatch(string? a, string? b)
    {
        if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b))
            return false;
        var normalizedA = new string(a.Where(char.IsLetterOrDigit).ToArray());
        var normalizedB = new string(b.Where(char.IsLetterOrDigit).ToArray());
        return !string.IsNullOrWhiteSpace(normalizedA)
               && normalizedA.Equals(normalizedB, StringComparison.OrdinalIgnoreCase);
    }
}
