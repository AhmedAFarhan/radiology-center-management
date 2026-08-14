using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.CreateExamination;

internal sealed record SeededExaminationItem(Guid ItemId, int Quantity, bool IsContrast, bool IsRequired);

internal static class ExaminationItemSeeding
{
    public static IReadOnlyList<SeededExaminationItem> Build(ExaminationTypeInfo type) =>
        type.Items
            .Where(i => i.IsRequired || i.IsContrast)
            .Select(i => new SeededExaminationItem(i.ItemId, i.Quantity, i.IsContrast, i.IsRequired))
            .ToList();
}