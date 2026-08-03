using RadiologyCenter.ResourceManagement.Application.Commands.Common;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateEquipment;

public record UpdateEquipmentCommand(
    Guid EquipmentId,
    string Name,
    string Modality,
    string? SerialNumber = null,
    DateTime? PurchaseDate = null) : ICommand, IEquipmentFields;
