namespace RadiologyCenter.ResourceManagement.Application.Commands.Common;

public interface IReferralDoctorFields
{
    string FullName { get; }
    string Phone { get; }
    string? Email { get; }
    string? Specialization { get; }
    string? Hospital { get; }
}
