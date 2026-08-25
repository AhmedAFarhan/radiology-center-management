using RadiologyCenter.Payroll.Application.Abstractions;

namespace RadiologyCenter.Payroll.Application.Services;

public class ReferralFeeStatementCalculator : IReferralFeeStatementCalculator
{
    private readonly IReferralFeeStatementResolver _resolver;

    public ReferralFeeStatementCalculator(IReferralFeeStatementResolver resolver)
        => _resolver = resolver;

    public async Task<IReadOnlyList<ReferralFeeStatementDraft>> CalculateAllAsync(
        DateTime from,
        DateTime to,
        CancellationToken ct = default)
    {
        var breakdowns = await _resolver.GetReferralFeeBreakdownsAsync(from, to, ct);

        return breakdowns
            .Where(b => b.TotalFee > 0)
            .Select(b => new ReferralFeeStatementDraft(b.ReferralDoctorId, b.TotalFee, b.ExamCount))
            .OrderByDescending(d => d.TotalFee)
            .ToList();
    }
}
