using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
using RadiologyCenter.ResourceManagement.Application.Commands.ActivateEquipment;
using RadiologyCenter.ResourceManagement.Application.Commands.CreateEquipment;
using RadiologyCenter.ResourceManagement.Application.Commands.DeactivateEquipment;
using RadiologyCenter.ResourceManagement.Application.Commands.DeleteEquipment;
using RadiologyCenter.ResourceManagement.Application.Commands.SetEquipmentStatus;
using RadiologyCenter.ResourceManagement.Application.Commands.UpdateEquipment;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Application.Queries.GetEquipmentById;
using RadiologyCenter.ResourceManagement.Application.Queries.GetEquipments;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Resources;

public class EquipmentOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public EquipmentOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(EquipmentReadCode)]
    [ClinicOperation("GET", "resources/equipment/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<EquipmentDto>>(new GetEquipmentByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(EquipmentReadCode)]
    [ClinicOperation("POST", "resources/equipment/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<EquipmentDto>>>(new GetEquipmentsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(EquipmentCreateCode)]
    [ClinicOperation("POST", "resources/equipment")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateEquipmentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<EquipmentDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(EquipmentUpdateCode)]
    [ClinicOperation("PUT", "resources/equipment/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateEquipmentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { EquipmentId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(EquipmentUpdateCode)]
    [ClinicOperation("POST", "resources/equipment/{id:guid}/status")]
    public async Task<ClinicResult> SetStatusAsync(Guid id, [FromBody] SetEquipmentStatusCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { EquipmentId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(EquipmentUpdateCode)]
    [ClinicOperation("POST", "resources/equipment/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateEquipmentCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(EquipmentUpdateCode)]
    [ClinicOperation("POST", "resources/equipment/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateEquipmentCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(EquipmentDeleteCode)]
    [ClinicOperation("DELETE", "resources/equipment/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteEquipmentCommand(id), ct);
        return result.ToActionResult();
    }
}
