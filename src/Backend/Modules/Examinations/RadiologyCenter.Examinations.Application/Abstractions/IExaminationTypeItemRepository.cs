using RadiologyCenter.Examinations.Domain.Entities;

namespace RadiologyCenter.Examinations.Application.Abstractions;

public interface IExaminationTypeItemRepository
{
    Task<IReadOnlyList<ExaminationTypeItem>> GetByTypeIdAsync(Guid examinationTypeId, CancellationToken ct = default);
    Task<ExaminationTypeItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByItemAsync(Guid examinationTypeId, Guid itemId, CancellationToken ct = default);
    Task AddAsync(ExaminationTypeItem item, CancellationToken ct = default);
    void Remove(ExaminationTypeItem item);
}