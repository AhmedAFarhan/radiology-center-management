using RadiologyCenter.Insurance.Domain.Entities;

namespace RadiologyCenter.Insurance.Application.Services;

public sealed record CoverageSplit(decimal PayerShare, decimal PatientShare);

public static class CoverageCalculationService
{
    public static CoverageSplit Split(InsurancePolicy policy, decimal billedAmount)
    {
        var payerShare = billedAmount * (policy.CoveragePercent / 100m);
        payerShare = Math.Max(0, payerShare);

        var patientShare = Math.Max(0, billedAmount - payerShare);

        return new CoverageSplit(payerShare, patientShare);
    }
}