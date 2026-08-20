using Microsoft.AspNetCore.Authorization;

namespace RadiologyCenter.Desktop.Services.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionCode { get; }

    public PermissionRequirement(string permissionCode) => PermissionCode = permissionCode;
}