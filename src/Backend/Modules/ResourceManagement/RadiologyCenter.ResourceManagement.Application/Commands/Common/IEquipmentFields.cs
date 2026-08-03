namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public interface IEquipmentFields
{
    string Name { get; }
    string Modality { get; }
    string? SerialNumber { get; }
    DateTime? PurchaseDate { get; }
}
