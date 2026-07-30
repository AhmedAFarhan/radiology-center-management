namespace RadiologyCenter.Idnetity.Application.Commands.RemovePermissionFromRole;

public record RemovePermissionFromRoleCommand(Guid RoleId, string PermissionCode) : ICommand;
