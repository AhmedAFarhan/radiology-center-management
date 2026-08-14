namespace RadiologyCenter.Examinations.Domain.Common;

public static class ExaminationPricing
{
    public const decimal PercentageCap = 100m;

    public static decimal DiscountValue(decimal price, decimal discount, bool isPercentage) =>
        isPercentage ? price * discount / PercentageCap : discount;

    public static decimal BillableAmount(decimal price, decimal discount, bool isPercentage) =>
        Math.Round(price - DiscountValue(price, discount, isPercentage), 2, MidpointRounding.AwayFromZero);
}
