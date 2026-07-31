namespace RadiologyCenter.Idnetity.Application.Commands.RefreshToken;

public record RefreshTokenCommand(string Token) : ICommand;
