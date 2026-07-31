namespace RadiologyCenter.Idnetity.Application.DTOs;

public record TokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
