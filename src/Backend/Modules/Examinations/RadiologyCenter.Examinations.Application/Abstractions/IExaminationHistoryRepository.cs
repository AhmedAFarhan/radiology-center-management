namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IExaminationHistoryRepository : IBaseRepository<ExaminationHistory, Guid>
{
    Task<IReadOnlyList<ExaminationHistory>> GetByCompletedRangeAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
}
