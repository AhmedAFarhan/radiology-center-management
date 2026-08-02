using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class PatientService
{
    private readonly ApiClient _api;

    public PatientService(ApiClient api) => _api = api;

    public Task<PagedResult<PatientDto>> GetPagedAsync(
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

        return _api.PostAsync<PagedResult<PatientDto>>("api/patients/all", query, ct);
    }

    public Task<PatientDto> GetByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<PatientDto>($"api/patients/{id}", ct);

    public Task<PatientDto> CreateAsync(PatientInput input, CancellationToken ct = default)
        => _api.PostAsync<PatientDto>("api/patients", input, ct);

    public Task UpdateAsync(string id, PatientInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/patients/{id}", input, ct);

    public Task ActivateAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/patients/{id}/activate", ct: ct);

    public Task DeactivateAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/patients/{id}/deactivate", ct: ct);

    public Task DeleteAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/patients/{id}", ct);
}
