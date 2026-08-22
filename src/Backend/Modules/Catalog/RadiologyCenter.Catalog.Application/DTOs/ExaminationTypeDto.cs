namespace RadiologyCenter.Catalog.Application.DTOs;

public record ExaminationTypeDto(
    Guid Id,
    string Code,
    string Name,
    string Modality,
    string BodyPart,
    int StandardDurationMinutes,
    decimal Price,
    bool RequiresPreparation,
    bool RequiresConsent,
    bool IsActive,
    DateTime CreatedAt,
    string ModalityKey = "");