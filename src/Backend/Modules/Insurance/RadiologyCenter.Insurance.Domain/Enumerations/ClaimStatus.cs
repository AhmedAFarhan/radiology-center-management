using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Insurance.Domain.Enumerations;

public sealed class ClaimStatus : Enumeration
{
    public static readonly ClaimStatus Draft = new(1, "Draft");
    public static readonly ClaimStatus Submitted = new(2, "Submitted");
    public static readonly ClaimStatus Adjudicated = new(3, "Adjudicated");
    public static readonly ClaimStatus Approved = new(4, "Approved");
    public static readonly ClaimStatus Rejected = new(5, "Rejected");
    public static readonly ClaimStatus Paid = new(6, "Paid");

    private ClaimStatus(int value, string name) : base(value, name) { }
}