using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Examinations.Domain.Enumerations;
using RadiologyCenter.Examinations.Infrastructure.Persistence;
using RadiologyCenter.Patients.Infrastructure.Persistence;
using RadiologyCenter.Reports.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;

namespace RadiologyCenter.Localhost.Extensions;

public class ReportDirectory : IReportDirectory
{
    private readonly PatientsDbContext _patientsDb;
    private readonly ResourceManagementDbContext _resourceManagementDb;
    private readonly ExaminationsDbContext _examinationsDb;

    public ReportDirectory(
        PatientsDbContext patientsDb,
        ResourceManagementDbContext resourceManagementDb,
        ExaminationsDbContext examinationsDb)
    {
        _patientsDb = patientsDb;
        _resourceManagementDb = resourceManagementDb;
        _examinationsDb = examinationsDb;
    }

    public async Task<IReadOnlyDictionary<Guid, string>> ResolvePatientNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<Guid, string>();

        return await _patientsDb.Patients
            .Where(p => idList.Contains(p.Id))
            .Select(p => new { p.Id, p.FirstName, p.MiddleName, p.LastName })
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.ToDictionary(
                p => p.Id,
                p => string.Join(' ', new[] { p.FirstName, p.MiddleName, p.LastName }.Where(part => !string.IsNullOrWhiteSpace(part)))));
    }

    public async Task<IReadOnlyDictionary<Guid, string>> ResolveRadiologistNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<Guid, string>();

        return await _resourceManagementDb.Staff
            .Where(s => idList.Contains(s.Id))
            .Select(s => new { s.Id, s.FirstName, s.MiddleName, s.LastName })
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.ToDictionary(
                s => s.Id,
                s => string.Join(' ', new[] { s.FirstName, s.MiddleName, s.LastName }.Where(part => !string.IsNullOrWhiteSpace(part)))));
    }

    public async Task<IReadOnlyDictionary<Guid, string>> ResolveExaminationTypeNamesAsync(IEnumerable<Guid> examinationIds, CancellationToken ct = default)
    {
        var idList = examinationIds.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<Guid, string>();

        var pairs = await _examinationsDb.Examinations
            .Where(e => idList.Contains(e.Id))
            .Select(e => new { e.Id, e.ExaminationTypeId })
            .ToListAsync(ct);

        var typeIds = pairs.Select(p => p.ExaminationTypeId).Distinct().ToList();
        var typeNames = await _examinationsDb.ExaminationTypes
            .Where(t => typeIds.Contains(t.Id))
            .Select(t => new { t.Id, t.Name })
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.ToDictionary(t => t.Id, t => t.Name));

        return pairs
            .GroupBy(p => p.Id)
            .ToDictionary(g => g.Key, g => typeNames.GetValueOrDefault(g.First().ExaminationTypeId) ?? string.Empty);
    }

    public async Task<bool> IsExaminationCompletedAsync(Guid examinationId, CancellationToken ct = default)
        => await _examinationsDb.Examinations
            .AnyAsync(e => e.Id == examinationId && e.Status == ExaminationStatus.Completed, ct);
}