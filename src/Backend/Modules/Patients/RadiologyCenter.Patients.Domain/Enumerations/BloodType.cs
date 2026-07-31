using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Patients.Domain.Enumerations;

public sealed class BloodType : Enumeration
{
    public static readonly BloodType APositive = new(1, "A+");
    public static readonly BloodType ANegative = new(2, "A-");
    public static readonly BloodType BPositive = new(3, "B+");
    public static readonly BloodType BNegative = new(4, "B-");
    public static readonly BloodType ABPositive = new(5, "AB+");
    public static readonly BloodType ABNegative = new(6, "AB-");
    public static readonly BloodType OPositive = new(7, "O+");
    public static readonly BloodType ONegative = new(8, "O-");

    private BloodType(int value, string name) : base(value, name) { }
}
