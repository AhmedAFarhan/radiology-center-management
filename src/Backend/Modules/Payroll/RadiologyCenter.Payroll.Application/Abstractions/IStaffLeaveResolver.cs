namespace RadiologyCenter.Payroll.Application.Abstractions;

public interface IStaffLeaveResolver
{
    Task<int> GetUnpaidLeaveDaysAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default);
}