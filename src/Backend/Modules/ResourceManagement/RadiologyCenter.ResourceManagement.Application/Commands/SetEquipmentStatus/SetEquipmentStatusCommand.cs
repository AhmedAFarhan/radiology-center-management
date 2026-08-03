namespace RadiologyCenter.ResourceManagement.Application.Commands.SetEquipmentStatus;

public record SetEquipmentStatusCommand(Guid EquipmentId, string Status) : ICommand;
