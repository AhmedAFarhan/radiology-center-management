namespace RadiologyCenter.Examinations.Application.Commands.Common;

public interface IExaminationTypeItemFields
{
    Guid ItemId { get; }
    int Quantity { get; }
    bool IsContrast { get; }
    bool IsRequired { get; }
    string? Notes { get; }
}
