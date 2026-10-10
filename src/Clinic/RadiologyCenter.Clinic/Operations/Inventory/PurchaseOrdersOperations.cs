using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Inventory.Application.Commands.CancelPurchaseOrder;
using RadiologyCenter.Inventory.Application.Commands.CreatePurchaseOrder;
using RadiologyCenter.Inventory.Application.Commands.PlacePurchaseOrder;
using RadiologyCenter.Inventory.Application.Commands.ReceivePurchaseOrder;
using RadiologyCenter.Inventory.Application.DTOs;
using RadiologyCenter.Inventory.Application.Queries.GetPurchaseOrderById;
using RadiologyCenter.Inventory.Application.Queries.GetPurchaseOrders;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Inventory;

public class PurchaseOrdersOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public PurchaseOrdersOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(InventoryPurchaseOrdersReadCode)]
    [ClinicOperation("GET", "inventory/purchase-orders/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PurchaseOrderDto>>(new GetPurchaseOrderByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryPurchaseOrdersReadCode)]
    [ClinicOperation("POST", "inventory/purchase-orders/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<PurchaseOrderDto>>>(new GetPurchaseOrdersQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryPurchaseOrdersCreateCode)]
    [ClinicOperation("POST", "inventory/purchase-orders")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreatePurchaseOrderCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PurchaseOrderDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryPurchaseOrdersUpdateCode)]
    [ClinicOperation("POST", "inventory/purchase-orders/{id:guid}/place")]
    public async Task<ClinicResult> PlaceAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new PlacePurchaseOrderCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryPurchaseOrdersUpdateCode)]
    [ClinicOperation("POST", "inventory/purchase-orders/{id:guid}/receive")]
    public async Task<ClinicResult> ReceiveAsync(Guid id, [FromBody] ReceivePurchaseOrderCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { PurchaseOrderId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventoryPurchaseOrdersUpdateCode)]
    [ClinicOperation("POST", "inventory/purchase-orders/{id:guid}/cancel")]
    public async Task<ClinicResult> CancelAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new CancelPurchaseOrderCommand(id), ct);
        return result.ToActionResult();
    }
}
