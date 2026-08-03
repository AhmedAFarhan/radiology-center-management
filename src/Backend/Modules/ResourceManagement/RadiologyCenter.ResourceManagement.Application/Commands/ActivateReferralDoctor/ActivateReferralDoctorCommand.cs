namespace RadiologyCenter.ResourceManagement.Application.Commands.ActivateReferralDoctor;

public record ActivateReferralDoctorCommand(Guid ReferralDoctorId) : ICommand;
