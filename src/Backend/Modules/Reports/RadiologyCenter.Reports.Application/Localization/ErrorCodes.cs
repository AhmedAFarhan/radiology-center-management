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

    public const string ReportIdRequired = "Report.ReportIdRequired";
    public const string ExaminationIdRequired = "Report.ExaminationIdRequired";
    public const string TemplateIdRequired = "Report.TemplateIdRequired";
    public const string ReportVersionIdRequired = "Report.ReportVersionIdRequired";
    public const string SectionTypeRequired = "Report.SectionTypeRequired";
    public const string ContentRequired = "Report.ContentRequired";
    public const string NameRequired = "Report.NameRequired";
    public const string NameTooLong = "Report.NameTooLong";
    public const string RequestRequired = "Report.RequestRequired";
    public const string PageNumberMustBePositive = "Report.PageNumberMustBePositive";
    public const string FindingIdRequired = "Report.FindingIdRequired";
    public const string PatientIdRequired = "Report.PatientIdRequired";
    public const string RadiologistIdRequired = "Report.RadiologistIdRequired";
    public const string RegionRequired = "Report.RegionRequired";
    public const string RegionTooLong = "Report.RegionTooLong";
    public const string DescriptionRequired = "Report.DescriptionRequired";
    public const string DescriptionTooLong = "Report.DescriptionTooLong";
    public const string SeverityRequired = "Report.SeverityRequired";
    public const string ModalityRequired = "Report.ModalityRequired";
    public const string BodyPartTooLong = "Report.BodyPartTooLong";
    public const string TitleRequired = "Report.TitleRequired";
    public const string TitleTooLong = "Report.TitleTooLong";
    public const string BodyTooLong = "Report.BodyTooLong";
    public const string SectionRequired = "Report.SectionRequired";
}
