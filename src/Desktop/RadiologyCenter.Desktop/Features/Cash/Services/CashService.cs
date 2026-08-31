using RadiologyCenter.Desktop.Features.Cash.Models;

namespace RadiologyCenter.Desktop.Features.Cash.Services;

public sealed class CashService : CrudServiceBase
{
    private const string SessionsRes = "api/cash/sessions";
    private const string HandoversRes = "api/cash/handovers";

    public CashService(ApiClient api) : base(api) { }

    public Task<CashSessionDto> OpenAsync(OpenCashSessionInput input, CancellationToken ct = default)
        => Api.PostAsync<CashSessionDto>(SessionsRes, input, ct);

    public Task<CashSessionDto?> GetMyOpenAsync(CancellationToken ct = default)
        => Api.GetAsync<CashSessionDto>($"{SessionsRes}/my-open", ct);

    public Task<CashSessionDto> GetByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<CashSessionDto>(SessionsRes, id, ct);

    public Task<PagedResult<CashSessionListItemDto>> GetSessionsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        string? status,
        CancellationToken ct = default)
    {
        var url = string.IsNullOrWhiteSpace(status)
            ? $"{SessionsRes}/all"
            : $"{SessionsRes}/all?status={Uri.EscapeDataString(status)}";

        return Api.PostAsync<PagedResult<CashSessionListItemDto>>(url, PagedQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);
    }

    public Task<IReadOnlyList<CashEntryDto>> GetEntriesAsync(string sessionId, CancellationToken ct = default)
        => Api.GetAsync<IReadOnlyList<CashEntryDto>>($"{SessionsRes}/{sessionId}/entries", ct);

    public Task<CashEntryDto> AddEntryAsync(string sessionId, AddCashEntryInput input, CancellationToken ct = default)
        => Api.PostAsync<CashEntryDto>($"{SessionsRes}/{sessionId}/entries", input, ct);

    public Task<CashHandoverDto> CloseAsync(string sessionId, CloseCashSessionInput input, CancellationToken ct = default)
        => Api.PostAsync<CashHandoverDto>($"{SessionsRes}/{sessionId}/close", input, ct);

    public Task<PagedResult<CashHandoverDto>> GetHandoversPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<CashHandoverDto>(HandoversRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<CashHandoverDto?> GetHandoverBySessionAsync(string sessionId, CancellationToken ct = default)
        => Api.GetAsync<CashHandoverDto>($"{HandoversRes}/{sessionId}", ct);

    public Task<CashHandoverDto> ApproveHandoverAsync(string sessionId, CancellationToken ct = default)
        => Api.PostAsync<CashHandoverDto>($"{HandoversRes}/{sessionId}/approve", null, ct);
}
