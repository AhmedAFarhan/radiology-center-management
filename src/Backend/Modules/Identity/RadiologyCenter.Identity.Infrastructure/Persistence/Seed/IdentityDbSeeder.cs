using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Identity.Domain;
using RadiologyCenter.Identity.Domain.Entities;

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Seed;

public static class IdentityDbSeeder
{
    public const string AdminRoleName = "Administrator";
    public const string AdminUserName = "admin123";
    private const string AdminPassword = "admin123";

    public static async Task SeedAsync(IdentityDbContext context, IPasswordHasher<User> passwordHasher, CancellationToken ct = default)
    {
        var permissions = await SeedPermissionsAsync(context, ct);

        var adminRole = await context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Name == AdminRoleName, ct)
            ?? Role.Create(AdminRoleName, "System administrator with full access", isSystem: true);

        foreach (var permission in permissions)
            adminRole.AddPermission(permission);

        if (context.Entry(adminRole).State == EntityState.Detached)
            context.Roles.Add(adminRole);

        await context.SaveChangesAsync(ct);

        var adminUser = await context.Users
            .Include(u => u.AssignedRoles)
            .FirstOrDefaultAsync(u => u.UserName == AdminUserName, ct);

        if (adminUser is null)
        {
            adminUser = User.Create(AdminUserName, "admin@radiologycenter.local", "Admin", "System");
            adminUser.SetPasswordHash(passwordHasher.HashPassword(adminUser, AdminPassword));
            adminUser.ConfirmEmail();
            adminUser.AssignRole(adminRole);
            context.Users.Add(adminUser);
        }
        else if (!adminUser.HasRole(adminRole.Id))
        {
            adminUser.AssignRole(adminRole);
        }

        await context.SaveChangesAsync(ct);
    }

    private static async Task<IReadOnlyList<Permission>> SeedPermissionsAsync(IdentityDbContext context, CancellationToken ct)
    {
        var existing = await context.Permissions.Select(p => p.Code).ToListAsync(ct);
        var missing = Permissions.All.Where(p => !existing.Contains(p.Code)).ToList();

        if (missing.Count > 0)
        {
            context.Permissions.AddRange(missing);
            await context.SaveChangesAsync(ct);
        }

        return await context.Permissions.ToListAsync(ct);
    }
}
