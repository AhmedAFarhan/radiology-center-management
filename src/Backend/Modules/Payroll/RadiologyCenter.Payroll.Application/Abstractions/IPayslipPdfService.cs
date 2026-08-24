namespace RadiologyCenter.Payroll.Application.Abstractions;

public interface IPayslipPdfService
{
    Task<byte[]> GeneratePayslipPdfAsync(Guid payRunId, Guid staffId, CancellationToken ct = default);
}
