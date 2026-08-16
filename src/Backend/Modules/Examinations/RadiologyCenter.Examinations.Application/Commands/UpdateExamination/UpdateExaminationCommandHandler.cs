using RadiologyCenter.Examinations.Application.Localization;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExamination;

public static class UpdateExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateExaminationCommand command,
        IExaminationRepository examinationRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var examination = await examinationRepository.GetWithItemsAsync(command.ExaminationId, ct);
        if (examination is null)
            return Result.Failure(Error.NotFound(ErrorCodes.ExaminationNotFound, "Examination", command.ExaminationId));

        var priority = ExaminationPriority.FromName<ExaminationPriority>(command.Priority);

        examination.Update(
            command.RadiologistId,
            command.TechnicianId,
            command.ClinicalIndication,
            priority,
            command.ReferralDoctorId,
            command.Notes);

        if (command.Paid.HasValue && examination.Paid > 0 && command.Paid.Value != examination.Paid)
            return Result.Failure(Error.Conflict(ErrorCodes.PaidAmountImmutable, "Paid amount cannot be modified once a payment has been recorded."));

        if (command.Discount.HasValue || command.IsDiscountPercentage.HasValue || command.Paid.HasValue)
        {
            examination.SetBilling(
                command.Discount ?? examination.Discount,
                command.IsDiscountPercentage ?? examination.IsDiscountPercentage,
                command.Paid);
        }

        if (command.Items is not null)
            ReconcileItems(examination, command.Items);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static void ReconcileItems(
        Examination examination,
        IReadOnlyList<UpdateExaminationItemRequest> requested)
    {
        foreach (var item in examination.Items.ToList())
            examination.RemoveItem(item.Id);

        foreach (var request in requested)
            examination.AddItem(
                request.ItemId,
                request.Quantity,
                request.IsContrast,
                request.IsRequired,
                request.Notes);
    }
}
