using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Domain.ValueObjects;

public sealed record ExaminationTypeSnapshot(
    Guid ExaminationTypeId,
    string Code,
    string Name,
    Modality Modality,
    string BodyPart,
    decimal Price,
    int StandardDurationMinutes);
