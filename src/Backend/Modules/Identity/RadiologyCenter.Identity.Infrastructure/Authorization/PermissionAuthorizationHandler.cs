using Microsoft.AspNetCore.Authorization;

namespace RadiologyCenter.Identity.Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.HasClaim("isAdmin", "true"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (context.User.HasClaim("permission", requirement.PermissionCode))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
