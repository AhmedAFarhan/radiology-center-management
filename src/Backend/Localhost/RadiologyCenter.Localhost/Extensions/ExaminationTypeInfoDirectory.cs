using RadiologyCenter.Catalog.Application.Abstractions;
using RadiologyCenter.Catalog.Domain.Entities;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;

namespace RadiologyCenter.Localhost.Extensions;

public class ExaminationTypeInfoDirectory : IExaminationTypeDirectory
{
    private readonly IExaminationTypeRepository _examinationTypeRepository;
    private readonly IExaminationTypeItemRepository _examinationTypeItemRepository;

    public ExaminationTypeInfoDirectory(
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationTypeItemRepository examinationTypeItemRepository)
    {
        _examinationTypeRepository = examinationTypeRepository;
        _examinationTypeItemRepository = examinationTypeItemRepository;
    }

    public async Task<ExaminationTypeInfo?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
    {
        var type = await _examinationTypeRepository.GetByIdAsync(id, ct);
        if (type is null)
            return null;

        var items = await _examinationTypeItemRepository.GetByTypeIdAsync(id, ct);
        return ToInfo(type, items);
    }

    public async Task<ExaminationTypeInfo?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var type = await _examinationTypeRepository.GetByIdAsync(id, ct);
        return type is null ? null : ToInfo(type, Array.Empty<ExaminationTypeItem>());
    }

    public async Task<IReadOnlyList<ExaminationTypeInfo>> GetWithItemsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return Array.Empty<ExaminationTypeInfo>();

        var infos = new List<ExaminationTypeInfo>(idList.Count);
        foreach (var id in idList)
        {
            var type = await _examinationTypeRepository.GetByIdAsync(id, ct);
            if (type is null)
                continue;

            var items = await _examinationTypeItemRepository.GetByTypeIdAsync(id, ct);
            infos.Add(ToInfo(type, items));
        }

        return infos;
    }

    private static ExaminationTypeInfo ToInfo(ExaminationType type, IReadOnlyList<ExaminationTypeItem> items) => new(
        type.Id,
        type.Code,
        type.Name,
        type.Modality.Name,
        type.BodyPart,
        type.Price,
        type.StandardDurationMinutes,
        items
            .Select(i => new ExaminationTypeItemInfo(i.ItemId, i.Quantity, i.IsContrast, i.IsRequired))
            .ToList());
}