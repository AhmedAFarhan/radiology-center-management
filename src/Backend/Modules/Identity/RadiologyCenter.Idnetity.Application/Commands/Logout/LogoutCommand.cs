namespace RadiologyCenter.Idnetity.Application.Commands.Logout;

public record LogoutCommand(Guid UserId, string? RefreshToken = null);
