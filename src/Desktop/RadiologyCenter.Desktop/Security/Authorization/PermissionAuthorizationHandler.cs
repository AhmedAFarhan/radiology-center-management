using Microsoft.AspNetCore.Authorization;

namespace RadiologyCenter.Desktop.Security.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.HasClaim(AppClaimTypes.IsAdmin, "true"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (context.User.HasClaim(AppClaimTypes.Permission, requirement.PermissionCode))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}