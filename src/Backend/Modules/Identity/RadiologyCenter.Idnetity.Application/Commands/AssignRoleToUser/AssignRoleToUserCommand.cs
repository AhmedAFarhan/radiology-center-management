namespace RadiologyCenter.Idnetity.Application.Commands.AssignRoleToUser;

public record AssignRoleToUserCommand(Guid UserId, Guid RoleId) : ICommand;
