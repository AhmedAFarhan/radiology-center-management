using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;

namespace RadiologyCenter.Localhost.Extensions;

public class AncillaryDirectory : IAncillaryDirectory
{
    private readonly ResourceManagementDbContext _db;

    public AncillaryDirectory(ResourceManagementDbContext db) => _db = db;

    public async Task<IReadOnlyDictionary<Guid, string>> ResolveStaffNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<Guid, string>();

        return await _db.Staff
            .Where(s => idList.Contains(s.Id))
            .Select(s => new { s.Id, s.FirstName, s.MiddleName, s.LastName })
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.ToDictionary(
                s => s.Id,
                s => string.Join(' ', new[] { s.FirstName, s.MiddleName, s.LastName }.Where(p => !string.IsNullOrWhiteSpace(p)))));
    }

    public async Task<IReadOnlyDictionary<Guid, string>> ResolveReferralNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<Guid, string>();

        return await _db.ReferralDoctors
            .Where(r => idList.Contains(r.Id))
            .Select(r => new { r.Id, r.FirstName, r.MiddleName, r.LastName })
            .ToListAsync(ct)
            .ContinueWith(items => items.Result.ToDictionary(
                r => r.Id,
                r => string.Join(' ', new[] { r.FirstName, r.MiddleName, r.LastName }.Where(p => !string.IsNullOrWhiteSpace(p)))));
    }

    public async Task<IReadOnlyDictionary<string, int>> GetActiveMachineCountByModalityAsync(CancellationToken ct = default)
    {
        var machines = await _db.Equipment
            .Where(e => e.IsActive && e.Status == EquipmentStatus.Operational)
            .Select(e => e.Modality)
            .ToListAsync(ct);

        return machines
            .GroupBy(m => m.Name)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}