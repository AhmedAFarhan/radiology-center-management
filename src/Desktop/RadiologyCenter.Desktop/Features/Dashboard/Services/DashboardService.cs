using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Dashboard.Services;

public sealed record DashboardData(
    int TotalPatients,
    int TotalExaminations,
    int ExamsInProgress,
    int TotalInventoryItems,
    IReadOnlyList<ExaminationDto> RecentExaminations,
    IReadOnlyList<PatientDto> RecentPatients);

public sealed class DashboardService
{
    private readonly ApiClient _api;

    public DashboardService(ApiClient api) => _api = api;

    public async Task<DashboardData> LoadAsync(CancellationToken ct = default)
    {
        var query = new { pagination = new { pageNumber = 1, pageSize = 100 } };

        var patientsTask = _api.PostAsync<PagedResult<PatientDto>>("api/patients/all", query, ct);
        var examinationsTask = _api.PostAsync<PagedResult<ExaminationDto>>("api/examinations/all", query, ct);
        var itemsTask = _api.PostAsync<PagedResult<ItemDto>>("api/inventory/items/all", query, ct);

        await Task.WhenAll(patientsTask, examinationsTask, itemsTask);

        var patients = await patientsTask;
        var examinations = await examinationsTask;
        var items = await itemsTask;

        return new DashboardData(
            patients.TotalCount,
            examinations.TotalCount,
            examinations.Items.Count(e => string.Equals(e.StatusKey, "InProgress", StringComparison.OrdinalIgnoreCase)),
            items.TotalCount,
            examinations.Items
                .OrderByDescending(e => e.CompletedAt ?? e.ScheduledAt ?? e.StartedAt)
                .Take(6)
                .ToList(),
            patients.Items
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .ToList());
    }
}

