namespace RadiologyCenter.Identity.Application.DTOs;

public record TokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    DateTime RefreshTokenExpiresAt
);
