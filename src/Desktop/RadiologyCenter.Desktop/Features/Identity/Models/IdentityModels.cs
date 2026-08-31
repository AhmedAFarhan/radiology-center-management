namespace RadiologyCenter.Desktop.Features.Identity.Models;

public sealed record UserDto(
    string Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsActive,
    bool EmailConfirmed,
    bool TwoFactorEnabled,
    bool LockoutEnabled,
    DateTimeOffset? LockoutEnd,
    DateTime? LastLoginAt,
    DateTime CreatedAt)
{
    public string FullName => string.Join(' ', new[] { FirstName, LastName }.Where(p => !string.IsNullOrWhiteSpace(p)));
    public bool IsLocked => LockoutEnd is { } end && end > DateTimeOffset.UtcNow;
}

public sealed record UserListItemDto(
    string Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsActive,
    DateTime? LastLoginAt,
    DateTime CreatedAt)
{
    public string FullName => string.Join(' ', new[] { FirstName, LastName }.Where(p => !string.IsNullOrWhiteSpace(p)));
    public bool IsLocked => false;
}

public sealed class CreateUserInput
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> RoleIds { get; set; } = new();
}

public sealed class UpdateUserProfileInput
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

public sealed class UpdateUserRolesInput
{
    public List<string> RoleIds { get; set; } = new();
}

public sealed class LockUserInput
{
    public DateTimeOffset LockoutEnd { get; set; }
}

public sealed record RoleDto(
    string Id,
    string Name,
    string? Description,
    bool IsSystem,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyCollection<string> Permissions);

public sealed class CreateRoleInput
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed class UpdateRoleInput
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public sealed record PermissionDto(
    string Code,
    string Name,
    string? Description,
    string? Group);