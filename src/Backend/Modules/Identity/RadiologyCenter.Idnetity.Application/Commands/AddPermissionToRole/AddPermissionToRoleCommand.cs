namespace RadiologyCenter.Idnetity.Application.Commands.AddPermissionToRole;

public record AddPermissionToRoleCommand(Guid RoleId, string PermissionCode);
