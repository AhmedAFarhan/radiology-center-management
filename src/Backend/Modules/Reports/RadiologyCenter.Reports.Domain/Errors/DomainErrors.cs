namespace RadiologyCenter.Reports.Domain.Errors;

/// <summary>
/// Stable semantic codes for domain-invariant violations. Thrown as
/// <see cref="DomainException"/> codes and resolved through the "codes"
/// section of the module JSON resource files.
/// </summary>
public static class DomainErrors
{
    public const string ReportContentDraftOnly = "Report.ReportContentDraftOnly";
    public const string FindingsRequired = "Report.FindingsRequired";
    public const string ImpressionRequired = "Report.ImpressionRequired";
    public const string SystemTemplateReadOnly = "Report.SystemTemplateReadOnly";
    public const string SectionLocked = "Report.SectionLocked";
    public const string InvalidStatusTransition = "Report.InvalidStatusTransition";
    public const string DuplicateTemplateSection = "Report.DuplicateTemplateSection";
    public const string DuplicateVersionSection = "Report.DuplicateVersionSection";
    public const string SectionNotOnTemplate = "Report.SectionNotOnTemplate";
    public const string FindingNotOnVersion = "Report.FindingNotOnVersion";
}
