using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Insurance.Domain.Enumerations;

public sealed class PolicyStatus : Enumeration
{
    public static readonly PolicyStatus Active = new(1, "Active");
    public static readonly PolicyStatus Inactive = new(2, "Inactive");
    public static readonly PolicyStatus Expired = new(3, "Expired");

    private PolicyStatus(int value, string name) : base(value, name) { }
}