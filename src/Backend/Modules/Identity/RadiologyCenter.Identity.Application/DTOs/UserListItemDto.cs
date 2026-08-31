namespace RadiologyCenter.Identity.Application.DTOs;

public record UserListItemDto(
    Guid Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsActive,
    DateTime? LastLoginAt,
    DateTime CreatedAt);
