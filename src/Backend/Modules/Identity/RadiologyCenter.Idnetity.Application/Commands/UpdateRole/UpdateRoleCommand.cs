namespace RadiologyCenter.Idnetity.Application.Commands.UpdateRole;

public record UpdateRoleCommand(Guid RoleId, string Name, string? Description) : ICommand;
