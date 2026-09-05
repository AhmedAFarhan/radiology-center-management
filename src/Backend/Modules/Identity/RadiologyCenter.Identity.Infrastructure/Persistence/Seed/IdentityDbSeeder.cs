using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Identity.Domain;
using RadiologyCenter.Identity.Domain.Entities;

namespace RadiologyCenter.Identity.Infrastructure.Persistence.Seed;

public static class IdentityDbSeeder
{
    public const string AdminRoleName = "Administrator";
    public const string AdminUserName = "admin123";
    public const string AdminPassword = "admin123";
    public const string AdminEmail = "admin@radiologycenter.local";
    public const string AdminPhone = "01000000000";

    public static async Task SeedAsync(
        IdentityDbContext context,
        IPasswordHasher<User> passwordHasher,
        string? resourcesPath = null,
        CancellationToken ct = default)
    {
        var permissions = await SeedPermissionsAsync(context, ct);

        if (!string.IsNullOrWhiteSpace(resourcesPath))
            await SeedPermissionTranslationsAsync(context, permissions, resourcesPath, ct);

        var adminRole = await context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Name == AdminRoleName, ct)
            ?? Role.Create(AdminRoleName, "System administrator with full access", isSystem: true);

        foreach (var permission in permissions)
            adminRole.AddPermission(permission);

        if (context.Entry(adminRole).State == EntityState.Detached)
            context.Roles.Add(adminRole);

        adminRole.ClearDomainEvents();
        await context.SaveChangesAsync(ct);

        var adminUser = await context.Users
            .Include(u => u.AssignedRoles)
            .FirstOrDefaultAsync(u => u.UserName == AdminUserName, ct);

        if (adminUser is null)
        {
            adminUser = User.Create(AdminUserName, AdminEmail, "Admin", "System", AdminPhone);
            adminUser.SetPasswordHash(passwordHasher.HashPassword(adminUser, AdminPassword));
            adminUser.ConfirmEmail();
            adminUser.AssignRole(adminRole);
            context.Users.Add(adminUser);
        }
        else if (!adminUser.HasRole(adminRole.Id))
        {
            adminUser.AssignRole(adminRole);
        }

        adminUser.ClearDomainEvents();
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

    private static async Task SeedPermissionTranslationsAsync(
        IdentityDbContext context,
        IReadOnlyList<Permission> permissions,
        string resourcesPath,
        CancellationToken ct)
    {
        if (!Directory.Exists(resourcesPath))
            return;

        var permissionsByCode = permissions.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);
        var existingTranslations = await context.PermissionTranslations.ToListAsync(ct);

        foreach (var file in Directory.GetFiles(resourcesPath, "*.json"))
        {
            var language = Path.GetFileNameWithoutExtension(file);
            if (string.IsNullOrWhiteSpace(language))
                continue;

            foreach (var (code, values) in ReadPermissionEntries(file))
            {
                if (!permissionsByCode.TryGetValue(code, out var permission))
                    continue;

                var name = values.GetValueOrDefault("name");
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var description = values.GetValueOrDefault("description");
                var group = values.GetValueOrDefault("group");

                var existing = existingTranslations.FirstOrDefault(t =>
                    t.PermissionId == permission.Id &&
                    t.Language.Equals(language, StringComparison.OrdinalIgnoreCase));

                if (existing is null)
                {
                    var translation = PermissionTranslation.Create(permission.Id, language, name, description, group);
                    context.PermissionTranslations.Add(translation);
                    existingTranslations.Add(translation);
                }
                else
                {
                    existing.Update(name, description, group);
                }
            }
        }

        await context.SaveChangesAsync(ct);
    }

    private static IEnumerable<KeyValuePair<string, IReadOnlyDictionary<string, string>>> ReadPermissionEntries(string file)
    {
        var result = new List<KeyValuePair<string, IReadOnlyDictionary<string, string>>>();

        try
        {
            using var stream = File.OpenRead(file);
            using var document = JsonDocument.Parse(stream);

            if (!document.RootElement.TryGetProperty("permissions", out var permissionsElement))
                return result;

            foreach (var property in permissionsElement.EnumerateObject())
            {
                var values = new Dictionary<string, string>(StringComparer.Ordinal);

                foreach (var field in new[] { "name", "description", "group" })
                {
                    if (property.Value.TryGetProperty(field, out var fieldElement) &&
                        fieldElement.ValueKind == JsonValueKind.String)
                    {
                        values[field] = fieldElement.GetString() ?? string.Empty;
                    }
                }

                result.Add(new KeyValuePair<string, IReadOnlyDictionary<string, string>>(property.Name, values));
            }
        }
        catch (JsonException)
        {
            // Ignore malformed resource files.
        }

        return result;
    }
}