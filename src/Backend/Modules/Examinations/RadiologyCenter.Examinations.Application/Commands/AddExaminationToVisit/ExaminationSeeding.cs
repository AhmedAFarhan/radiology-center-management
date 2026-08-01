using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationToVisit;

public static class ExaminationSeeding
{
    public static Examination Add(
        Visit visit,
        ExaminationType examinationType,
        string referringDoctor,
        string clinicalIndication,
        ExaminationPriority priority,
        string? notes,
        IReadOnlyList<AddExaminationToVisitItem>? items)
    {
        var examination = visit.AddExamination(
            examinationType.Id,
            referringDoctor,
            clinicalIndication,
            priority,
            notes);

        var resolvedItems = items ?? examinationType.Items
            .Select(i => new AddExaminationToVisitItem(i.ItemId, i.Quantity, i.IsContrast, i.IsRequired, i.Notes))
            .ToList();

        foreach (var item in resolvedItems)
            visit.AddExaminationItem(
                examination.Id,
                item.ItemId,
                item.Quantity,
                item.IsContrast,
                item.IsRequired,
                item.Notes);

        return examination;
    }
}
