namespace RadiologyCenter.ResourceManagement.Application.Commands.DeactivateReferralDoctor;

public record DeactivateReferralDoctorCommand(Guid ReferralDoctorId) : ICommand;
