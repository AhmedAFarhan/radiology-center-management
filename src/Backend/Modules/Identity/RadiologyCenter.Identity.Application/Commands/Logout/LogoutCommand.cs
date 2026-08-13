namespace RadiologyCenter.Identity.Application.Commands.Logout;

public record LogoutCommand(string? RefreshToken = null) : ICommand;
