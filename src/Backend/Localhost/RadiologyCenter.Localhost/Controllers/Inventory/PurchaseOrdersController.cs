using Microsoft.AspNetCore.Mvc;
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
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Inventory;

[ApiController]
[Route("api/inventory/purchase-orders")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IMessageBus _bus;

    public PurchaseOrdersController(IMessageBus bus) => _bus = bus;

    [HasPermission(InventoryPurchaseOrdersReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PurchaseOrderDto>>(new GetPurchaseOrderByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryPurchaseOrdersReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<PurchaseOrderDto>>>(new GetPurchaseOrdersQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryPurchaseOrdersCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreatePurchaseOrderCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PurchaseOrderDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryPurchaseOrdersUpdateCode)]
    [HttpPost("{id:guid}/place")]
    public async Task<IActionResult> PlaceAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new PlacePurchaseOrderCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryPurchaseOrdersUpdateCode)]
    [HttpPost("{id:guid}/receive")]
    public async Task<IActionResult> ReceiveAsync(Guid id, [FromBody] ReceivePurchaseOrderCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { PurchaseOrderId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryPurchaseOrdersUpdateCode)]
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new CancelPurchaseOrderCommand(id), ct);
        return result.ToActionResult();
    }
}
