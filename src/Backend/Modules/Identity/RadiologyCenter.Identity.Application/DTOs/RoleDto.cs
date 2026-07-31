namespace RadiologyCenter.Identity.Application.DTOs;

public record RoleDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsSystem,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyCollection<string> Permissions
);
