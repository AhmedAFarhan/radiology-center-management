using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Reports.Domain.Enumerations;

public sealed class ReportSectionType : Enumeration
{
    public static readonly ReportSectionType ClinicalIndication = new(1, "ClinicalIndication");
    public static readonly ReportSectionType Technique = new(2, "Technique");
    public static readonly ReportSectionType Findings = new(3, "Findings");
    public static readonly ReportSectionType Impression = new(4, "Impression");
    public static readonly ReportSectionType Recommendation = new(5, "Recommendation");

    private ReportSectionType(int value, string name) : base(value, name) { }
}