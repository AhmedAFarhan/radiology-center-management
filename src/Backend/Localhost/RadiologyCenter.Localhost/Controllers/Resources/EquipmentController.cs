using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
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

namespace RadiologyCenter.Localhost.Controllers.Resources;

[ApiController]
[Route("api/resources/equipment")]
public class EquipmentController : ControllerBase
{
    private readonly IMessageBus _bus;

    public EquipmentController(IMessageBus bus) => _bus = bus;

    [HasPermission(EquipmentReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<EquipmentDto>>(new GetEquipmentByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(EquipmentReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<EquipmentDto>>>(new GetEquipmentsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(EquipmentCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateEquipmentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<EquipmentDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(EquipmentUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateEquipmentCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { EquipmentId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(EquipmentUpdateCode)]
    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> SetStatusAsync(Guid id, [FromBody] SetEquipmentStatusCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { EquipmentId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(EquipmentUpdateCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateEquipmentCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(EquipmentUpdateCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateEquipmentCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(EquipmentDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteEquipmentCommand(id), ct);
        return result.ToActionResult();
    }
}
