using Mapster;
using RadiologyCenter.Identity.Application.DTOs;
using RadiologyCenter.Identity.Domain.Entities;

namespace RadiologyCenter.Identity.Application;

public static class IdentityMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Role, RoleDto>.NewConfig()
            .Map(d => d.Permissions, s => s.Permissions.Select(p => p.Code).ToList());

        TypeAdapterConfig<User, UserListItemDto>.NewConfig()
            .Map(d => d.FirstName, s => s.FirstName)
            .Map(d => d.LastName, s => s.LastName);
    }
}
