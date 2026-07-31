namespace RadiologyCenter.Identity.Application.Commands.Logout;

public record LogoutCommand(Guid UserId, string? RefreshToken = null) : ICommand;
