namespace RadiologyCenter.Payroll.Application.Abstractions;

public interface IStaffWorkHoursResolver
{
    Task<decimal> GetWorkedHoursAsync(Guid staffId, DateTime from, DateTime to, CancellationToken ct = default);
}