using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.ResourceManagement.Application.Commands.ActivateStaff;
using RadiologyCenter.ResourceManagement.Application.Commands.CreateStaff;
using RadiologyCenter.ResourceManagement.Application.Commands.DeactivateStaff;
using RadiologyCenter.ResourceManagement.Application.Commands.DeleteStaff;
using RadiologyCenter.ResourceManagement.Application.Commands.UpdateStaff;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Application.Queries.GetStaffById;
using RadiologyCenter.ResourceManagement.Application.Queries.GetStaffs;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Resources;

[ApiController]
[Route("api/resources/staff")]
public class StaffController : ControllerBase
{
    private readonly IMessageBus _bus;

    public StaffController(IMessageBus bus) => _bus = bus;

    [HasPermission(StaffReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<StaffDto>>(new GetStaffByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(StaffReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<StaffDto>>>(new GetStaffsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(StaffCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateStaffCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<StaffDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(StaffUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateStaffCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { StaffId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(StaffUpdateCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateStaffCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(StaffUpdateCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateStaffCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(StaffDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteStaffCommand(id), ct);
        return result.ToActionResult();
    }
}
