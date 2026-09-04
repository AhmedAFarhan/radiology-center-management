using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Events;

namespace RadiologyCenter.Examinations.Application.Events;

public static class ExaminationCompletedEventHandler
{
    public static async Task HandleAsync(
        ExaminationCompletedEvent e,
        IExaminationRepository examinationRepository,
        IExaminationFeeResolver examinationFeeResolver,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetWithItemsAsync(e.ExaminationId, ct);
        if (examination is null)
            return;

        var fees = await examinationFeeResolver.ResolveAsync(
            examination.ExaminationTypeId,
            examination.TypePrice,
            examination.RadiologistId!.Value,
            examination.TechnicianId!.Value,
            examination.ReferralDoctorId,
            ct);

        examination.SetCompletionFees(fees?.RadiologistFee, fees?.TechnicianFee, fees?.ReferralFee);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
