using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Idnetity.Application.Abstractions;
using RadiologyCenter.Idnetity.Application.DTOs;

namespace RadiologyCenter.Idnetity.Application.Queries.Roles;

public record GetRolesQuery(QueryRequest Request);

public static class GetRolesHandler
{
    public static async Task<Result<PagedResult<RoleDto>>> HandleAsync(
        GetRolesQuery query,
        IRoleRepository roleRepository,
        CancellationToken ct)
    {
        var paged = await roleRepository.GetPagedAsync(query.Request, ct);
        var dtos = paged.Items.Select(Map).ToList();

        return Result.Success(new PagedResult<RoleDto>(
            dtos,
            paged.PageNumber,
            paged.PageSize,
            paged.TotalCount
        ));
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
