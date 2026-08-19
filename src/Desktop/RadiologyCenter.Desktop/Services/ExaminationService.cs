using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class ExaminationService : CrudServiceBase
{
    private const string TypesRes = "api/catalog/examination-types";
    private const string Res = "api/examinations";

    public ExaminationService(ApiClient api) : base(api) { }

    public Task<PagedResult<ExaminationTypeDto>> GetTypesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<ExaminationTypeDto>(TypesRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<ExaminationTypeDto> GetTypeByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<ExaminationTypeDto>(TypesRes, id, ct);

    public Task<ExaminationTypeDto> CreateTypeAsync(ExaminationTypeInput input, CancellationToken ct = default)
        => CreateEntityAsync<ExaminationTypeDto>(TypesRes, input, ct);

    public Task UpdateTypeAsync(string id, ExaminationTypeInput input, CancellationToken ct = default)
        => UpdateEntityAsync(TypesRes, id, input, ct);

    public Task ActivateTypeAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(TypesRes, id, true, ct);

    public Task DeactivateTypeAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(TypesRes, id, false, ct);

    public Task DeleteTypeAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(TypesRes, id, ct);

    public Task<PagedResult<ExaminationDto>> GetPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<ExaminationDto>(Res, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<ExaminationDto> GetByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<ExaminationDto>(Res, id, ct);

    public Task<ExaminationDto> CreateAsync(ExaminationInput input, CancellationToken ct = default)
        => CreateEntityAsync<ExaminationDto>(Res, input, ct);

    public Task UpdateAsync(string id, ExaminationUpdateInput input, CancellationToken ct = default)
        => UpdateEntityAsync(Res, id, input, ct);

    public Task ScheduleAsync(string id, DateTime scheduledAt, CancellationToken ct = default)
        => Api.SendAsync($"{Res}/{id}/schedule", new { scheduledAt }, ct);

    public Task CheckInAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{Res}/{id}/check-in", ct: ct);

    public Task StartAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{Res}/{id}/start", ct: ct);

    public Task CompleteAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{Res}/{id}/complete", ct: ct);

    public Task RecordPacsImagesAsync(string id, string studyInstanceUID, string? accessionNumber = null, CancellationToken ct = default)
        => Api.SendAsync($"{Res}/{id}/pacs-images", new { studyInstanceUID, accessionNumber }, ct);

    public Task CancelAsync(string id, string? reason = null, CancellationToken ct = default)
        => Api.SendAsync($"{Res}/{id}/cancel", new { reason }, ct);
}