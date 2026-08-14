using RadiologyCenter.Catalog.Domain.Enumerations;

namespace RadiologyCenter.Catalog.Domain.ValueObjects;

public sealed record ExaminationTypeSnapshot(
    Guid ExaminationTypeId,
    string Code,
    string Name,
    Modality Modality,
    string BodyPart,
    decimal Price,
    int StandardDurationMinutes);
