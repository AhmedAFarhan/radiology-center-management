namespace RadiologyCenter.Identity.Application.Commands.RemoveRoleFromUser;

public record RemoveRoleFromUserCommand(Guid UserId, Guid RoleId) : ICommand;
