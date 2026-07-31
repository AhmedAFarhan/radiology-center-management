using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Idnetity.Application.Commands.ActivateUser;
using RadiologyCenter.Idnetity.Application.Commands.AssignRoleToUser;
using RadiologyCenter.Idnetity.Application.Commands.CreateUser;
using RadiologyCenter.Idnetity.Application.Commands.DeactivateUser;
using RadiologyCenter.Idnetity.Application.Commands.LockUser;
using RadiologyCenter.Idnetity.Application.Commands.RemoveRoleFromUser;
using RadiologyCenter.Idnetity.Application.Commands.UnlockUser;
using RadiologyCenter.Idnetity.Application.Commands.UpdateUserProfile;
using RadiologyCenter.Idnetity.Application.DTOs;
using RadiologyCenter.Idnetity.Application.Queries.GetUserById;
using RadiologyCenter.Idnetity.Application.Queries.GetUsers;
using Wolverine;

namespace RadiologyCenter.Localhost.Controllers.Identity;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMessageBus _bus;

    public UsersController(IMessageBus bus) => _bus = bus;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<UserDto>>(new GetUserByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<UserDto>>>(new GetUsersQuery(request), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command, ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}/profile")]
    public async Task<IActionResult> UpdateProfileAsync(Guid id, [FromBody] UpdateUserProfileCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = id }, ct);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateUserCommand(id), ct);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateUserCommand(id), ct);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/lock")]
    public async Task<IActionResult> LockAsync(Guid id, [FromBody] LockUserCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = id }, ct);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/unlock")]
    public async Task<IActionResult> UnlockAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new UnlockUserCommand(id), ct);
        return result.ToActionResult();
    }

    [HttpPost("{userId:guid}/roles")]
    public async Task<IActionResult> AssignRoleAsync(Guid userId, [FromBody] AssignRoleToUserCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { UserId = userId }, ct);
        return result.ToActionResult();
    }

    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    public async Task<IActionResult> RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new RemoveRoleFromUserCommand(userId, roleId), ct);
        return result.ToActionResult();
    }
}
