namespace RadiologyCenter.Reports.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    public const string ReportNotFound = "Report.ReportNotFound";
    public const string ReportTemplateNotFound = "Report.ReportTemplateNotFound";
    public const string ReportAlreadyExists = "Report.ReportAlreadyExists";
    public const string ExaminationNotCompleted = "Report.ExaminationNotCompleted";
    public const string SystemTemplateReadOnly = "Report.SystemTemplateReadOnly";
    public const string SystemTemplateCannotDelete = "Report.SystemTemplateCannotDelete";
    public const string TemplateNameExists = "Report.TemplateNameExists";
}
