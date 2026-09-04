using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Common;
using RadiologyCenter.Examinations.Domain.Entities;

namespace RadiologyCenter.Examinations.Application.Queries.GetMonthlyProfit;

public static class GetMonthlyProfitQueryHandler
{
    public static async Task<Result<ProfitAnalyticsDto>> HandleAsync(
        GetMonthlyProfitQuery query,
        IExaminationRepository examinationRepository,
        IProfitSourceResolver profitSourceResolver,
        ITimezoneConverter timezone,
        CancellationToken ct)
    {
        var today = timezone.GetLocalDate(DateTime.UtcNow);
        var fromDate = query.From?.Date ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
        var toDate = query.To?.Date.AddDays(1) ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var fromUtc = timezone.ToUtc(fromDate);
        var toUtc = timezone.ToUtc(toDate);
        var examinations = await examinationRepository.GetCompletedByRangeAsync(fromUtc, toUtc, ct);

        var collected = examinations.Sum(e => e.Paid);
        var billed = examinations.Sum(Billable);
        var discounts = examinations.Sum(e => e.Price - Billable(e));
        var staffCaseFees = examinations.Sum(e => (e.RadiologistFee ?? 0m) + (e.TechnicianFee ?? 0m));
        var referralFees = examinations.Sum(e => e.ReferralFee ?? 0m);

        var laborCost = await profitSourceResolver.GetLaborCostForAsync(fromUtc, toUtc, ct);
        var (materialCost, materialTracked) = await profitSourceResolver.GetMaterialCostForAsync(fromUtc, toUtc, ct);

        var totalCosts = staffCaseFees + referralFees + laborCost + materialCost;
        var netProfit = collected - totalCosts;
        var netMargin = collected == 0m ? 0m : Math.Round(netProfit / collected, 4);

        return Result.Success(new ProfitAnalyticsDto(
            fromUtc,
            toUtc.AddDays(-1),
            Math.Round(collected, 2, MidpointRounding.AwayFromZero),
            Math.Round(billed, 2, MidpointRounding.AwayFromZero),
            Math.Round(discounts, 2, MidpointRounding.AwayFromZero),
            Math.Round(staffCaseFees, 2, MidpointRounding.AwayFromZero),
            Math.Round(referralFees, 2, MidpointRounding.AwayFromZero),
            Math.Round(laborCost, 2, MidpointRounding.AwayFromZero),
            true,
            Math.Round(materialCost, 2, MidpointRounding.AwayFromZero),
            materialTracked,
            Math.Round(totalCosts, 2, MidpointRounding.AwayFromZero),
            Math.Round(netProfit, 2, MidpointRounding.AwayFromZero),
            netMargin));
    }

    private static decimal Billable(Examination e) =>
        ExaminationPricing.BillableAmount(e.Price, e.Discount, e.IsDiscountPercentage);
}
