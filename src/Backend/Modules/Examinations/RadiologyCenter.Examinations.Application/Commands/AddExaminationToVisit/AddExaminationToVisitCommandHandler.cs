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
        var visit = await visitRepository.GetByIdAsync(command.VisitId, ct);
        if (visit is null)
            return Result.Failure<ExaminationDto>(Error.NotFound("Visit", command.VisitId));

        var examinationType = await examinationTypeRepository.GetByIdAsync(command.ExaminationTypeId, ct);
        if (examinationType is null)
            return Result.Failure<ExaminationDto>(Error.NotFound("ExaminationType", command.ExaminationTypeId));

        var priority = ExaminationPriority.FromName<ExaminationPriority>(command.Priority);

        var examination = visit.AddExamination(
            command.ExaminationTypeId,
            command.ReferringDoctor,
            command.ClinicalIndication,
            priority,
            command.Notes);

        foreach (var preference in examinationType.Items)
            visit.AddExaminationItem(
                examination.Id,
                preference.ItemId,
                preference.Quantity,
                preference.IsContrast,
                preference.IsRequired,
                preference.Notes);

        visitRepository.Update(visit);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(
            VisitMapper.MapExamination(examination, new Dictionary<Guid, string> { [examinationType.Id] = examinationType.Name }));
    }
}
