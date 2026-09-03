using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using RadiologyCenter.IntegrationTests.Shared;

namespace Tests;

public class InventoryTests : TestBase
{
    private const string ItemsUrl = "api/inventory/items";
    private const string SuppliersUrl = "api/inventory/suppliers";
    private const string PurchaseOrdersUrl = "api/inventory/purchase-orders";
    private const string StockMovementsUrl = "api/inventory/stock-movements";

    public InventoryTests(CustomWebApplicationFactory factory) : base(factory) { }

    #region Items — Create

    [Fact]
    public async Task CreateItem_ValidCommand_ReturnsOk()
    {
        var command = new
        {
            Name = $"Test Item {Guid.NewGuid():N}",
            Category = "MedicalSupply",
            Unit = "Piece"
        };
        var response = await Client.PostAsJsonAsync(ItemsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ItemDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Name.Should().Be(command.Name);
        body.Data.Category.Should().Be(command.Category);
        body.Data.Unit.Should().Be(command.Unit);
        body.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateItem_MissingName_ReturnsBadRequest()
    {
        var command = new { Category = "MedicalSupply", Unit = "Piece" };
        var response = await Client.PostAsJsonAsync(ItemsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateItem_MissingCategory_ReturnsBadRequest()
    {
        var command = new { Name = $"Item {Guid.NewGuid():N}", Unit = "Piece" };
        var response = await Client.PostAsJsonAsync(ItemsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateItem_MissingUnit_ReturnsBadRequest()
    {
        var command = new { Name = $"Item {Guid.NewGuid():N}", Category = "MedicalSupply" };
        var response = await Client.PostAsJsonAsync(ItemsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateItem_DuplicateName_ReturnsConflict()
    {
        var name = $"DupItem_{Guid.NewGuid():N}";
        var command = new { Name = name, Category = "MedicalSupply", Unit = "Piece" };
        await Client.PostAsJsonAsync(ItemsUrl, command);
        var response = await Client.PostAsJsonAsync(ItemsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateItem_WithOptionalFields_ReturnsOk()
    {
        var command = new
        {
            Name = $"Full Item {Guid.NewGuid():N}",
            Category = "Drug",
            Unit = "Box",
            Brand = "TestBrand",
            ReorderLevel = 10,
            ReorderQuantity = 50,
            LotTracked = true,
            StorageInstructions = "Store at room temperature"
        };
        var response = await Client.PostAsJsonAsync(ItemsUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ItemDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Brand.Should().Be(command.Brand);
        body.Data.ReorderLevel.Should().Be(command.ReorderLevel);
        body.Data.ReorderQuantity.Should().Be(command.ReorderQuantity);
        body.Data.LotTracked.Should().BeTrue();
        body.Data.StorageInstructions.Should().Be(command.StorageInstructions);
    }

    #endregion

    #region Items — GetById

    [Fact]
    public async Task GetItemById_ExistingItem_ReturnsOk()
    {
        var itemId = await CreateTestItemAsync();
        var response = await Client.GetAsync($"{ItemsUrl}/{itemId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ItemDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(itemId);
    }

    [Fact]
    public async Task GetItemById_NonexistentItem_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{ItemsUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Items — GetAll

    [Fact]
    public async Task GetItems_Paged_ReturnsOk()
    {
        await CreateTestItemAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{ItemsUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ItemDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    #endregion

    #region Items — Update

    [Fact]
    public async Task UpdateItem_ExistingItem_ReturnsOk()
    {
        var itemId = await CreateTestItemAsync();
        var command = new
        {
            Name = $"Updated Item {Guid.NewGuid():N}",
            Category = "Drug",
            Unit = "Bottle"
        };
        var response = await Client.PutAsJsonAsync($"{ItemsUrl}/{itemId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateItem_NonexistentItem_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var command = new { Name = "Updated", Category = "Drug", Unit = "Bottle" };
        var response = await Client.PutAsJsonAsync($"{ItemsUrl}/{fakeId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Items — Activate / Deactivate

    [Fact]
    public async Task ActivateItem_DeactivatedItem_ReturnsOk()
    {
        var itemId = await CreateTestItemAsync();
        await Client.PostAsJsonAsync($"{ItemsUrl}/{itemId}/deactivate", new { });
        var response = await Client.PostAsJsonAsync($"{ItemsUrl}/{itemId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivateItem_ExistingItem_ReturnsOk()
    {
        var itemId = await CreateTestItemAsync();
        var response = await Client.PostAsJsonAsync($"{ItemsUrl}/{itemId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Items — Delete

    [Fact]
    public async Task DeleteItem_ExistingItem_ReturnsOk()
    {
        var itemId = await CreateTestItemAsync();
        var response = await Client.DeleteAsync($"{ItemsUrl}/{itemId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteItem_NonexistentItem_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{ItemsUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Items — Stock

    [Fact]
    public async Task GetStock_ExistingItem_ReturnsOk()
    {
        var itemId = await CreateTestItemAsync();
        var response = await Client.GetAsync($"{ItemsUrl}/{itemId}/stock");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<ItemStockDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.StockOnHand.Should().Be(0);
    }

    [Fact]
    public async Task GetStock_NonexistentItem_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{ItemsUrl}/{fakeId}/stock");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task IssueStock_ValidRequest_ReturnsOk()
    {
        var itemId = await CreateTestItemAsync();
        var supplierId = await CreateTestSupplierAsync();
        await ReceiveStockAsync(itemId, supplierId, quantity: 20, lotNumber: $"LOT-{Guid.NewGuid():N}");

        var command = new { ItemId = itemId, Quantity = 5, Notes = "Test issue" };
        var response = await Client.PostAsJsonAsync($"{ItemsUrl}/{itemId}/issue", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task IssueStock_InsufficientStock_ReturnsConflict()
    {
        var itemId = await CreateTestItemAsync();
        var supplierId = await CreateTestSupplierAsync();
        await ReceiveStockAsync(itemId, supplierId, quantity: 3, lotNumber: $"LOT-{Guid.NewGuid():N}");

        var command = new { ItemId = itemId, Quantity = 10, Notes = "Exceed available" };
        var response = await Client.PostAsJsonAsync($"{ItemsUrl}/{itemId}/issue", command);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task IssueStock_ZeroQuantity_ReturnsBadRequest()
    {
        var itemId = await CreateTestItemAsync();
        var command = new { ItemId = itemId, Quantity = 0 };
        var response = await Client.PostAsJsonAsync($"{ItemsUrl}/{itemId}/issue", command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Suppliers — Create

    [Fact]
    public async Task CreateSupplier_ValidCommand_ReturnsOk()
    {
        var command = new
        {
            Name = $"Supplier {Guid.NewGuid():N}",
            Phone = "01012345678"
        };
        var response = await Client.PostAsJsonAsync(SuppliersUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Name.Should().Be(command.Name);
        body.Data.Phone.Should().Be(command.Phone);
        body.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateSupplier_MissingName_ReturnsBadRequest()
    {
        var command = new { Phone = "01012345678" };
        var response = await Client.PostAsJsonAsync(SuppliersUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSupplier_MissingPhone_ReturnsBadRequest()
    {
        var command = new { Name = $"Supplier {Guid.NewGuid():N}" };
        var response = await Client.PostAsJsonAsync(SuppliersUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSupplier_InvalidPhoneFormat_ReturnsBadRequest()
    {
        var command = new
        {
            Name = $"Supplier {Guid.NewGuid():N}",
            Phone = "not-a-phone"
        };
        var response = await Client.PostAsJsonAsync(SuppliersUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSupplier_DuplicatePhone_ReturnsConflict()
    {
        var phone = $"010{Random.Shared.Next(10000000, 99999999)}";
        var cmd1 = new { Name = $"Supplier1_{Guid.NewGuid():N}", Phone = phone };
        var cmd2 = new { Name = $"Supplier2_{Guid.NewGuid():N}", Phone = phone };
        await Client.PostAsJsonAsync(SuppliersUrl, cmd1);
        var response = await Client.PostAsJsonAsync(SuppliersUrl, cmd2);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    #endregion

    #region Suppliers — GetById

    [Fact]
    public async Task GetSupplierById_ExistingSupplier_ReturnsOk()
    {
        var supplierId = await CreateTestSupplierAsync();
        var response = await Client.GetAsync($"{SuppliersUrl}/{supplierId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(supplierId);
    }

    [Fact]
    public async Task GetSupplierById_NonexistentSupplier_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{SuppliersUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Suppliers — GetAll

    [Fact]
    public async Task GetSuppliers_Paged_ReturnsOk()
    {
        await CreateTestSupplierAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{SuppliersUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<SupplierDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    #endregion

    #region Suppliers — Update

    [Fact]
    public async Task UpdateSupplier_ExistingSupplier_ReturnsOk()
    {
        var supplierId = await CreateTestSupplierAsync();
        var command = new
        {
            Name = $"Updated Supplier {Guid.NewGuid():N}",
            Phone = "01011112222"
        };
        var response = await Client.PutAsJsonAsync($"{SuppliersUrl}/{supplierId}", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Suppliers — Activate / Deactivate

    [Fact]
    public async Task ActivateSupplier_DeactivatedSupplier_ReturnsOk()
    {
        var supplierId = await CreateTestSupplierAsync();
        await Client.PostAsJsonAsync($"{SuppliersUrl}/{supplierId}/deactivate", new { });
        var response = await Client.PostAsJsonAsync($"{SuppliersUrl}/{supplierId}/activate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeactivateSupplier_ExistingSupplier_ReturnsOk()
    {
        var supplierId = await CreateTestSupplierAsync();
        var response = await Client.PostAsJsonAsync($"{SuppliersUrl}/{supplierId}/deactivate", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Suppliers — Delete

    [Fact]
    public async Task DeleteSupplier_ExistingSupplier_ReturnsOk()
    {
        var supplierId = await CreateTestSupplierAsync();
        var response = await Client.DeleteAsync($"{SuppliersUrl}/{supplierId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteSupplier_NonexistentSupplier_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.DeleteAsync($"{SuppliersUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region PurchaseOrders — Create

    [Fact]
    public async Task CreatePurchaseOrder_ValidCommand_ReturnsOk()
    {
        var itemId = await CreateTestItemAsync();
        var supplierId = await CreateTestSupplierAsync();
        var command = new
        {
            SupplierId = supplierId,
            Items = new[]
            {
                new { ItemId = itemId, QuantityOrdered = 10, UnitCost = 25.50m }
            }
        };
        var response = await Client.PostAsJsonAsync(PurchaseOrdersUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PurchaseOrderDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.SupplierId.Should().Be(supplierId);
        body.Data.Status.Should().Be("Draft");
    }

    [Fact]
    public async Task CreatePurchaseOrder_MissingSupplierId_ReturnsBadRequest()
    {
        var itemId = await CreateTestItemAsync();
        var command = new
        {
            Items = new[]
            {
                new { ItemId = itemId, QuantityOrdered = 10, UnitCost = 25.50m }
            }
        };
        var response = await Client.PostAsJsonAsync(PurchaseOrdersUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePurchaseOrder_EmptyItems_ReturnsBadRequest()
    {
        var supplierId = await CreateTestSupplierAsync();
        var command = new
        {
            SupplierId = supplierId,
            Items = Array.Empty<object>()
        };
        var response = await Client.PostAsJsonAsync(PurchaseOrdersUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePurchaseOrder_DuplicateItems_ReturnsBadRequest()
    {
        var itemId = await CreateTestItemAsync();
        var supplierId = await CreateTestSupplierAsync();
        var command = new
        {
            SupplierId = supplierId,
            Items = new[]
            {
                new { ItemId = itemId, QuantityOrdered = 5, UnitCost = 10m },
                new { ItemId = itemId, QuantityOrdered = 3, UnitCost = 15m }
            }
        };
        var response = await Client.PostAsJsonAsync(PurchaseOrdersUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePurchaseOrder_ZeroQuantity_ReturnsBadRequest()
    {
        var itemId = await CreateTestItemAsync();
        var supplierId = await CreateTestSupplierAsync();
        var command = new
        {
            SupplierId = supplierId,
            Items = new[]
            {
                new { ItemId = itemId, QuantityOrdered = 0, UnitCost = 10m }
            }
        };
        var response = await Client.PostAsJsonAsync(PurchaseOrdersUrl, command);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region PurchaseOrders — GetById

    [Fact]
    public async Task GetPurchaseOrderById_ExistingOrder_ReturnsOk()
    {
        var poId = await CreateTestPurchaseOrderAsync();
        var response = await Client.GetAsync($"{PurchaseOrdersUrl}/{poId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PurchaseOrderDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(poId);
    }

    [Fact]
    public async Task GetPurchaseOrderById_NonexistentOrder_ReturnsNotFound()
    {
        var fakeId = Guid.NewGuid();
        var response = await Client.GetAsync($"{PurchaseOrdersUrl}/{fakeId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region PurchaseOrders — GetAll

    [Fact]
    public async Task GetPurchaseOrders_Paged_ReturnsOk()
    {
        await CreateTestPurchaseOrderAsync();
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{PurchaseOrdersUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<PurchaseOrderDto>>>();
        body!.Success.Should().BeTrue();
        body.Data!.Items.Should().NotBeEmpty();
    }

    #endregion

    #region PurchaseOrders — Place / Receive / Cancel

    [Fact]
    public async Task PlacePurchaseOrder_DraftOrder_ReturnsOk()
    {
        var poId = await CreateTestPurchaseOrderAsync();
        var response = await Client.PostAsJsonAsync($"{PurchaseOrdersUrl}/{poId}/place", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReceivePurchaseOrder_PlacedOrder_ReturnsOk()
    {
        var itemId = await CreateTestItemAsync();
        var supplierId = await CreateTestSupplierAsync();
        var poId = await CreateTestPurchaseOrderAsync(itemId, supplierId);
        await Client.PostAsJsonAsync($"{PurchaseOrdersUrl}/{poId}/place", new { });

        var command = new
        {
            PurchaseOrderId = poId,
            Lines = new[]
            {
                new { ItemId = itemId, Quantity = 10, LotNumber = $"LOT-{Guid.NewGuid():N}" }
            }
        };
        var response = await Client.PostAsJsonAsync($"{PurchaseOrdersUrl}/{poId}/receive", command);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelPurchaseOrder_DraftOrder_ReturnsOk()
    {
        var poId = await CreateTestPurchaseOrderAsync();
        var response = await Client.PostAsJsonAsync($"{PurchaseOrdersUrl}/{poId}/cancel", new { });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region StockMovements

    [Fact]
    public async Task GetStockMovements_Paged_ReturnsOk()
    {
        var request = new { Pagination = new { PageNumber = 1, PageSize = 10 } };
        var response = await Client.PostAsJsonAsync($"{StockMovementsUrl}/all", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<StockMovementDto>>>();
        body!.Success.Should().BeTrue();
    }

    #endregion

    #region Helpers

    private async Task<Guid> CreateTestItemAsync(string? name = null)
    {
        var itemName = name ?? $"Test Item {Guid.NewGuid():N}";
        var command = new { Name = itemName, Category = "MedicalSupply", Unit = "Piece" };
        await Client.PostAsJsonAsync(ItemsUrl, command);
        var allResponse = await Client.PostAsJsonAsync($"{ItemsUrl}/all",
            new { Pagination = new { PageNumber = 1, PageSize = 1 }, SearchTerm = itemName });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<ItemDto>>>();
        return allBody!.Data!.Items.First().Id;
    }

    private async Task<Guid> CreateTestSupplierAsync(string? phone = null)
    {
        var supplierPhone = phone ?? $"010{Random.Shared.Next(10000000, 99999999)}";
        var command = new { Name = $"Supplier {Guid.NewGuid():N}", Phone = supplierPhone };
        await Client.PostAsJsonAsync(SuppliersUrl, command);
        var allResponse = await Client.PostAsJsonAsync($"{SuppliersUrl}/all",
            new { Pagination = new { PageNumber = 1, PageSize = 1 }, SearchTerm = command.Name });
        allResponse.EnsureSuccessStatusCode();
        var allBody = await allResponse.Content.ReadFromJsonAsync<ApiResponse<PagedResultDto<SupplierDto>>>();
        return allBody!.Data!.Items.First().Id;
    }

    private async Task<Guid> CreateTestPurchaseOrderAsync(Guid? itemId = null, Guid? supplierId = null)
    {
        var item = itemId ?? await CreateTestItemAsync();
        var supplier = supplierId ?? await CreateTestSupplierAsync();
        var command = new
        {
            SupplierId = supplier,
            Items = new[]
            {
                new { ItemId = item, QuantityOrdered = 10, UnitCost = 25.50m }
            }
        };
        var response = await Client.PostAsJsonAsync(PurchaseOrdersUrl, command);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<PurchaseOrderDto>>();
        return body!.Data!.Id;
    }

    private async Task ReceiveStockAsync(Guid itemId, Guid supplierId, int quantity, string lotNumber)
    {
        var poId = await CreateTestPurchaseOrderAsync(itemId, supplierId);
        await Client.PostAsJsonAsync($"{PurchaseOrdersUrl}/{poId}/place", new { });
        var receiveCommand = new
        {
            PurchaseOrderId = poId,
            Lines = new[]
            {
                new { ItemId = itemId, Quantity = quantity, LotNumber = lotNumber }
            }
        };
        var receiveResponse = await Client.PostAsJsonAsync($"{PurchaseOrdersUrl}/{poId}/receive", receiveCommand);
        receiveResponse.EnsureSuccessStatusCode();
    }

    #endregion

    #region DTOs

    private sealed class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }

    private sealed class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    private sealed class ItemDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? Unit { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }
        public bool LotTracked { get; set; }
        public string? StorageInstructions { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    private sealed class ItemStockDto
    {
        public Guid ItemId { get; set; }
        public string? ItemName { get; set; }
        public int StockOnHand { get; set; }
        public List<object>? Batches { get; set; }
    }

    private sealed class SupplierDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public string? OrderNumber { get; set; }
        public Guid SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? Status { get; set; }
        public DateTime? ExpectedDeliveryAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public string? Notes { get; set; }
        public List<object>? Items { get; set; }
    }

    private sealed class StockMovementDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string? MovementType { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    private sealed class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    #endregion
}
