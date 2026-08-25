namespace RadiologyCenter.Payroll.Application.Abstractions;

public interface IPayRunRepository : IBaseRepository<PayRun, Guid>
{
    Task<PayRun?> GetWithPayslipsAsync(Guid id, CancellationToken ct = default);
    Task<PayRun?> GetWithPayslipsAndReferralStatementsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsOverlappingAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
