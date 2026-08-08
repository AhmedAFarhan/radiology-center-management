using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Insurance.Domain.Enumerations;

public sealed class PreAuthorizationStatus : Enumeration
{
    public static readonly PreAuthorizationStatus Requested = new(1, "Requested");
    public static readonly PreAuthorizationStatus Approved = new(2, "Approved");
    public static readonly PreAuthorizationStatus Denied = new(3, "Denied");
    public static readonly PreAuthorizationStatus Expired = new(4, "Expired");

    private PreAuthorizationStatus(int value, string name) : base(value, name) { }
}