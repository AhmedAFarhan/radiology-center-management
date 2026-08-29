namespace RadiologyCenter.ResourceManagement.Application.Localization;

/// <summary>
/// Strongly-typed semantic error codes used as localization keys and as the
/// stable machine-readable identifier surfaced in API responses. Codes are
/// resolved through the "codes" section of the module JSON resource files,
/// falling back to the legacy message-text keys when absent.
/// </summary>
public static class ErrorCodes
{
    // Existing codes
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

    // ID Required
    public const string EquipmentIdRequired = "ResourceManagement.EquipmentIdRequired";
    public const string StaffIdRequired = "ResourceManagement.StaffIdRequired";
    public const string ReferralDoctorIdRequired = "ResourceManagement.ReferralDoctorIdRequired";
    public const string LeaveIdRequired = "ResourceManagement.LeaveIdRequired";
    public const string WorkShiftIdRequired = "ResourceManagement.WorkShiftIdRequired";
    public const string UserIdRequired = "ResourceManagement.UserIdRequired";

    // Field Required
    public const string EquipmentNameRequired = "ResourceManagement.EquipmentNameRequired";
    public const string ModalityRequired = "ResourceManagement.ModalityRequired";
    public const string FullNameRequired = "ResourceManagement.FullNameRequired";
    public const string PhoneNumberRequired = "ResourceManagement.PhoneNumberRequired";
    public const string PositionRequired = "ResourceManagement.PositionRequired";
    public const string HireDateRequired = "ResourceManagement.HireDateRequired";
    public const string LeaveTypeRequired = "ResourceManagement.LeaveTypeRequired";
    public const string StartDateRequired = "ResourceManagement.StartDateRequired";
    public const string EndDateRequired = "ResourceManagement.EndDateRequired";
    public const string DateRequired = "ResourceManagement.DateRequired";
    public const string StartTimeRequired = "ResourceManagement.StartTimeRequired";
    public const string EndTimeRequired = "ResourceManagement.EndTimeRequired";
    public const string PhoneRequired = "ResourceManagement.PhoneRequired";
    public const string StatusRequired = "ResourceManagement.StatusRequired";

    // Text Too Long
    public const string EquipmentNameTooLong = "ResourceManagement.EquipmentNameTooLong";
    public const string SerialNumberTooLong = "ResourceManagement.SerialNumberTooLong";
    public const string FullNameTooLong = "ResourceManagement.FullNameTooLong";
    public const string PhoneNumberTooLong = "ResourceManagement.PhoneNumberTooLong";
    public const string DepartmentTooLong = "ResourceManagement.DepartmentTooLong";
    public const string SpecializationTooLong = "ResourceManagement.SpecializationTooLong";
    public const string LicenseNumberTooLong = "ResourceManagement.LicenseNumberTooLong";
    public const string EmailTooLong = "ResourceManagement.EmailTooLong";
    public const string HospitalTooLong = "ResourceManagement.HospitalTooLong";
    public const string NotesTooLong = "ResourceManagement.NotesTooLong";
    public const string ReasonTooLong = "ResourceManagement.ReasonTooLong";
    public const string PhoneTooLong = "ResourceManagement.PhoneTooLong";
    public const string DescriptionTooLong = "ResourceManagement.DescriptionTooLong";

    // Other
    public const string InvalidEmail = "ResourceManagement.InvalidEmail";
}
