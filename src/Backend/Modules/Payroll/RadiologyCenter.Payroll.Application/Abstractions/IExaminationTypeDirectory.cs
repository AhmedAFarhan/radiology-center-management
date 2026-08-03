namespace RadiologyCenter.Payroll.Application.Abstractions;

public interface IExaminationTypeDirectory
{
    Task<bool> ExistsAsync(Guid examinationTypeId, CancellationToken ct = default);
}