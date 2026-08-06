using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Inventory.Domain.Enumerations;
using RadiologyCenter.Inventory.Infrastructure.Persistence;
using RadiologyCenter.Payroll.Infrastructure.Persistence;

namespace RadiologyCenter.Localhost.Extensions;

public class ProfitSourceResolver : IProfitSourceResolver
{
    private readonly PayrollDbContext _payroll;
    private readonly InventoryDbContext _inventory;

    public ProfitSourceResolver(PayrollDbContext payroll, InventoryDbContext inventory)
    {
        _payroll = payroll;
        _inventory = inventory;
    }

    public async Task<decimal> GetLaborCostForAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var payslipIds = await _payroll.Payslips
            .Where(ps => _payroll.PayRuns
                .Where(pr => pr.Id == ps.PayRunId && pr.RunFrom < to && pr.RunTo >= from)
                .Any())
            .Select(ps => ps.Id)
            .ToListAsync(ct);

        if (payslipIds.Count == 0)
            return 0m;

        var baseRows = await _payroll.Payslips
            .Where(ps => payslipIds.Contains(ps.Id))
            .Select(ps => new { ps.Id, ps.GrossSalary, ps.UnpaidLeaveDeduction })
            .ToListAsync(ct);

        var components = await _payroll.PayslipComponents
            .Where(c => payslipIds.Contains(c.PayslipId))
            .Select(c => new { c.PayslipId, c.Amount, c.IsDeduction })
            .ToListAsync(ct);

        var compAdjustment = components
            .GroupBy(c => c.PayslipId)
            .ToDictionary(g => g.Key, g => g.Sum(c => c.IsDeduction ? -c.Amount : c.Amount));

        return baseRows.Sum(r => r.GrossSalary - r.UnpaidLeaveDeduction +
            (compAdjustment.TryGetValue(r.Id, out var adj) ? adj : 0m));
    }

    public async Task<(decimal Cost, bool Tracked)> GetMaterialCostForAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        var total = await _inventory.StockMovements
            .Where(m => m.MovementType == StockMovementType.Receive
                        && m.UnitCost != null
                        && m.CreatedAt >= fromDate && m.CreatedAt < toDate)
            .SumAsync(m => (decimal?)m.Quantity * m.UnitCost, ct) ?? 0m;

        return (total, true);
    }
}