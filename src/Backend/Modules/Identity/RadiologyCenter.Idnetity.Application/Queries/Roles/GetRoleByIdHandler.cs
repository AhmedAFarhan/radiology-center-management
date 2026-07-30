using FluentValidation;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.DTOs;

namespace RadiologyCenter.Idnetity.Application.Queries.Roles;

public record GetRoleByIdQuery(Guid Id);

public class GetRoleByIdValidator : AbstractValidator<GetRoleByIdQuery>
{
    public GetRoleByIdValidator() => RuleFor(x => x.Id).NotEmpty();
}

public static class GetRoleByIdHandler
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
