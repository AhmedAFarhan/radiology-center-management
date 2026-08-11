using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.RecordExaminationPayment;

public static class RecordExaminationPaymentCommandHandler
{
    public static async Task<Result> HandleAsync(
        RecordExaminationPaymentCommand command,
        IExaminationRepository examinationRepository,
        IPaymentCashEntryRecorder cashEntryRecorder,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetByIdAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound("Examination", command.ExaminationId));

        if (examination.Status == ExaminationStatus.Cancelled)
            return Result.Failure(Error.Conflict("Cannot record a payment for a cancelled examination."));

        var cashResult = await cashEntryRecorder.RecordAsync(examination.Id, command.Amount, command.Description, ct);
        if (cashResult.IsFailure)
            return cashResult;

        examination.RecordPayment(command.Amount);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}