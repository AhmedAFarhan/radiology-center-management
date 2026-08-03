using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.ResourceManagement.Domain.Enumerations;

public sealed class EquipmentStatus : Enumeration
{
    public static readonly EquipmentStatus Operational = new(1, "Operational");
    public static readonly EquipmentStatus UnderMaintenance = new(2, "UnderMaintenance");
    public static readonly EquipmentStatus OutOfService = new(3, "OutOfService");
    public static readonly EquipmentStatus Retired = new(4, "Retired");

    private EquipmentStatus(int value, string name) : base(value, name) { }
}
