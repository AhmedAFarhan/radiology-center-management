namespace RadiologyCenter.Identity.Application.DTOs;

public record UserDto(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string TimeZoneId,
    bool IsActive,
    bool EmailConfirmed,
    bool TwoFactorEnabled,
    bool LockoutEnabled,
    DateTimeOffset? LockoutEnd,
    DateTime? LastLoginAt,
    DateTime CreatedAt
);
