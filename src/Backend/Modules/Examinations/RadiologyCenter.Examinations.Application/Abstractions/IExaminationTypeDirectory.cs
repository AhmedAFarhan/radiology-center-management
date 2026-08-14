namespace RadiologyCenter.Examinations.Application.Abstractions;

public sealed record ExaminationTypeItemInfo(
    Guid ItemId,
    int Quantity,
    bool IsContrast,
    bool IsRequired);

public sealed record ExaminationTypeInfo(
    Guid Id,
    string Code,
    string Name,
    string Modality,
    string BodyPart,
    decimal Price,
    int StandardDurationMinutes,
    IReadOnlyList<ExaminationTypeItemInfo> Items);

public interface IExaminationTypeDirectory
{
    Task<ExaminationTypeInfo?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<ExaminationTypeInfo?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ExaminationTypeInfo>> GetWithItemsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}