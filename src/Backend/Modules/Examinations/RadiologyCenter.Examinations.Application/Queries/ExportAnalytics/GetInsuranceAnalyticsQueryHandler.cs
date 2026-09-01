using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.ExportAnalytics;

public static class GetInsuranceAnalyticsQueryHandler
{
    public static async Task<Result<InsuranceAnalyticsDto>> HandleAsync(
        GetInsuranceAnalyticsQuery query,
        IInsuranceAnalyticsDataSource dataSource,
        ITimezoneConverter timezone,
        CancellationToken ct)
    {
        var today = timezone.GetLocalDate(DateTime.UtcNow);
        var from = query.From?.Date ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
        var to = query.To?.Date.AddDays(1) ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var fromUtc = timezone.ToUtc(from);
        var toUtc = timezone.ToUtc(to);

        var totalClaims = await dataSource.GetTotalClaimsAsync(fromUtc, toUtc, ct);
        var draftClaims = await dataSource.GetClaimsByStatusAsync("Draft", fromUtc, toUtc, ct);
        var submittedClaims = await dataSource.GetClaimsByStatusAsync("Submitted", fromUtc, toUtc, ct);
        var approvedClaims = await dataSource.GetClaimsByStatusAsync("Approved", fromUtc, toUtc, ct);
        var rejectedClaims = await dataSource.GetClaimsByStatusAsync("Rejected", fromUtc, toUtc, ct);
        var paidClaims = await dataSource.GetClaimsByStatusAsync("Paid", fromUtc, toUtc, ct);
        var totalBilled = await dataSource.GetTotalBilledAsync(fromUtc, toUtc, ct);
        var totalPayerShare = await dataSource.GetTotalPayerShareAsync(fromUtc, toUtc, ct);
        var totalPatientShare = await dataSource.GetTotalPatientShareAsync(fromUtc, toUtc, ct);
        var totalSettled = await dataSource.GetTotalSettledAsync(fromUtc, toUtc, ct);
        var outstanding = await dataSource.GetOutstandingAmountAsync(fromUtc, toUtc, ct);
        var claimRows = await dataSource.GetClaimRowsAsync(fromUtc, toUtc, ct);

        var approvalRate = submittedClaims + approvedClaims + rejectedClaims == 0
            ? 0m
            : Math.Round((decimal)approvedClaims / (submittedClaims + approvedClaims + rejectedClaims), 4);

        return Result.Success(new InsuranceAnalyticsDto(
            totalClaims, draftClaims, submittedClaims, approvedClaims,
            rejectedClaims, paidClaims, totalBilled, totalPayerShare,
            totalPatientShare, totalSettled, outstanding, approvalRate, claimRows));
    }
}
