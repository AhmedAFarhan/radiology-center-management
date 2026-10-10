using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Identity.Application.Commands.ActivateUser;
using static RadiologyCenter.Identity.Domain.Permissions;
using RadiologyCenter.Identity.Application.Commands.AssignRoleToUser;
using RadiologyCenter.Identity.Application.Commands.CreateUser;
using RadiologyCenter.Identity.Application.Commands.DeactivateUser;
using RadiologyCenter.Identity.Application.Commands.LockUser;
using RadiologyCenter.Identity.Application.Commands.RemoveRoleFromUser;
using RadiologyCenter.Identity.Application.Commands.ResetPassword;
using RadiologyCenter.Identity.Application.Commands.UnlockUser;
using RadiologyCenter.Identity.Application.Commands.UpdateUserProfile;
using RadiologyCenter.Identity.Application.Commands.UpdateUserRoles;
using RadiologyCenter.Identity.Application.DTOs;
using RadiologyCenter.Identity.Application.Queries.GetUserById;
using RadiologyCenter.Identity.Application.Queries.GetUsers;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;

namespace RadiologyCenter.Clinic.Operations.Identity;

public class UsersOperations : ClinicOperations
{
    private readonly IMessageBus _bus;
    private readonly RadiologyCenter.Clinic.Security.SafeIdentityReader _directory;

    public UsersOperations(IMessageBus bus,RadiologyCenter.Clinic.Security.SafeIdentityReader directory) { _bus=bus; _directory=directory; }

    [ClinicPermission(UsersReadCode)]
    [ClinicOperation("GET", "users/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        if(_directory.Enabled)
        {
            var user=(await _directory.ReadAsync<UserDto>("User",ct)).FirstOrDefault(x=>x.Id==id);
            return user is null ? NotFound() : Result.Success(user).ToActionResult();
        }
        var result = await _bus.InvokeAsync<Result<UserDto>>(new GetUserByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersReadCode)]
    [ClinicOperation("POST", "users/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        if(_directory.Enabled) return Result.Success(RadiologyCenter.Clinic.Security.SafeIdentityReader.Page(await _directory.ReadAsync<UserListItemDto>("User",ct),request)).ToActionResult();
        var result = await _bus.InvokeAsync<Result<PagedResult<UserListItemDto>>>(new GetUsersQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersCreateCode)]
    [ClinicOperation("POST", "users")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersUpdateCode)]
    [ClinicOperation("PUT", "users/{id:guid}/profile")]
    public async Task<ClinicResult> UpdateProfileAsync(Guid id, [FromBody] UpdateUserProfileCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersUpdateCode)]
    [ClinicOperation("POST", "users/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateUserCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersUpdateCode)]
    [ClinicOperation("POST", "users/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateUserCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersUpdateCode)]
    [ClinicOperation("POST", "users/{id:guid}/lock")]
    public async Task<ClinicResult> LockAsync(Guid id, [FromBody] LockUserCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersUpdateCode)]
    [ClinicOperation("POST", "users/{id:guid}/unlock")]
    public async Task<ClinicResult> UnlockAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new UnlockUserCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersUpdateCode)]
    [ClinicOperation("POST", "users/{id:guid}/reset-password")]
    public async Task<ClinicResult> ResetPasswordAsync(Guid id, [FromBody] ResetPasswordCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersManageRolesCode)]
    [ClinicOperation("POST", "users/{userId:guid}/roles")]
    public async Task<ClinicResult> AssignRoleAsync(Guid userId, [FromBody] AssignRoleToUserCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = userId }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersManageRolesCode)]
    [ClinicOperation("DELETE", "users/{userId:guid}/roles/{roleId:guid}")]
    public async Task<ClinicResult> RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemoveRoleFromUserCommand(userId, roleId), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(UsersManageRolesCode)]
    [ClinicOperation("PUT", "users/{userId:guid}/roles")]
    public async Task<ClinicResult> UpdateRolesAsync(Guid userId, [FromBody] UpdateUserRolesCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = userId }, ct);
        return result.ToActionResult();
    }
}
