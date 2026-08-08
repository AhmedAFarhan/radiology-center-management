namespace RadiologyCenter.Insurance.Application.Commands.Policies.CreateInsurancePolicy;

public record CreateInsurancePolicyCommand(
    Guid CompanyId,
    Guid PatientId,
    string PolicyNumber,
    decimal CoveragePercent,
    DateTime EffectiveFrom,
    DateTime? EffectiveTo = null) : ICommand;