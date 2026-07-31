namespace RadiologyCenter.Identity.Application.Commands.DeactivateUser;

public record DeactivateUserCommand(Guid UserId) : ICommand;
