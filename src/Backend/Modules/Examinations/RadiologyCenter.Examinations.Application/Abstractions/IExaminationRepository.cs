using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IExaminationRepository : IBaseRepository<Examination, Guid>
{
    Task<Examination?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Examination>> GetPagedWithItemsAsync(QueryRequest request, CancellationToken ct = default);
    Task<bool> HasActiveExaminationsByTypeAsync(Guid examinationTypeId, CancellationToken ct = default);
    Task<IReadOnlyList<ExamFinancialProjection>> GetFinancialProjectionAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<IReadOnlyList<OperationalExamProjection>> GetOperationalProjectionAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
}
