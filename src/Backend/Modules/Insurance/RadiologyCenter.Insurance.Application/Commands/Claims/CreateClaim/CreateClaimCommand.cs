namespace RadiologyCenter.Insurance.Application.Commands.Claims.CreateClaim;

public record CreateClaimCommand(
    Guid ExaminationId,
    Guid PatientId,
    Guid PolicyId,
    Guid PreAuthorizationId,
    decimal BilledAmount) : ICommand;