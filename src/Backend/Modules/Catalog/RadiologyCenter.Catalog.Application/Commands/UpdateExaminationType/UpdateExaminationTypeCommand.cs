using RadiologyCenter.Catalog.Application.Commands.Common;

namespace RadiologyCenter.Catalog.Application.Commands.UpdateExaminationType;

public record UpdateExaminationTypeCommand(
    Guid ExaminationTypeId,
    string Name,
    string Modality,
    string BodyPart,
    int StandardDurationMinutes = 0,
    decimal Price = 0,
    bool RequiresPreparation = false,
    bool RequiresConsent = false) : ICommand, IExaminationTypeFields;