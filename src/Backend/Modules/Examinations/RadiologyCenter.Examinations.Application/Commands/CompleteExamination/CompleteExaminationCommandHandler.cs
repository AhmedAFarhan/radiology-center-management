using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.CompleteExamination;

public static class CompleteExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        CompleteExaminationCommand command,
        IExaminationRepository examinationRepository,
        IExaminationFeeResolver examinationFeeResolver,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        if (examination.RadiologistId is null || examination.TechnicianId is null)
            return Result.Failure(Error.Conflict(
                ErrorCodes.StaffNotAssigned,
                "A radiologist and a technician must be assigned before the examination can be completed."));

        if (examination.EquipmentId is null)
            return Result.Failure(Error.Conflict(
                ErrorCodes.EquipmentNotAssigned,
                "An equipment/device must be assigned before the examination can be completed."));

        // Fee snapshots belong to the completion transaction, not a background process.
        var fees = await examinationFeeResolver.ResolveAsync(examination.ExaminationTypeId,
            examination.TypePrice, examination.RadiologistId.Value, examination.TechnicianId.Value,
            examination.ReferralDoctorId, ct);
        examination.Complete();
        examination.SetCompletionFees(fees?.RadiologistFee, fees?.TechnicianFee, fees?.ReferralFee);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
