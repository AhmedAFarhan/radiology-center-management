using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Domain.Entities;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Localhost.Extensions;

public class ExaminationTypeInfoDirectory : IExaminationTypeDirectory
{
    private readonly IExaminationTypeRepository _examinationTypeRepository;

    public ExaminationTypeInfoDirectory(IExaminationTypeRepository examinationTypeRepository)
        => _examinationTypeRepository = examinationTypeRepository;

    public async Task<ExaminationTypeInfo?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
    {
        var type = await _examinationTypeRepository.GetWithItemsAsync(id, ct);
        return type is null ? null : ToInfo(type);
    }

    public async Task<ExaminationTypeInfo?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var type = await _examinationTypeRepository.GetByIdAsync(id, ct);
        return type is null ? null : ToInfo(type);
    }

    public async Task<IReadOnlyList<ExaminationTypeInfo>> GetWithItemsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var types = await _examinationTypeRepository.GetWithItemsByIdsAsync(ids, ct);
        return types.Select(ToInfo).ToList();
    }

    private static ExaminationTypeInfo ToInfo(ExaminationType type) => new(
        type.Id,
        type.Code,
        type.Name,
        type.Modality.Name,
        type.BodyPart,
        type.Price,
        type.StandardDurationMinutes,
        type.Items
            .Select(i => new ExaminationTypeItemInfo(i.ItemId, i.Quantity, i.IsContrast, i.IsRequired))
            .ToList());
}