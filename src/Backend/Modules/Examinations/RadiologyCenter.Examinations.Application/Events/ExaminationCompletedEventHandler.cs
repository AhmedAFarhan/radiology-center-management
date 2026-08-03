using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Events;

namespace RadiologyCenter.Examinations.Application.Events;

public static class ExaminationCompletedEventHandler
{
    public static async Task HandleAsync(
        ExaminationCompletedEvent e,
        IExaminationRepository examinationRepository,
        IExaminationTypeRepository examinationTypeRepository,
        IItemSnapshotResolver itemSnapshotResolver,
        IExaminationFeeResolver examinationFeeResolver,
        IExaminationHistoryRepository historyRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetWithItemsAsync(e.ExaminationId, ct);
        if (examination is null)
            return;

        var type = await examinationTypeRepository.GetByIdAsync(examination.ExaminationTypeId, ct);
        if (type is null)
            return;

        var itemIds = examination.Items.Select(i => i.ItemId).Distinct().ToList();
        var itemSnapshots = await itemSnapshotResolver.ResolveAsync(itemIds, ct);

        var typeSnapshot = type.ToSnapshot();

        var fees = await examinationFeeResolver.ResolveAsync(
            examination.ExaminationTypeId,
            typeSnapshot.Price,
            examination.RadiologistId,
            examination.TechnicianId,
            examination.ReferralDoctorId,
            ct);

        var history = ExaminationHistory.Create(
            examination,
            typeSnapshot,
            itemSnapshots,
            fees?.RadiologistFee,
            fees?.TechnicianFee,
            fees?.ReferralFee);

        await historyRepository.AddAsync(history, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
