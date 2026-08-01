using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.AddExaminationToVisit;

public static class AddExaminationToVisitCommandHandler
{
    public static async Task<Result<ExaminationDto>> HandleAsync(
        AddExaminationToVisitCommand command,
        IVisitRepository visitRepository,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        var visit = await visitRepository.GetWithExaminationsAsync(command.VisitId, ct);
        if (visit is null)
            return Result.Failure<ExaminationDto>(Error.NotFound("Visit", command.VisitId));

        var examinationType = await examinationTypeRepository.GetWithItemsAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure<ExaminationDto>(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        if (!examinationType.IsActive)
            return Result.Failure<ExaminationDto>(Error.Validation("ExaminationTypeInactive", $"Examination type '{examinationType.Name}' is deactivated and cannot be used."));

        var priority = ExaminationPriority.FromName<ExaminationPriority>(command.Priority);

        var examination = ExaminationSeeding.Add(
            visit,
            examinationType,
            command.ReferringDoctor,
            command.ClinicalIndication,
            priority,
            command.Notes,
            command.Items);

        visitRepository.Update(visit);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(
            VisitMapper.MapExamination(examination, new Dictionary<Guid, string> { [examinationType.Id] = examinationType.Name }));
    }
}
