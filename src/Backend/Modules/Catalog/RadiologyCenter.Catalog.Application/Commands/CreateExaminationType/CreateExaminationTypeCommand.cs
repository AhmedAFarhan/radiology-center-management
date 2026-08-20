using RadiologyCenter.Catalog.Application.Commands.Common;

namespace RadiologyCenter.Catalog.Application.Commands.CreateExaminationType;

public record CreateExaminationTypeCommand(
    string Name,
    string Modality,
    string BodyPart,
    int StandardDurationMinutes = 0,
    decimal Price = 0,
    bool RequiresPreparation = false,
    bool RequiresConsent = false) : ICommand, IExaminationTypeFields;