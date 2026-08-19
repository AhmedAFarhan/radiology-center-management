using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class ReportService : CrudServiceBase
{
    private const string Res = "api/reports";
    private const string TemplatesRes = "api/reports/templates";

    public ReportService(ApiClient api) : base(api) { }

    public Task<PagedResult<ReportListItemDto>> GetPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<ReportListItemDto>(Res, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<ReportDto> GetByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<ReportDto>(Res, id, ct);

    public Task<ReportDto> GetByExaminationAsync(string examinationId, CancellationToken ct = default)
        => Api.GetAsync<ReportDto>($"{Res}/by-examination/{examinationId}", ct);

    public Task<IReadOnlyList<ReportVersionDto>> GetVersionsAsync(string id, CancellationToken ct = default)
        => Api.GetAsync<IReadOnlyList<ReportVersionDto>>($"{Res}/{id}/versions", ct);

    public Task<ReportDto> CreateDraftAsync(CreateReportDraftInput input, CancellationToken ct = default)
        => CreateEntityAsync<ReportDto>(Res, input, ct);

    public Task<ReportDto> UpsertSectionAsync(string id, UpsertReportSectionInput input, CancellationToken ct = default)
        => Api.PutAsync<ReportDto>($"{Res}/{id}/sections", input, ct);

    public Task<ReportFindingDto> AddFindingAsync(string id, AddReportFindingInput input, CancellationToken ct = default)
        => Api.PostAsync<ReportFindingDto>($"{Res}/{id}/findings", input, ct);

    public Task UpdateFindingAsync(string id, string findingId, UpdateReportFindingInput input, CancellationToken ct = default)
        => Api.PutAsync<object>($"{Res}/{id}/findings/{findingId}", input, ct);

    public Task RemoveFindingAsync(string id, string findingId, CancellationToken ct = default)
        => Api.SendDeleteAsync($"{Res}/{id}/findings/{findingId}", ct);

    public Task<ReportDto> FinalizeAsync(string id, CancellationToken ct = default)
        => Api.PostAsync<ReportDto>($"{Res}/{id}/finalize", null, ct);

    public Task<ReportDto> AmendAsync(string id, string reason, CancellationToken ct = default)
        => Api.PostAsync<ReportDto>($"{Res}/{id}/amend", new { reason }, ct);

    public Task CancelAsync(string id, string? reason = null, CancellationToken ct = default)
        => Api.SendAsync($"{Res}/{id}/cancel", new { reason }, ct);

    public Task<ReportDto> ApplyTemplateAsync(string id, string templateId, CancellationToken ct = default)
        => Api.PostAsync<ReportDto>($"{Res}/{id}/apply-template", new { templateId }, ct);

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

        return Api.PostAsync<PagedResult<ReportTemplateDto>>($"{TemplatesRes}/all", query, ct);
    }
}