namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.CreatePreAuthorization;

public record CreatePreAuthorizationCommand(
    Guid ExaminationId,
    Guid PatientId,
    Guid PolicyId,
    decimal EstimatedAmount) : ICommand;