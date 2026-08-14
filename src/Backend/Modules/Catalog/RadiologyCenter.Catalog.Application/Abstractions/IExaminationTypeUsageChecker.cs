namespace RadiologyCenter.Catalog.Application.Abstractions;

public interface IExaminationTypeUsageChecker
{
    Task<bool> HasActiveExaminationsAsync(Guid examinationTypeId, CancellationToken ct = default);
}