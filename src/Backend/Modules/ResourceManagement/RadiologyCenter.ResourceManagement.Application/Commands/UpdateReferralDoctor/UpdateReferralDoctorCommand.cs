namespace RadiologyCenter.ResourceManagement.Application.Commands.UpdateReferralDoctor;

public record UpdateReferralDoctorCommand(
    Guid ReferralDoctorId,
    string Name,
    string Phone,
    string? Email = null,
    string? Specialization = null,
    string? Hospital = null) : ICommand;
