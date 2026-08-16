namespace RadiologyCenter.Patients.Domain.Errors;

/// <summary>
/// Stable semantic codes for domain-invariant violations. Thrown as
/// <see cref="DomainException"/> codes and resolved through the "codes"
/// section of the module JSON resource files.
/// </summary>
public static class DomainErrors
{
    public const string DobOrAgeRequired = "Patient.DobOrAgeRequired";
    public const string DateOfBirthFuture = "Patient.DateOfBirthFuture";
    public const string AgeOutOfRange = "Patient.AgeOutOfRange";
}
