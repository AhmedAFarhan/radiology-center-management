using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.ResourceManagement.Application.Commands.CreateWorkShift;
using RadiologyCenter.ResourceManagement.Application.Commands.DeleteWorkShift;
using RadiologyCenter.ResourceManagement.Application.Commands.UpdateWorkShift;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Application.Queries.GetWorkShiftById;
using RadiologyCenter.ResourceManagement.Application.Queries.GetWorkShifts;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Resources;

[ApiController]
[Route("api/resources/work-shifts")]
public class WorkShiftsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public WorkShiftsController(IMessageBus bus) => _bus = bus;

    [HasPermission(ShiftsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<WorkShiftDto>>(new GetWorkShiftByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ShiftsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<WorkShiftDto>>>(new GetWorkShiftsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(ShiftsCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateWorkShiftCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<WorkShiftDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(ShiftsUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateWorkShiftCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { WorkShiftId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ShiftsDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteWorkShiftCommand(id), ct);
        return result.ToActionResult();
    }
}
