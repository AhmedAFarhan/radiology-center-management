using RadiologyCenter.Insurance.Domain.Entities;

namespace RadiologyCenter.Insurance.Application.Services;

public sealed record CoverageSplit(decimal PayerShare, decimal PatientShare, decimal CopayApplied);

public static class CoverageCalculationService
{
    public static CoverageSplit Split(InsurancePolicy policy, decimal billedAmount)
    {
        var payerShare = billedAmount * (policy.CoveragePercent / 100m) - policy.Deductible;
        payerShare = Math.Max(0, payerShare);

        var patientShare = Math.Min(policy.Copay, billedAmount - payerShare);
        patientShare = Math.Max(0, patientShare);

        var copayApplied = Math.Min(policy.Copay, patientShare);

        return new CoverageSplit(payerShare, patientShare, copayApplied);
    }
}