using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class ExaminationService
{
    private readonly ApiClient _api;

    public ExaminationService(ApiClient api) => _api = api;

    public Task<PagedResult<ExaminationTypeDto>> GetTypesPagedAsync(
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

        return _api.PostAsync<PagedResult<ExaminationTypeDto>>("api/catalog/examination-types/all", query, ct);
    }

    public Task<ExaminationTypeDto> GetTypeByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<ExaminationTypeDto>($"api/catalog/examination-types/{id}", ct);

    public Task<ExaminationTypeDto> CreateTypeAsync(ExaminationTypeInput input, CancellationToken ct = default)
        => _api.PostAsync<ExaminationTypeDto>("api/catalog/examination-types", input, ct);

    public Task UpdateTypeAsync(string id, ExaminationTypeInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/catalog/examination-types/{id}", input, ct);

    public Task ActivateTypeAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/catalog/examination-types/{id}/activate", ct: ct);

    public Task DeactivateTypeAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/catalog/examination-types/{id}/deactivate", ct: ct);

    public Task DeleteTypeAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/catalog/examination-types/{id}", ct);

    public Task<PagedResult<ExaminationDto>> GetPagedAsync(
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

        return _api.PostAsync<PagedResult<ExaminationDto>>("api/examinations/all", query, ct);
    }

    public Task<ExaminationDto> GetByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<ExaminationDto>($"api/examinations/{id}", ct);

    public Task<ExaminationDto> CreateAsync(ExaminationInput input, CancellationToken ct = default)
        => _api.PostAsync<ExaminationDto>("api/examinations", input, ct);

    public Task UpdateAsync(string id, ExaminationUpdateInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/examinations/{id}", input, ct);

    public Task ScheduleAsync(string id, DateTime scheduledAt, CancellationToken ct = default)
        => _api.SendAsync($"api/examinations/{id}/schedule", new { scheduledAt }, ct);

    public Task CheckInAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/examinations/{id}/check-in", ct: ct);

    public Task StartAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/examinations/{id}/start", ct: ct);

    public Task CompleteAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/examinations/{id}/complete", ct: ct);

    public Task CancelAsync(string id, string? reason = null, CancellationToken ct = default)
        => _api.SendAsync($"api/examinations/{id}/cancel", new { reason }, ct);
}
