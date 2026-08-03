namespace RadiologyCenter.ResourceManagement.Application.Commands.DeleteReferralDoctor;

public record DeleteReferralDoctorCommand(Guid ReferralDoctorId) : ICommand;
