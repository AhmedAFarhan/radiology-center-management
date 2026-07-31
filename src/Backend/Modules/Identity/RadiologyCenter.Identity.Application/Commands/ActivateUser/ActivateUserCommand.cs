namespace RadiologyCenter.Identity.Application.Commands.ActivateUser;

public record ActivateUserCommand(Guid UserId) : ICommand;
