using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.UpdateExamination;

public static class UpdateExaminationCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateExaminationCommand command,
        IVisitRepository visitRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var visit = await visitRepository.GetWithExaminationsAsync(command.VisitId, ct);
        if (visit is null)
            return Result.Failure(Error.NotFound("Visit", command.VisitId));

        var examination = visit.Examinations.FirstOrDefault(e => e.Id == command.ExaminationId);
        if (examination is null)
            return Result.Failure(Error.NotFound("Examination", command.ExaminationId));

        var priority = ExaminationPriority.FromName<ExaminationPriority>(command.Priority);

        visit.UpdateExamination(
            command.ExaminationId,
            command.ReferringDoctor,
            command.ClinicalIndication,
            priority,
            command.Notes);

        if (command.Items is not null)
        {
            var reconcileResult = ReconcileItems(visit, examination, command.Items);
            if (reconcileResult is not null)
                return reconcileResult;
        }

        visitRepository.Update(visit);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static Result? ReconcileItems(
        Visit visit,
        Examination examination,
        IReadOnlyList<UpdateExaminationItemRequest> requested)
    {
        var currentItems = examination.Items.ToList();
        var requestedIds = requested
            .Where(i => i.ExaminationItemId is not null)
            .Select(i => i.ExaminationItemId!.Value)
            .ToHashSet();

        foreach (var item in currentItems.Where(i => !requestedIds.Contains(i.Id)))
            visit.RemoveExaminationItem(examination.Id, item.Id);

        foreach (var request in requested)
        {
            if (request.ExaminationItemId is not null)
            {
                if (currentItems.All(i => i.Id != request.ExaminationItemId.Value))
                    return Result.Failure(Error.Validation("ExaminationItemNotFound", $"Examination item '{request.ExaminationItemId.Value}' is not on examination '{examination.Id}'."));

                visit.UpdateExaminationItem(
                    examination.Id,
                    request.ExaminationItemId.Value,
                    request.ItemId,
                    request.Quantity,
                    request.IsContrast,
                    request.IsRequired,
                    request.Notes);
            }
            else
            {
                visit.AddExaminationItem(
                    examination.Id,
                    request.ItemId,
                    request.Quantity,
                    request.IsContrast,
                    request.IsRequired,
                    request.Notes);
            }
        }

        return null;
    }
}
