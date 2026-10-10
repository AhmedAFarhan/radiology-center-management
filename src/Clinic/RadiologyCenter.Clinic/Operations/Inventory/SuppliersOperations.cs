using RadiologyCenter.Clinic.Dispatching;
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

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Inventory;

public class SuppliersOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public SuppliersOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(InventorySuppliersReadCode)]
    [ClinicOperation("GET", "inventory/suppliers/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SupplierDto>>(new GetSupplierByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventorySuppliersReadCode)]
    [ClinicOperation("POST", "inventory/suppliers/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<SupplierDto>>>(new GetSuppliersQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventorySuppliersCreateCode)]
    [ClinicOperation("POST", "inventory/suppliers")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateSupplierCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<SupplierDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventorySuppliersUpdateCode)]
    [ClinicOperation("PUT", "inventory/suppliers/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateSupplierCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { SupplierId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventorySuppliersUpdateCode)]
    [ClinicOperation("POST", "inventory/suppliers/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateSupplierCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventorySuppliersUpdateCode)]
    [ClinicOperation("POST", "inventory/suppliers/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateSupplierCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InventorySuppliersDeleteCode)]
    [ClinicOperation("DELETE", "inventory/suppliers/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteSupplierCommand(id), ct);
        return result.ToActionResult();
    }
}
