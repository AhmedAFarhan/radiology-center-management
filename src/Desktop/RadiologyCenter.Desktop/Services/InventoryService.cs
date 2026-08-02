using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class InventoryService
{
    private readonly ApiClient _api;

    public InventoryService(ApiClient api) => _api = api;

    public Task<PagedResult<ItemDto>> GetItemsPagedAsync(
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

        return _api.PostAsync<PagedResult<ItemDto>>("api/inventory/items/all", query, ct);
    }

    public Task<ItemDto> GetItemByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<ItemDto>($"api/inventory/items/{id}", ct);

    public Task<ItemDto> CreateItemAsync(ItemInput input, CancellationToken ct = default)
        => _api.PostAsync<ItemDto>("api/inventory/items", input, ct);

    public Task UpdateItemAsync(string id, ItemInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/inventory/items/{id}", input, ct);

    public Task ActivateItemAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/inventory/items/{id}/activate", ct: ct);

    public Task DeactivateItemAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/inventory/items/{id}/deactivate", ct: ct);

    public Task DeleteItemAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/inventory/items/{id}", ct);

    public Task<ItemStockDto> GetStockAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<ItemStockDto>($"api/inventory/items/{id}/stock", ct);

    public Task IssueStockAsync(string id, IssueStockInput input, CancellationToken ct = default)
        => _api.PostAsync<object>($"api/inventory/items/{id}/issue", input, ct);

    public Task<PagedResult<SupplierDto>> GetSuppliersPagedAsync(
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

        return _api.PostAsync<PagedResult<SupplierDto>>("api/inventory/suppliers/all", query, ct);
    }

    public Task<SupplierDto> GetSupplierByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<SupplierDto>($"api/inventory/suppliers/{id}", ct);

    public Task<SupplierDto> CreateSupplierAsync(SupplierInput input, CancellationToken ct = default)
        => _api.PostAsync<SupplierDto>("api/inventory/suppliers", input, ct);

    public Task UpdateSupplierAsync(string id, SupplierInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/inventory/suppliers/{id}", input, ct);

    public Task ActivateSupplierAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/inventory/suppliers/{id}/activate", ct: ct);

    public Task DeactivateSupplierAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/inventory/suppliers/{id}/deactivate", ct: ct);

    public Task DeleteSupplierAsync(string id, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/inventory/suppliers/{id}", ct);

    public Task<PagedResult<StockMovementDto>> GetStockMovementsPagedAsync(
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

        return _api.PostAsync<PagedResult<StockMovementDto>>("api/inventory/stock-movements/all", query, ct);
    }

    public Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersPagedAsync(
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

        return _api.PostAsync<PagedResult<PurchaseOrderDto>>("api/inventory/purchase-orders/all", query, ct);
    }

    public Task<PurchaseOrderDto> GetPurchaseOrderByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<PurchaseOrderDto>($"api/inventory/purchase-orders/{id}", ct);

    public Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderInput input, CancellationToken ct = default)
        => _api.PostAsync<PurchaseOrderDto>("api/inventory/purchase-orders", input, ct);

    public Task PlacePurchaseOrderAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/inventory/purchase-orders/{id}/place", ct: ct);

    public Task ReceivePurchaseOrderAsync(string id, ReceivePurchaseOrderInput input, CancellationToken ct = default)
        => _api.PostAsync<object>($"api/inventory/purchase-orders/{id}/receive", input, ct);

    public Task CancelPurchaseOrderAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/inventory/purchase-orders/{id}/cancel", ct: ct);
}
