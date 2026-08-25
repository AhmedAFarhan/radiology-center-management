using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Infrastructure.Repositories;
using RadiologyCenter.Payroll.Application.Abstractions;
using RadiologyCenter.Payroll.Domain.Entities;
using RadiologyCenter.Payroll.Infrastructure.Persistence;

namespace RadiologyCenter.Payroll.Infrastructure.Repositories;

public class PayRunRepository : BaseRepository<PayRun, Guid>, IPayRunRepository
{
    public PayRunRepository(PayrollDbContext context) : base(context)
    {
    }

    public async Task<PayRun?> GetWithPayslipsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(p => p.Payslips)
                .ThenInclude(ps => ps.Components)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<PayRun?> GetWithPayslipsAndReferralStatementsAsync(Guid id, CancellationToken ct = default) =>
        await DbSet
            .Include(p => p.Payslips)
                .ThenInclude(ps => ps.Components)
            .Include(p => p.ReferralFeeStatements)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<bool> ExistsOverlappingAsync(DateTime from, DateTime to, CancellationToken ct = default) =>
        await DbSet.AnyAsync(p => p.RunFrom <= to && p.RunTo >= from, ct);
}
