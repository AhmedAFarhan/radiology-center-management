namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IVisitRepository : IBaseRepository<Visit, Guid>
{
    Task<Visit?> GetWithExaminationsAsync(Guid id, CancellationToken ct = default);
}
