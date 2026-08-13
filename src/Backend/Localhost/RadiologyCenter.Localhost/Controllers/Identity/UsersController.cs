using Microsoft.AspNetCore.Mvc;
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
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;

namespace RadiologyCenter.Localhost.Controllers.Identity;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMessageBus _bus;

    public UsersController(IMessageBus bus) => _bus = bus;

    [HasPermission(UsersReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<UserDto>>(new GetUserByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<UserDto>>>(new GetUsersQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersUpdateCode)]
    [HttpPut("{id:guid}/profile")]
    public async Task<IActionResult> UpdateProfileAsync(Guid id, [FromBody] UpdateUserProfileCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersUpdateCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateUserCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersUpdateCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateUserCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersUpdateCode)]
    [HttpPost("{id:guid}/lock")]
    public async Task<IActionResult> LockAsync(Guid id, [FromBody] LockUserCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersUpdateCode)]
    [HttpPost("{id:guid}/unlock")]
    public async Task<IActionResult> UnlockAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new UnlockUserCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersUpdateCode)]
    [HttpPost("{id:guid}/reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(Guid id, [FromBody] ResetPasswordCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersManageRolesCode)]
    [HttpPost("{userId:guid}/roles")]
    public async Task<IActionResult> AssignRoleAsync(Guid userId, [FromBody] AssignRoleToUserCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = userId }, ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersManageRolesCode)]
    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemoveRoleFromUserCommand(userId, roleId), ct);
        return result.ToActionResult();
    }

    [HasPermission(UsersManageRolesCode)]
    [HttpPut("{userId:guid}/roles")]
    public async Task<IActionResult> UpdateRolesAsync(Guid userId, [FromBody] UpdateUserRolesCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = userId }, ct);
        return result.ToActionResult();
    }
}
