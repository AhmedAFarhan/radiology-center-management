using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.ResourceManagement.Application.Commands.CreateLeave;
using RadiologyCenter.ResourceManagement.Application.Commands.DeleteLeave;
using RadiologyCenter.ResourceManagement.Application.Commands.UpdateLeave;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Application.Queries.GetLeaveById;
using RadiologyCenter.ResourceManagement.Application.Queries.GetLeaves;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Resources;

[ApiController]
[Route("api/resources/leave")]
public class LeaveController : ControllerBase
{
    private readonly IMessageBus _bus;

    public LeaveController(IMessageBus bus) => _bus = bus;

    [HasPermission(LeaveReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<LeaveDto>>(new GetLeaveByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(LeaveReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<LeaveDto>>>(new GetLeavesQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(LeaveCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateLeaveCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<LeaveDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(LeaveUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateLeaveCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { LeaveId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(LeaveDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteLeaveCommand(id), ct);
        return result.ToActionResult();
    }
}
