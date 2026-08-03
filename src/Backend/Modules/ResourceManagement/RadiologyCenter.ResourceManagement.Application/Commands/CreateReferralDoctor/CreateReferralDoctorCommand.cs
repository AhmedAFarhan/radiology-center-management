namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateReferralDoctor;

public record CreateReferralDoctorCommand(
    string FullName,
    string Phone,
    string? Email = null,
    string? Specialization = null,
    string? Hospital = null) : ICommand;
