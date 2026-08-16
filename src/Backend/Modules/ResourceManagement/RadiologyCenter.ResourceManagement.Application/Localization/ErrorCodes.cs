namespace RadiologyCenter.ResourceManagement.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    public const string WorkShiftEndAfterStart = "WorkShift.EndTimeAfterStart";
    public const string LeaveEndOnOrAfterStart = "Leave.EndDateOnOrAfterStart";
    public const string EquipmentNotFound = "ResourceManagement.EquipmentNotFound";
    public const string ReferralDoctorNotFound = "ResourceManagement.ReferralDoctorNotFound";
    public const string StaffNotFound = "ResourceManagement.StaffNotFound";
    public const string LeaveNotFound = "ResourceManagement.LeaveNotFound";
    public const string WorkShiftNotFound = "ResourceManagement.WorkShiftNotFound";
    public const string LeaveOverlap = "ResourceManagement.LeaveOverlap";
    public const string ReferralDoctorPhoneExists = "ResourceManagement.ReferralDoctorPhoneExists";
    public const string ResourceAlreadyBooked = "ResourceManagement.ResourceAlreadyBooked";
}
