using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Identity.Application.Commands.AddPermissionToRole;
using RadiologyCenter.Identity.Application.Commands.CreateRole;
using RadiologyCenter.Identity.Application.Commands.RemovePermissionFromRole;
using RadiologyCenter.Identity.Application.Commands.UpdateRole;
using RadiologyCenter.Identity.Application.DTOs;
using RadiologyCenter.Identity.Application.Queries.GetRoleById;
using RadiologyCenter.Identity.Application.Queries.GetRoles;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Identity;

[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public RolesController(IMessageBus bus) => _bus = bus;

    [HasPermission(RolesReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<RoleDto>>(new GetRoleByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(RolesReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<RoleDto>>>(new GetRolesQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(RolesCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRoleCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(RolesUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateRoleCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { RoleId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(RolesManagePermissionsCode)]
    [HttpPost("{id:guid}/permissions")]
    public async Task<IActionResult> AddPermissionAsync(Guid id, [FromBody] AddPermissionToRoleCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { RoleId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(RolesManagePermissionsCode)]
    [HttpDelete("{id:guid}/permissions/{permissionCode}")]
    public async Task<IActionResult> RemovePermissionAsync(Guid id, string permissionCode, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemovePermissionFromRoleCommand(id, permissionCode), ct);
        return result.ToActionResult();
    }
}
