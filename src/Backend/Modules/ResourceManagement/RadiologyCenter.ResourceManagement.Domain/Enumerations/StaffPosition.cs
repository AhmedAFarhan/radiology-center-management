using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.ResourceManagement.Domain.Enumerations;

public sealed class StaffPosition : Enumeration
{
    public static readonly StaffPosition Technician = new(1, "Technician");
    public static readonly StaffPosition Radiologist = new(2, "Radiologist");
    public static readonly StaffPosition Receptionist = new(3, "Receptionist");
    public static readonly StaffPosition Nurse = new(4, "Nurse");
    public static readonly StaffPosition Other = new(5, "Other");

    private StaffPosition(int value, string name) : base(value, name) { }
}
