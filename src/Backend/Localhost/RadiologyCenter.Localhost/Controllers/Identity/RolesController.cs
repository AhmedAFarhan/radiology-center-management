using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Idnetity.Application.Commands.AddPermissionToRole;
using RadiologyCenter.Idnetity.Application.Commands.CreateRole;
using RadiologyCenter.Idnetity.Application.Commands.RemovePermissionFromRole;
using RadiologyCenter.Idnetity.Application.Commands.UpdateRole;
using RadiologyCenter.Idnetity.Application.DTOs;
using RadiologyCenter.Idnetity.Application.Queries.GetRoleById;
using RadiologyCenter.Idnetity.Application.Queries.GetRoles;
using Wolverine;

namespace RadiologyCenter.Localhost.Controllers.Identity;

[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public RolesController(IMessageBus bus) => _bus = bus;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<RoleDto>>(new GetRoleByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<RoleDto>>>(new GetRolesQuery(request), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRoleCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command, ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateRoleCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { RoleId = id }, ct);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/permissions")]
    public async Task<IActionResult> AddPermissionAsync(Guid id, [FromBody] AddPermissionToRoleCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { RoleId = id }, ct);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}/permissions/{permissionCode}")]
    public async Task<IActionResult> RemovePermissionAsync(Guid id, string permissionCode, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemovePermissionFromRoleCommand(id, permissionCode), ct);
        return result.ToActionResult();
    }
}
