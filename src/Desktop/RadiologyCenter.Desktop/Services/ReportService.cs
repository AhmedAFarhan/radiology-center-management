using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class ReportService
{
    private readonly ApiClient _api;

    public ReportService(ApiClient api) => _api = api;

    public Task<PagedResult<ReportListItemDto>> GetPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

        return _api.PostAsync<PagedResult<ReportListItemDto>>("api/reports/all", query, ct);
    }

    public Task<ReportDto> GetByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<ReportDto>($"api/reports/{id}", ct);

    public Task<ReportDto> GetByExaminationAsync(string examinationId, CancellationToken ct = default)
        => _api.GetAsync<ReportDto>($"api/reports/by-examination/{examinationId}", ct);

    public Task<IReadOnlyList<ReportVersionDto>> GetVersionsAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<IReadOnlyList<ReportVersionDto>>($"api/reports/{id}/versions", ct);

    public Task<ReportDto> CreateDraftAsync(CreateReportDraftInput input, CancellationToken ct = default)
        => _api.PostAsync<ReportDto>("api/reports", input, ct);

    public Task<ReportDto> UpsertSectionAsync(string id, UpsertReportSectionInput input, CancellationToken ct = default)
        => _api.PutAsync<ReportDto>($"api/reports/{id}/sections", input, ct);

    public Task<ReportFindingDto> AddFindingAsync(string id, AddReportFindingInput input, CancellationToken ct = default)
        => _api.PostAsync<ReportFindingDto>($"api/reports/{id}/findings", input, ct);

    public Task UpdateFindingAsync(string id, string findingId, UpdateReportFindingInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/reports/{id}/findings/{findingId}", input, ct);

    public Task RemoveFindingAsync(string id, string findingId, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/reports/{id}/findings/{findingId}", ct);

    public Task<ReportDto> FinalizeAsync(string id, CancellationToken ct = default)
        => _api.PostAsync<ReportDto>($"api/reports/{id}/finalize", null, ct);

    public Task<ReportDto> AmendAsync(string id, string reason, CancellationToken ct = default)
        => _api.PostAsync<ReportDto>($"api/reports/{id}/amend", new { reason }, ct);

    public Task CancelAsync(string id, string? reason = null, CancellationToken ct = default)
        => _api.SendAsync($"api/reports/{id}/cancel", new { reason }, ct);

    public Task<ReportDto> ApplyTemplateAsync(string id, string templateId, CancellationToken ct = default)
        => _api.PostAsync<ReportDto>($"api/reports/{id}/apply-template", new { templateId }, ct);

    public Task<PagedResult<ReportTemplateDto>> GetTemplatesPagedAsync(
        string? searchTerm,
        bool onlyActive,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = new
        {
            pagination = new { pageNumber, pageSize },
            searchTerm,
        };

        return _api.PostAsync<PagedResult<ReportTemplateDto>>("api/reports/templates/all", query, ct);
    }
}