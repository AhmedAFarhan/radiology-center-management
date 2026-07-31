namespace RadiologyCenter.Identity.Application.Commands.UpdateUserRoles;

public record UpdateUserRolesCommand(Guid UserId, IReadOnlyList<Guid> RoleIds) : ICommand;
