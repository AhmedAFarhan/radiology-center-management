using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Insurance.Domain.Enumerations;

public sealed class ClaimRejectionCode : Enumeration
{
    public static readonly ClaimRejectionCode MissingInformation = new(1, "MissingInformation");
    public static readonly ClaimRejectionCode PreAuthorizationNotApproved = new(2, "PreAuthorizationNotApproved");
    public static readonly ClaimRejectionCode PolicyNotActive = new(3, "PolicyNotActive");
    public static readonly ClaimRejectionCode CoverageExcluded = new(4, "CoverageExcluded");
    public static readonly ClaimRejectionCode LimitExceeded = new(5, "LimitExceeded");
    public static readonly ClaimRejectionCode DuplicateClaim = new(6, "DuplicateClaim");
    public static readonly ClaimRejectionCode Other = new(7, "Other");

    private ClaimRejectionCode(int value, string name) : base(value, name) { }
}