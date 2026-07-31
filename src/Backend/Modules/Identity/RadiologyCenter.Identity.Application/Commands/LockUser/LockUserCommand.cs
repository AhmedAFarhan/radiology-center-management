namespace RadiologyCenter.Identity.Application.Commands.LockUser;

public record LockUserCommand(Guid UserId, DateTimeOffset LockoutEnd) : ICommand;
