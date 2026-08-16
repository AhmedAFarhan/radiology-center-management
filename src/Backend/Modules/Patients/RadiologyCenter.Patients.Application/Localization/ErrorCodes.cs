namespace RadiologyCenter.Patients.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    public const string NameRequired = "Patient.NameRequired";
    public const string DobOrAgeRequired = "Patient.DobOrAgeRequired";
    public const string DateOfBirthFuture = "Patient.DateOfBirthFuture";
    public const string AgeOutOfRange = "Patient.AgeOutOfRange";
    public const string BloodTypeInvalid = "Patient.BloodTypeInvalid";
    public const string PatientNotFound = "Patient.PatientNotFound";
}
