namespace RadiologyCenter.ResourceManagement.Application.Commands.CreateReferralDoctor;

public record CreateReferralDoctorCommand(
    string Name,
    string Phone,
    string? Email = null,
    string? Specialization = null,
    string? Hospital = null) : ICommand;
