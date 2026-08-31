using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Common;

namespace RadiologyCenter.Examinations.Application.Queries.GetMonthlyProfit;

public static class GetMonthlyProfitQueryHandler
{
    public static async Task<Result<ProfitAnalyticsDto>> HandleAsync(
        GetMonthlyProfitQuery query,
        IExaminationHistoryRepository historyRepository,
        IProfitSourceResolver profitSourceResolver,
        ITimezoneConverter timezone,
        CancellationToken ct)
    {
        var today = timezone.GetLocalDate(DateTime.UtcNow);
        var fromDate = query.From?.Date ?? today.AddMonths(-1).AddDays(1).ToDateTime(TimeOnly.MinValue);
        var toDate = query.To?.Date.AddDays(1) ?? today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var fromUtc = timezone.ToUtc(fromDate);
        var toUtc = timezone.ToUtc(toDate);
        var histories = await historyRepository.GetByCompletedRangeAsync(fromUtc, toUtc, ct);

        var collected = histories.Sum(h => h.Paid);
        var billed = histories.Sum(Billable);
        var discounts = histories.Sum(h => h.Price - Billable(h));
        var staffCaseFees = histories.Sum(h => (h.RadiologistFee ?? 0m) + (h.TechnicianFee ?? 0m));
        var referralFees = histories.Sum(h => h.ReferralFee ?? 0m);

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

    private static decimal Billable(Domain.Entities.ExaminationHistory h) =>
        ExaminationPricing.BillableAmount(h.Price, h.Discount, h.IsDiscountPercentage);
}