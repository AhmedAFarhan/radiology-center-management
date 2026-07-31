namespace RadiologyCenter.Identity.Application.Commands.AddPermissionToRole;

public record AddPermissionToRoleCommand(Guid RoleId, string PermissionCode) : ICommand;
