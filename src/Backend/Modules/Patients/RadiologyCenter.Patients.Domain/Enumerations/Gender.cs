using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Patients.Domain.Enumerations;

public sealed class Gender : Enumeration
{
    public static readonly Gender Male = new(1, "Male");
    public static readonly Gender Female = new(2, "Female");
    public static readonly Gender Other = new(3, "Other");

    private Gender(int value, string name) : base(value, name) { }
}
