using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class CashService
{
    private readonly ApiClient _api;

    public CashService(ApiClient api) => _api = api;

    public Task<CashSessionDto> OpenAsync(OpenCashSessionInput input, CancellationToken ct = default)
        => _api.PostAsync<CashSessionDto>("api/cash/sessions", input, ct);

    public Task<CashSessionDto?> GetMyOpenAsync(CancellationToken ct = default)
        => _api.GetAsync<CashSessionDto>("api/cash/sessions/my-open", ct);

    public Task<CashSessionDto> GetByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<CashSessionDto>($"api/cash/sessions/{id}", ct);

    public Task<PagedResult<CashSessionDto>> GetSessionsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        string? status,
        CancellationToken ct = default)
    {
        var query = new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

        var url = string.IsNullOrWhiteSpace(status)
            ? "api/cash/sessions/all"
            : $"api/cash/sessions/all?status={Uri.EscapeDataString(status)}";

        return _api.PostAsync<PagedResult<CashSessionDto>>(url, query, ct);
    }

    public Task<IReadOnlyList<CashEntryDto>> GetEntriesAsync(string sessionId, CancellationToken ct = default)
        => _api.GetAsync<IReadOnlyList<CashEntryDto>>($"api/cash/sessions/{sessionId}/entries", ct);

    public Task<CashEntryDto> AddEntryAsync(string sessionId, AddCashEntryInput input, CancellationToken ct = default)
        => _api.PostAsync<CashEntryDto>($"api/cash/sessions/{sessionId}/entries", input, ct);

    public Task<CashHandoverDto> CloseAsync(string sessionId, CloseCashSessionInput input, CancellationToken ct = default)
        => _api.PostAsync<CashHandoverDto>($"api/cash/sessions/{sessionId}/close", input, ct);

    public Task<PagedResult<CashHandoverDto>> GetHandoversPagedAsync(
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

        return _api.PostAsync<PagedResult<CashHandoverDto>>("api/cash/handovers/all", query, ct);
    }

    public Task<CashHandoverDto?> GetHandoverBySessionAsync(string sessionId, CancellationToken ct = default)
        => _api.GetAsync<CashHandoverDto>($"api/cash/handovers/{sessionId}", ct);

    public Task<CashHandoverDto> ApproveHandoverAsync(string sessionId, CancellationToken ct = default)
        => _api.PostAsync<CashHandoverDto>($"api/cash/handovers/{sessionId}/approve", null, ct);
}