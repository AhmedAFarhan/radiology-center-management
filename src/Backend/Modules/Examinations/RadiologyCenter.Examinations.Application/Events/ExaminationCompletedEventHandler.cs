using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Entities;
using RadiologyCenter.Examinations.Domain.Events;

namespace RadiologyCenter.Examinations.Application.Events;

public static class ExaminationCompletedEventHandler
{
    public static async Task HandleAsync(
        ExaminationCompletedEvent e,
        IVisitRepository visitRepository,
        IExaminationTypeRepository examinationTypeRepository,
        IItemSnapshotResolver itemSnapshotResolver,
        IExaminationHistoryRepository historyRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var visit = await visitRepository.GetWithExaminationsAsync(e.VisitId, ct);
        var examination = visit?.Examinations.FirstOrDefault(x => x.Id == e.ExaminationId);
        if (examination is null)
            return;

        var type = await examinationTypeRepository.GetByIdAsync(examination.ExaminationTypeId, ct);
        if (type is null)
            return;

        var itemIds = examination.Items.Select(i => i.ItemId).Distinct().ToList();
        var itemSnapshots = await itemSnapshotResolver.ResolveAsync(itemIds, ct);

        var history = ExaminationHistory.Create(examination, type.ToSnapshot(), itemSnapshots);
        await historyRepository.AddAsync(history, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
