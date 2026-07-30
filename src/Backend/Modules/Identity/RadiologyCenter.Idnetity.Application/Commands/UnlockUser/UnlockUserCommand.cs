namespace RadiologyCenter.Idnetity.Application.Commands.UnlockUser;

public record UnlockUserCommand(Guid UserId) : ICommand;
