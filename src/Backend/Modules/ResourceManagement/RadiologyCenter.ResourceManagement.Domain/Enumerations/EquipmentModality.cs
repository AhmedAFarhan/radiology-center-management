using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.ResourceManagement.Domain.Enumerations;

public sealed class EquipmentModality : Enumeration
{
    public static readonly EquipmentModality XRay = new(1, "XRay");
    public static readonly EquipmentModality CT = new(2, "CT");
    public static readonly EquipmentModality MRI = new(3, "MRI");
    public static readonly EquipmentModality Ultrasound = new(4, "Ultrasound");
    public static readonly EquipmentModality Mammography = new(5, "Mammography");
    public static readonly EquipmentModality Fluoroscopy = new(6, "Fluoroscopy");
    public static readonly EquipmentModality DEXA = new(7, "DEXA");
    public static readonly EquipmentModality Other = new(8, "Other");

    private EquipmentModality(int value, string name) : base(value, name) { }
}
