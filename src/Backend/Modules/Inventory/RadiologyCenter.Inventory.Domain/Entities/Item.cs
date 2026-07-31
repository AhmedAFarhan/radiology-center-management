using RadiologyCenter.BuildingBlocks.Domain.Common;
using RadiologyCenter.BuildingBlocks.Domain.SoftDeletable;
using RadiologyCenter.Inventory.Domain.Enumerations;
using RadiologyCenter.Inventory.Domain.Events;

namespace RadiologyCenter.Inventory.Domain.Entities;

public sealed class Item : SoftDeletableAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string? Brand { get; private set; }
    public ItemCategory Category { get; private set; }
    public UnitType Unit { get; private set; }
    public int ReorderLevel { get; private set; }
    public int ReorderQuantity { get; private set; }
    public bool LotTracked { get; private set; }
    public string? StorageInstructions { get; private set; }
    public bool IsActive { get; private set; }

    private Item()
    {
        Name = null!;
        Category = null!;
        Unit = null!;
    }

    public static Item Create(
        string name,
        ItemCategory category,
        UnitType unit,
        string? brand = null,
        int reorderLevel = 0,
        int reorderQuantity = 0,
        bool lotTracked = false,
        string? storageInstructions = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNull(category, nameof(category));
        Guard.AgainstNull(unit, nameof(unit));
        Guard.Against(reorderLevel, r => r < 0, "Reorder level cannot be negative.");
        Guard.Against(reorderQuantity, r => r < 0, "Reorder quantity cannot be negative.");

        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Brand = brand?.Trim(),
            Category = category,
            Unit = unit,
            ReorderLevel = reorderLevel,
            ReorderQuantity = reorderQuantity,
            LotTracked = lotTracked,
            StorageInstructions = storageInstructions?.Trim(),
            IsActive = true
        };

        item.RaiseDomainEvent(new ItemCreatedEvent(item.Id, item.Name));
        return item;
    }

    public void Update(
        string name,
        ItemCategory category,
        UnitType unit,
        string? brand = null,
        int reorderLevel = 0,
        int reorderQuantity = 0,
        bool lotTracked = false,
        string? storageInstructions = null)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNull(category, nameof(category));
        Guard.AgainstNull(unit, nameof(unit));
        Guard.Against(reorderLevel, r => r < 0, "Reorder level cannot be negative.");
        Guard.Against(reorderQuantity, r => r < 0, "Reorder quantity cannot be negative.");

        Name = name.Trim();
        Brand = brand?.Trim();
        Category = category;
        Unit = unit;
        ReorderLevel = reorderLevel;
        ReorderQuantity = reorderQuantity;
        LotTracked = lotTracked;
        StorageInstructions = storageInstructions?.Trim();

        RaiseDomainEvent(new ItemUpdatedEvent(Id));
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
