using Microsoft.AspNetCore.Components.Authorization;
using RadiologyCenter.Desktop.Models;
using RadiologyCenter.Desktop.Security;
using RadiologyCenter.Desktop.Security.Permissions;

namespace RadiologyCenter.Desktop.Features.Dashboard.Services;

public sealed record DashboardData(
    int? TotalPatients,
    int? TotalExaminations,
    int? ExamsInProgress,
    int? TotalInventoryItems,
    IReadOnlyList<ExaminationListItemDto> RecentExaminations);

public sealed class DashboardService(ApiClient api, AuthenticationStateProvider authentication)
{
    public async Task<DashboardData> LoadAsync(CancellationToken ct = default)
    {
        var user = (await authentication.GetAuthenticationStateAsync()).User;
        bool Allowed(string permission) => user.HasClaim(AppClaimTypes.IsAdmin, "true") || user.HasClaim(AppClaimTypes.Permission, permission);
        var countQuery = new { pagination = new { pageNumber = 1, pageSize = 1 } };
        var recentQuery = new { pagination = new { pageNumber = 1, pageSize = 6 }, sortBy = "CreatedAt", sortDescending = true };
        var progressQuery = new { pagination = new { pageNumber = 1, pageSize = 1 }, filters = new[] { new { field = "Status", @operator = "Equals", value = "InProgress" } } };

        async Task<PagedResult<T>?> FetchAsync<T>(bool allowed, string path, object query) =>
            allowed ? await api.PostAsync<PagedResult<T>>(path, query, ct) : null;

        var patientsTask = FetchAsync<PatientDto>(Allowed(PatientsPermissions.Read), "api/patients/all", countQuery);
        var examinationsTask = FetchAsync<ExaminationListItemDto>(Allowed(ExaminationsPermissions.Read), "api/examinations/all", recentQuery);
        var progressTask = FetchAsync<ExaminationListItemDto>(Allowed(ExaminationsPermissions.Read), "api/examinations/all", progressQuery);
        var itemsTask = FetchAsync<ItemDto>(Allowed(InventoryPermissions.ItemsRead), "api/inventory/items/all", countQuery);
        await Task.WhenAll(patientsTask, examinationsTask, progressTask, itemsTask);
        return new DashboardData((await patientsTask)?.TotalCount, (await examinationsTask)?.TotalCount,
            (await progressTask)?.TotalCount, (await itemsTask)?.TotalCount, (await examinationsTask)?.Items ?? []);
    }
}
