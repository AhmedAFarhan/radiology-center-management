namespace RadiologyCenter.Insurance.Application.Commands.Policies.UpdateCoverage;

public record UpdatePolicyCoverageCommand(
    Guid PolicyId,
    decimal CoveragePercent,
    DateTime? EffectiveTo = null) : ICommand;