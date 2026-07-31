using Mapster;
using RadiologyCenter.Identity.Application.Abstractions;
using RadiologyCenter.Identity.Application.DTOs;

namespace RadiologyCenter.Identity.Application.Queries.GetRoleById;

public static class GetRoleByIdQueryHandler
{
    public static async Task<Result<RoleDto>> HandleAsync(
        GetRoleByIdQuery query,
        IRoleRepository roleRepository,
        CancellationToken ct)
    {
        var role = await roleRepository.GetByIdAsync(query.Id, ct);
        if (role is null)
            return Result.Failure<RoleDto>(Error.NotFound("Role", query.Id));

        return Result.Success(role.Adapt<RoleDto>());
    }
}
