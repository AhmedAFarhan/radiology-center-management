namespace RadiologyCenter.Payroll.Application.Abstractions;

public interface IPayrollStaffDirectory
{
    Task<bool> ExistsAsync(Guid staffId, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetActiveStaffIdsAsync(CancellationToken ct = default);
}