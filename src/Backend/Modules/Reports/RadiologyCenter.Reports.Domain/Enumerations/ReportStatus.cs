using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.Reports.Domain.Enumerations;

public sealed class ReportStatus : Enumeration
{
    public static readonly ReportStatus Draft = new(1, "Draft");
    public static readonly ReportStatus Finalized = new(2, "Finalized");
    public static readonly ReportStatus Cancelled = new(3, "Cancelled");

    private ReportStatus(int value, string name) : base(value, name) { }
}
