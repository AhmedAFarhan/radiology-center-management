namespace RadiologyCenter.Identity.Application.Commands.AssignRoleToUser;

public record AssignRoleToUserCommand(Guid UserId, Guid RoleId) : ICommand;
