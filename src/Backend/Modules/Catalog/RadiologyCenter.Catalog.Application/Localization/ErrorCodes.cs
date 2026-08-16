namespace RadiologyCenter.Catalog.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    public const string ExaminationTypeNotFound = "Catalog.ExaminationTypeNotFound";
    public const string ExaminationTypeCodeExists = "Catalog.ExaminationTypeCodeExists";
    public const string ExaminationTypeInUse = "Catalog.ExaminationTypeInUse";
}
