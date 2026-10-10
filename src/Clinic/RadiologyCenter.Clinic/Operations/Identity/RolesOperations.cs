using RadiologyCenter.Clinic.Dispatching;
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

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Identity;

public class RolesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;
    private readonly RadiologyCenter.Clinic.Security.SafeIdentityReader _directory;

    public RolesOperations(IMessageBus bus,RadiologyCenter.Clinic.Security.SafeIdentityReader directory) { _bus=bus; _directory=directory; }

    [ClinicPermission(RolesReadCode)]
    [ClinicOperation("GET", "roles/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        if(_directory.Enabled)
        {
            var role=(await _directory.ReadAsync<RoleDto>("Role",ct)).FirstOrDefault(x=>x.Id==id);
            return role is null ? NotFound() : Result.Success(role).ToActionResult();
        }
        var result = await _bus.InvokeAsync<Result<RoleDto>>(new GetRoleByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(RolesReadCode)]
    [ClinicOperation("POST", "roles/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        if(_directory.Enabled) return Result.Success(RadiologyCenter.Clinic.Security.SafeIdentityReader.Page(await _directory.ReadAsync<RoleDto>("Role",ct),request)).ToActionResult();
        var result = await _bus.InvokeAsync<Result<PagedResult<RoleDto>>>(new GetRolesQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(RolesCreateCode)]
    [ClinicOperation("POST", "roles")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateRoleCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(RolesUpdateCode)]
    [ClinicOperation("PUT", "roles/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateRoleCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { RoleId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(RolesManagePermissionsCode)]
    [ClinicOperation("POST", "roles/{id:guid}/permissions")]
    public async Task<ClinicResult> AddPermissionAsync(Guid id, [FromBody] AddPermissionToRoleCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { RoleId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(RolesManagePermissionsCode)]
    [ClinicOperation("DELETE", "roles/{id:guid}/permissions/{permissionCode}")]
    public async Task<ClinicResult> RemovePermissionAsync(Guid id, string permissionCode, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemovePermissionFromRoleCommand(id, permissionCode), ct);
        return result.ToActionResult();
    }
}
