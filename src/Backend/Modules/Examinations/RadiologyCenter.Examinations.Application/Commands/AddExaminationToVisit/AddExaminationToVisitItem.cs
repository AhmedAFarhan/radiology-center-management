namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationToVisit;

public record AddExaminationToVisitItem(
    Guid ItemId,
    int Quantity,
    bool IsContrast = false,
    bool IsRequired = false,
    string? Notes = null);
