using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.ResourceManagement.Domain.Enumerations;

namespace RadiologyCenter.ResourceManagement.Domain.Entities;

public sealed class Equipment : SoftDeletableAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string? SerialNumber { get; private set; }
    public EquipmentModality Modality { get; private set; }
    public EquipmentStatus Status { get; private set; }
    public DateTime? PurchaseDate { get; private set; }
    public bool IsActive { get; private set; }

    private Equipment()
    {
        Name = null!;
        Modality = null!;
        Status = null!;
    }

    public static Equipment Create(
        string name,
        EquipmentModality modality,
        string? serialNumber = null,
        DateTime? purchaseDate = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNull(modality, nameof(modality));

        var equipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            SerialNumber = serialNumber?.Trim(),
            Modality = modality,
            Status = EquipmentStatus.Operational,
            PurchaseDate = purchaseDate,
            IsActive = true
        };

        return equipment;
    }

    public void Update(
        string name,
        EquipmentModality modality,
        string? serialNumber = null,
        DateTime? purchaseDate = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNull(modality, nameof(modality));

        Name = name.Trim();
        SerialNumber = serialNumber?.Trim();
        Modality = modality;
        PurchaseDate = purchaseDate;
    }

    public void SetStatus(EquipmentStatus status)
    {
        Guard.AgainstNull(status, nameof(status));
        Status = status;
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
    }
}
