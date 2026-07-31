using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Inventory.Domain.Enumerations;

public sealed class ItemCategory : Enumeration
{
    public static readonly ItemCategory ContrastMedia = new(1, "ContrastMedia");
    public static readonly ItemCategory Drug = new(2, "Drug");
    public static readonly ItemCategory MedicalSupply = new(3, "MedicalSupply");
    public static readonly ItemCategory Consumable = new(4, "Consumable");
    public static readonly ItemCategory Other = new(5, "Other");

    private ItemCategory(int value, string name) : base(value, name) { }
}
