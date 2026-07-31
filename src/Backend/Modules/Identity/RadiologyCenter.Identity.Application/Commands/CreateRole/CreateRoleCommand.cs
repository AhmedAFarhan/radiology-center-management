namespace RadiologyCenter.Identity.Application.Commands.CreateRole;

public record CreateRoleCommand(string Name, string? Description, bool IsSystem = false) : ICommand;
