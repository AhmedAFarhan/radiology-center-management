namespace RadiologyCenter.ResourceManagement.Application.Commands.DeleteEquipment;

public record DeleteEquipmentCommand(Guid EquipmentId) : ICommand;
