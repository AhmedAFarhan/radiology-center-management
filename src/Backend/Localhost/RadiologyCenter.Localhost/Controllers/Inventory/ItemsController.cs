using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Inventory.Application.Commands.ActivateItem;
using RadiologyCenter.Inventory.Application.Commands.CreateItem;
using RadiologyCenter.Inventory.Application.Commands.DeactivateItem;
using RadiologyCenter.Inventory.Application.Commands.DeleteItem;
using RadiologyCenter.Inventory.Application.Commands.IssueStock;
using RadiologyCenter.Inventory.Application.Commands.UpdateItem;
using RadiologyCenter.Inventory.Application.DTOs;
using RadiologyCenter.Inventory.Application.Queries.GetItemById;
using RadiologyCenter.Inventory.Application.Queries.GetItems;
using RadiologyCenter.Inventory.Application.Queries.GetItemStock;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Inventory;

[ApiController]
[Route("api/inventory/items")]
public class ItemsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ItemsController(IMessageBus bus) => _bus = bus;

    [HasPermission(InventoryItemsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemDto>>(new GetItemByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ItemDto>>>(new GetItemsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateItemCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ItemId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsUpdateCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateItemCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsUpdateCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateItemCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryItemsDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteItemCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryStockReadCode)]
    [HttpGet("{id:guid}/stock")]
    public async Task<IActionResult> GetStockAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ItemStockDto>>(new GetItemStockQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventoryStockIssueCode)]
    [HttpPost("{id:guid}/issue")]
    public async Task<IActionResult> IssueStockAsync(Guid id, [FromBody] IssueStockCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ItemId = id }, ct);
        return result.ToActionResult();
    }
}
