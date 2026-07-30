namespace RadiologyCenter.Idnetity.Application.Commands.RemoveRoleFromUser;

public record RemoveRoleFromUserCommand(Guid UserId, Guid RoleId) : ICommand;
