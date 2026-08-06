namespace RadiologyCenter.Examinations.Application.DTOs;

public record ProfitAnalyticsDto(
    DateTime From,
    DateTime To,
    decimal RevenueCollected,
    decimal TotalBilled,
    decimal Discounts,
    decimal StaffCaseFees,
    decimal ReferralFees,
    decimal LaborCosts,
    bool LaborCostsTracked,
    decimal MaterialCosts,
    bool MaterialCostsTracked,
    decimal TotalCosts,
    decimal NetProfit,
    decimal NetMargin);