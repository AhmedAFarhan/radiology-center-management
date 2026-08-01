using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.Commands.AddExaminationToVisit;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Domain.Enumerations;

namespace RadiologyCenter.Examinations.Application.Commands.CreateVisit;

public static class CreateVisitCommandHandler
{
    public static async Task<Result<VisitDto>> HandleAsync(
        CreateVisitCommand command,
        IVisitRepository visitRepository,
        IExaminationTypeRepository examinationTypeRepository,
        IExaminationsUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (command.Examinations is null || command.Examinations.Count == 0)
            return Result.Failure<VisitDto>(Error.Validation("ExaminationsRequired", "A visit must include at least one examination."));

        var typeIds = command.Examinations.Select(e => e.ExaminationTypeId).Distinct().ToList();
        var types = await examinationTypeRepository.GetWithItemsByIdsAsync(typeIds, ct);
        var typesById = types.ToDictionary(t => t.Id, t => t);

        var visit = Visit.Create(
            command.PatientId,
            command.VisitedAt,
            command.AppointmentId,
            command.Notes);

        foreach (var input in command.Examinations)
        {
            if (!typesById.TryGetValue(input.ExaminationTypeId, out var type))
                return Result.Failure<VisitDto>(Error.NotFound("ExaminationType", input.ExaminationTypeId));

            if (!type.IsActive)
                return Result.Failure<VisitDto>(Error.Validation("ExaminationTypeInactive", $"Examination type '{type.Name}' is deactivated and cannot be used."));

            var priority = ExaminationPriority.FromName<ExaminationPriority>(input.Priority);

            ExaminationSeeding.Add(
                visit,
                type,
                input.ReferringDoctor,
                input.ClinicalIndication,
                priority,
                input.Notes,
                input.Items);
        }

        await visitRepository.AddAsync(visit, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(VisitMapper.Map(visit, types.ToDictionary(t => t.Id, t => t.Name)));
    }
}
