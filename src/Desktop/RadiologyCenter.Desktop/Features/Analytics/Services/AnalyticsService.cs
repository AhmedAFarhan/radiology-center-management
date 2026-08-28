using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Analytics.Services;

public sealed class AnalyticsService
{
    private readonly ApiClient _api;

    public AnalyticsService(ApiClient api) => _api = api;

    public Task<FinancialAnalyticsDto> GetFinancialAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => _api.GetAsync<FinancialAnalyticsDto>(BuildPath("api/analytics/financial", from, to), ct);

    public Task<OperationalAnalyticsDto> GetOperationalAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => _api.GetAsync<OperationalAnalyticsDto>(BuildPath("api/analytics/operational", from, to), ct);

    public Task<StaffMachineAnalyticsDto> GetStaffMachineAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => _api.GetAsync<StaffMachineAnalyticsDto>(BuildPath("api/analytics/staff-machine", from, to), ct);

    public Task<ProfitAnalyticsDto> GetProfitAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => _api.GetAsync<ProfitAnalyticsDto>(BuildPath("api/analytics/profit", from, to), ct);

    public Task<IReadOnlyList<FinancialExamRowDto>> GetFinancialExamsAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => _api.GetAsync<IReadOnlyList<FinancialExamRowDto>>(BuildPath("api/analytics/financial/exams", from, to), ct);

    private static string BuildPath(string path, DateTime from, DateTime to)
    {
        var fromText = from == DateTime.MinValue ? string.Empty : from.ToString("s");
        var toText = to.ToString("s");
        return $"{path}?from={Uri.EscapeDataString(fromText)}&to={Uri.EscapeDataString(toText)}";
    }
}
