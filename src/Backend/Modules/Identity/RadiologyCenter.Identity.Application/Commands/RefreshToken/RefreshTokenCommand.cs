namespace RadiologyCenter.Identity.Application.Commands.RefreshToken;

public record RefreshTokenCommand(string Token) : ICommand;
