using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Insurance.Domain.Enumerations;

public sealed class DocumentType : Enumeration
{
    public static readonly DocumentType InsuranceCard = new(1, "InsuranceCard");
    public static readonly DocumentType NationalId = new(2, "NationalId");
    public static readonly DocumentType GovernmentApproval = new(3, "GovernmentApproval");
    public static readonly DocumentType Other = new(4, "Other");

    private DocumentType(int value, string name) : base(value, name) { }
}