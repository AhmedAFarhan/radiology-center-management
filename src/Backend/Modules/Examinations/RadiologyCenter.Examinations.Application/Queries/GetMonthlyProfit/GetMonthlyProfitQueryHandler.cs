using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetMonthlyProfit;

public static class GetMonthlyProfitQueryHandler
{
    public static async Task<Result<ProfitAnalyticsDto>> HandleAsync(
        GetMonthlyProfitQuery query,
        IExaminationHistoryRepository historyRepository,
        IProfitSourceResolver profitSourceResolver,
        CancellationToken ct)
    {
        var from = query.From?.Date ?? DateTime.Today.AddMonths(-1).AddDays(1);
        var to = query.To?.Date.AddDays(1) ?? DateTime.Today.AddDays(1);

        var histories = await historyRepository.GetByCompletedRangeAsync(from, to, ct);

        var collected = histories.Sum(h => h.Paid);
        var billed = histories.Sum(Billable);
        var discounts = histories.Sum(h => h.Price - Billable(h));
        var staffCaseFees = histories.Sum(h => (h.RadiologistFee ?? 0m) + (h.TechnicianFee ?? 0m));
        var referralFees = histories.Sum(h => h.ReferralFee ?? 0m);

        var laborCost = await profitSourceResolver.GetLaborCostForAsync(from, to, ct);
        var (materialCost, materialTracked) = await profitSourceResolver.GetMaterialCostForAsync(from, to, ct);

        var totalCosts = staffCaseFees + referralFees + laborCost + materialCost;
        var netProfit = collected - totalCosts;
        var netMargin = collected == 0m ? 0m : Math.Round(netProfit / collected, 4);

        return Result.Success(new ProfitAnalyticsDto(
            from,
            to.AddDays(-1),
            Math.Round(collected, 2),
            Math.Round(billed, 2),
            Math.Round(discounts, 2),
            Math.Round(staffCaseFees, 2),
            Math.Round(referralFees, 2),
            Math.Round(laborCost, 2),
            true,
            Math.Round(materialCost, 2),
            materialTracked,
            Math.Round(totalCosts, 2),
            Math.Round(netProfit, 2),
            netMargin));
    }

    private static decimal Billable(Domain.Entities.ExaminationHistory h) =>
        h.IsDiscountPercentage
            ? Math.Round(h.Price * (1m - h.Discount / 100m), 2)
            : h.Price - h.Discount;
}