using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.DTOs;

namespace RadiologyCenter.Idnetity.Application.Queries.GetRoleById;

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

        return Result.Success(Map(role));
    }

    private static RoleDto Map(Role role) => new(
        role.Id,
        role.Name!,
        role.Description,
        role.IsSystem,
        role.IsActive,
        role.CreatedAt,
        role.Permissions.Select(p => p.Code).ToList()
    );
}
