using RadiologyCenter.ResourceManagement.Application.Commands.Common;

namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateReferralDoctor;

public record UpdateReferralDoctorCommand(
    Guid ReferralDoctorId,
    string FullName,
    string Phone,
    string? Email = null,
    string? Specialization = null,
    string? Hospital = null) : ICommand, IReferralDoctorFields;
