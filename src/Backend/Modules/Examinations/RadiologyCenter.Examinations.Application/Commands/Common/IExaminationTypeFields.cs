namespace RadiologyCenter.Examinations.Application.Commands.Common;

public interface IExaminationTypeFields<TItem> where TItem : IExaminationTypeItemFields
{
    string Code { get; }
    string Name { get; }
    string Modality { get; }
    string BodyPart { get; }
    int StandardDurationMinutes { get; }
    decimal Price { get; }
    bool RequiresPreparation { get; }
    bool RequiresConsent { get; }
    IReadOnlyList<TItem>? Items { get; }
}
