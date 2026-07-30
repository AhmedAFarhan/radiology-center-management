namespace RadiologyCenter.Idnetity.Application.Commands.LockUser;

public record LockUserCommand(Guid UserId, DateTimeOffset LockoutEnd);
