using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class InventoryService : CrudServiceBase
{
    private const string ItemsRes = "api/inventory/items";
    private const string SuppliersRes = "api/inventory/suppliers";
    private const string StockMovementsRes = "api/inventory/stock-movements";
    private const string PurchaseOrdersRes = "api/inventory/purchase-orders";

    public InventoryService(ApiClient api) : base(api) { }

    public Task<PagedResult<ItemDto>> GetItemsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<ItemDto>(ItemsRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<byte[]> ExportItemsAsync(string? searchTerm, CancellationToken ct = default)
        => Api.PostBytesAsync($"{ItemsRes}/export", new
        {
            searchTerm,
            pagination = new { pageNumber = 1, pageSize = 50_000 },
        }, ct);

    public Task<byte[]> DownloadItemsImportTemplateAsync(CancellationToken ct = default)
        => Api.GetBytesAsync($"{ItemsRes}/import-template", ct);

    public Task<ExcelImportResultDto> ImportItemsAsync(string fileName, Stream content, CancellationToken ct = default)
        => Api.PostFormAsync<ExcelImportResultDto>(
            $"{ItemsRes}/import",
            file: ("file", fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", content),
            ct: ct);

    public Task<ItemDto> GetItemByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<ItemDto>(ItemsRes, id, ct);

    public Task<ItemDto> CreateItemAsync(ItemInput input, CancellationToken ct = default)
        => CreateEntityAsync<ItemDto>(ItemsRes, input, ct);

    public Task UpdateItemAsync(string id, ItemInput input, CancellationToken ct = default)
        => UpdateEntityAsync(ItemsRes, id, input, ct);

    public Task ActivateItemAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(ItemsRes, id, true, ct);

    public Task DeactivateItemAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(ItemsRes, id, false, ct);

    public Task DeleteItemAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(ItemsRes, id, ct);

    public Task<ItemStockDto> GetStockAsync(string id, CancellationToken ct = default)
        => Api.GetAsync<ItemStockDto>($"{ItemsRes}/{id}/stock", ct);

    public Task IssueStockAsync(string id, IssueStockInput input, CancellationToken ct = default)
        => Api.PostAsync<object>($"{ItemsRes}/{id}/issue", input, ct);

    public Task<PagedResult<SupplierDto>> GetSuppliersPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<SupplierDto>(SuppliersRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<SupplierDto> GetSupplierByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<SupplierDto>(SuppliersRes, id, ct);

    public Task<SupplierDto> CreateSupplierAsync(SupplierInput input, CancellationToken ct = default)
        => CreateEntityAsync<SupplierDto>(SuppliersRes, input, ct);

    public Task UpdateSupplierAsync(string id, SupplierInput input, CancellationToken ct = default)
        => UpdateEntityAsync(SuppliersRes, id, input, ct);

    public Task ActivateSupplierAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(SuppliersRes, id, true, ct);

    public Task DeactivateSupplierAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(SuppliersRes, id, false, ct);

    public Task DeleteSupplierAsync(string id, CancellationToken ct = default)
        => DeleteEntityAsync(SuppliersRes, id, ct);

    public Task<PagedResult<StockMovementDto>> GetStockMovementsPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<StockMovementDto>(StockMovementsRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<PurchaseOrderDto>(PurchaseOrdersRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<PurchaseOrderDto> GetPurchaseOrderByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<PurchaseOrderDto>(PurchaseOrdersRes, id, ct);

    public Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderInput input, CancellationToken ct = default)
        => CreateEntityAsync<PurchaseOrderDto>(PurchaseOrdersRes, input, ct);

    public Task PlacePurchaseOrderAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{PurchaseOrdersRes}/{id}/place", ct: ct);

    public Task ReceivePurchaseOrderAsync(string id, ReceivePurchaseOrderInput input, CancellationToken ct = default)
        => Api.PostAsync<object>($"{PurchaseOrdersRes}/{id}/receive", input, ct);

    public Task CancelPurchaseOrderAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{PurchaseOrdersRes}/{id}/cancel", ct: ct);
}