namespace RadiologyCenter.Identity.Application.Commands.UnlockUser;

public record UnlockUserCommand(Guid UserId) : ICommand;
