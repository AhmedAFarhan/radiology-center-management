namespace RadiologyCenter.ResourceManagement.Application.Commands.DeactivateEquipment;

public record DeactivateEquipmentCommand(Guid EquipmentId) : ICommand;
