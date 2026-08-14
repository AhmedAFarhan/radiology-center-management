using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.RecordExaminationPayment;

public static class RecordExaminationPaymentCommandHandler
{
    public static async Task<Result> HandleAsync(
        RecordExaminationPaymentCommand command,
        IExaminationRepository examinationRepository,
        IExaminationHistoryRepository historyRepository,
        IPaymentCashEntryRecorder cashEntryRecorder,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        var examination = await examinationRepository.GetByIdForUpdateAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound("Examination", command.ExaminationId));

        if (examination.Status == ExaminationStatus.Cancelled)
            return Result.Failure(Error.Conflict("Cannot record a payment for a cancelled examination."));

        if (command.Amount > examination.Remaining)
            return Result.Failure(
                Error.Validation(
                    "PaymentExceedsRemaining",
                    $"Payment of '{command.Amount}' exceeds the remaining balance of '{examination.Remaining}'."));

        examination.RecordPayment(command.Amount);

        if (examination.Status == ExaminationStatus.Completed)
        {
            var history = await historyRepository.GetByExaminationIdAsync(examination.Id, ct);
            if (history is not null)
                history.UpdatePaymentSnapshot(examination.Paid, examination.Remaining);
        }

        var cashResult = await cashEntryRecorder.RecordAsync(
            examination.Id,
            command.Amount,
            command.Description,
            transaction,
            ct);
        if (cashResult.IsFailure)
            return cashResult;

        await transaction.CommitAsync(ct);

        return Result.Success();
    }
}