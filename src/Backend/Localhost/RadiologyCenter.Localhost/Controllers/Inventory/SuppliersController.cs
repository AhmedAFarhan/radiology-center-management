using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Inventory.Application.Commands.ActivateSupplier;
using RadiologyCenter.Inventory.Application.Commands.CreateSupplier;
using RadiologyCenter.Inventory.Application.Commands.DeactivateSupplier;
using RadiologyCenter.Inventory.Application.Commands.DeleteSupplier;
using RadiologyCenter.Inventory.Application.Commands.UpdateSupplier;
using RadiologyCenter.Inventory.Application.DTOs;
using RadiologyCenter.Inventory.Application.Queries.GetSupplierById;
using RadiologyCenter.Inventory.Application.Queries.GetSuppliers;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Inventory;

[ApiController]
[Route("api/inventory/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly IMessageBus _bus;

    public SuppliersController(IMessageBus bus) => _bus = bus;

    [HasPermission(InventorySuppliersReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SupplierDto>>(new GetSupplierByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventorySuppliersReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<SupplierDto>>>(new GetSuppliersQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventorySuppliersCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateSupplierCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SupplierDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(InventorySuppliersUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateSupplierCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { SupplierId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(InventorySuppliersUpdateCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateSupplierCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventorySuppliersUpdateCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateSupplierCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InventorySuppliersDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteSupplierCommand(id), ct);
        return result.ToActionResult();
    }
}
