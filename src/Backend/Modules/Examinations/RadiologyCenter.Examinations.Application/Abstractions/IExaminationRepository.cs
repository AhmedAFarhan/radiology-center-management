namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IExaminationRepository : IBaseRepository<Examination, Guid>
{
    Task<bool> HasActiveExaminationsByTypeAsync(Guid examinationTypeId, CancellationToken ct = default);
}
