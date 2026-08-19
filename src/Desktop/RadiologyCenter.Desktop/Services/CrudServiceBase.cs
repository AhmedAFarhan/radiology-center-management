using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

/// <summary>
/// Shared plumbing for domain services that talk to the backend's standard
/// REST/CRUD endpoints. Derived services supply a resource path (e.g.
/// "api/patients") and their DTO/input types; the base implements the
/// paged-list, by-id, create, update, activate/deactivate and delete verbs.
/// </summary>
public abstract class CrudServiceBase
{
    protected ApiClient Api { get; }

    protected CrudServiceBase(ApiClient api) => Api = api;

    protected static object PagedQuery(string? searchTerm, string? sortBy, bool sortDescending, int pageNumber, int pageSize)
        => new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

    protected Task<PagedResult<T>> FetchPageAsync<T>(
        string resource,
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => Api.PostAsync<PagedResult<T>>($"{resource}/all", PagedQuery(searchTerm, sortBy, sortDescending, pageNumber, pageSize), ct);

    protected Task<T> FetchByIdAsync<T>(string resource, string id, CancellationToken ct = default)
        => Api.GetAsync<T>($"{resource}/{id}", ct);

    protected Task<T> CreateEntityAsync<T>(string resource, object input, CancellationToken ct = default)
        => Api.PostAsync<T>(resource, input, ct);

    protected Task UpdateEntityAsync(string resource, string id, object input, CancellationToken ct = default)
        => Api.PutAsync<object>($"{resource}/{id}", input, ct);

    protected Task SetEntityActiveAsync(string resource, string id, bool active, CancellationToken ct = default)
        => Api.SendAsync($"{resource}/{id}/{(active ? "activate" : "deactivate")}", ct: ct);

    protected Task DeleteEntityAsync(string resource, string id, CancellationToken ct = default)
        => Api.SendDeleteAsync($"{resource}/{id}", ct);
}