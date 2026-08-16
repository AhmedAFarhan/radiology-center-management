using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.Commands.CancelExamination;

public static class CancelExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        CancelExaminationCommand command,
        IExaminationRepository examinationRepository,
        IPaymentCashEntryRecorder cashEntryRecorder,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(ct);

        var examination = await examinationRepository.GetByIdForUpdateAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        if (examination.Paid > 0)
        {
            var refundAmount = examination.Paid;
            examination.Refund(refundAmount);

            var refundResult = await cashEntryRecorder.RecordRefundAsync(
                examination.Id,
                refundAmount,
                "Refund of payments on cancelled examination.",
                transaction,
                ct);
            if (refundResult.IsFailure)
                return refundResult;
        }

        examination.Cancel(command.Reason);

        await transaction.CommitAsync(ct);

        return Result.Success();
    }
}