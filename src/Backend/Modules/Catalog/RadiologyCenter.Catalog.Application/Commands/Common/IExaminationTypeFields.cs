namespace RadiologyCenter.Catalog.Application.Commands.Common;

public interface IExaminationTypeFields
{
    string Name { get; }
    string Modality { get; }
    string BodyPart { get; }
    int StandardDurationMinutes { get; }
    decimal Price { get; }
    bool RequiresPreparation { get; }
    bool RequiresConsent { get; }
}