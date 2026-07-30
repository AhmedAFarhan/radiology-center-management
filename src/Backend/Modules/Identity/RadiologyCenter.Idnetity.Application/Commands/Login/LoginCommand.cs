namespace RadiologyCenter.Idnetity.Application.Commands.Login;

public record LoginCommand(string UserName, string Password) : ICommand;
