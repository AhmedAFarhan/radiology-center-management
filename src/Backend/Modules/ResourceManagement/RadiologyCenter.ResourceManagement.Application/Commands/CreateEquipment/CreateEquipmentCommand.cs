namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateEquipment;

public record CreateEquipmentCommand(
    string Name,
    string Modality,
    string? SerialNumber = null,
    DateTime? PurchaseDate = null) : ICommand;
