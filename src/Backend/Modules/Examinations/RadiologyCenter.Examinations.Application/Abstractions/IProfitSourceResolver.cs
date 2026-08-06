namespace RadiologyCenter.Examinations.Application.Abstractions;

/// <summary>
/// Resolves the cost side of a monthly profit statement (labor and consumable materials)
/// across the Payroll and Inventory modules for the analytics read side.
/// </summary>
public interface IProfitSourceResolver
{
    Task<decimal> GetLaborCostForAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<(decimal Cost, bool Tracked)> GetMaterialCostForAsync(DateTime from, DateTime to, CancellationToken ct = default);
}