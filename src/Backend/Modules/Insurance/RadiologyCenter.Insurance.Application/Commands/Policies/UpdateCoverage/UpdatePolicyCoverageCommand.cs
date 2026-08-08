namespace RadiologyCenter.Insurance.Application.Commands.Policies.UpdateCoverage;

public record UpdatePolicyCoverageCommand(
    Guid PolicyId,
    decimal CoveragePercent,
    decimal Deductible,
    decimal Copay,
    DateTime? EffectiveTo = null) : ICommand;