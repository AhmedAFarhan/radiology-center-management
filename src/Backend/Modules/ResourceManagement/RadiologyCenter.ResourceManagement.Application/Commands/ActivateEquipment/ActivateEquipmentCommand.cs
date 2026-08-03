namespace RadiologyCenter.ResourceManagement.Application.Commands.ActivateEquipment;

public record ActivateEquipmentCommand(Guid EquipmentId) : ICommand;
