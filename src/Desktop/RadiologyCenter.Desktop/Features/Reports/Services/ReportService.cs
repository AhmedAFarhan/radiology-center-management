using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Reports.Services;

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

    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, int> _revisions = new();

    private ReportDto Remember(ReportDto report)
    {
        _revisions[report.Id] = report.Revision;
        return report;
    }

    private async Task<int> RevisionAsync(string id, CancellationToken ct)
    {
        if (_revisions.TryGetValue(id, out var revision)) return revision;
        return (await GetByIdAsync(id, ct)).Revision;
    }

    public async Task<ReportDto> GetByIdAsync(string id, CancellationToken ct = default)
        => Remember(await FetchByIdAsync<ReportDto>(Res, id, ct));

    public async Task<ReportDto> GetByExaminationAsync(string examinationId, CancellationToken ct = default)
        => Remember(await Api.GetAsync<ReportDto>($"{Res}/by-examination/{examinationId}", ct));

    public Task<IReadOnlyList<ReportVersionDto>> GetVersionsAsync(string id, CancellationToken ct = default)
        => Api.GetAsync<IReadOnlyList<ReportVersionDto>>($"{Res}/{id}/versions", ct);

    public async Task<ReportDto> CreateDraftAsync(CreateReportDraftInput input, CancellationToken ct = default)
        => Remember(await CreateEntityAsync<ReportDto>(Res, input, ct));

    public async Task<ReportDto> UpsertSectionAsync(string id, UpsertReportSectionInput input, CancellationToken ct = default)
    {
        input.ExpectedRevision = await RevisionAsync(id, ct);
        return Remember(await Api.PutAsync<ReportDto>($"{Res}/{id}/sections", input, ct));
    }

    public async Task<ReportFindingDto> AddFindingAsync(string id, AddReportFindingInput input, CancellationToken ct = default)
    {
        input.ExpectedRevision = await RevisionAsync(id, ct);
        var finding = await Api.PostAsync<ReportFindingDto>($"{Res}/{id}/findings", input, ct);
        await GetByIdAsync(id, ct);
        return finding;
    }

    public async Task UpdateFindingAsync(string id, string findingId, UpdateReportFindingInput input, CancellationToken ct = default)
    {
        input.ExpectedRevision = await RevisionAsync(id, ct);
        await Api.PutAsync<object>($"{Res}/{id}/findings/{findingId}", input, ct);
        await GetByIdAsync(id, ct);
    }

    public async Task RemoveFindingAsync(string id, string findingId, CancellationToken ct = default)
    {
        var expectedRevision = await RevisionAsync(id, ct);
        await Api.SendDeleteAsync($"{Res}/{id}/findings/{findingId}?expectedRevision={expectedRevision}", ct);
        await GetByIdAsync(id, ct);
    }

    public async Task<ReportDto> FinalizeAsync(string id, CancellationToken ct = default)
        => Remember(await Api.PostAsync<ReportDto>($"{Res}/{id}/finalize", new { expectedRevision = await RevisionAsync(id, ct) }, ct));

    public async Task<ReportDto> AmendAsync(string id, string reason, CancellationToken ct = default)
        => Remember(await Api.PostAsync<ReportDto>($"{Res}/{id}/amend", new { reason, expectedRevision = await RevisionAsync(id, ct) }, ct));

    public async Task CancelAsync(string id, string? reason = null, CancellationToken ct = default)
    {
        await Api.SendAsync($"{Res}/{id}/cancel", new { reason, expectedRevision = await RevisionAsync(id, ct) }, ct);
        await GetByIdAsync(id, ct);
    }

    public async Task<ReportDto> ApplyTemplateAsync(string id, string templateId, CancellationToken ct = default)
        => Remember(await Api.PostAsync<ReportDto>($"{Res}/{id}/apply-template", new { templateId, expectedRevision = await RevisionAsync(id, ct) }, ct));

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
