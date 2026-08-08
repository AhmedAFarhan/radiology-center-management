namespace RadiologyCenter.Insurance.Application.Commands.Claims.CreateClaim;

public record CreateClaimCommand(
    Guid ExaminationId,
    Guid PatientId,
    Guid PolicyId,
    decimal BilledAmount,
    Guid? PreAuthorizationId = null) : ICommand;