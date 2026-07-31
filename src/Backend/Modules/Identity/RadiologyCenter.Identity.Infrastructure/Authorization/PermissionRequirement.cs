using Microsoft.AspNetCore.Authorization;

namespace RadiologyCenter.Identity.Infrastructure.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionCode { get; }

    public PermissionRequirement(string permissionCode) => PermissionCode = permissionCode;
}
