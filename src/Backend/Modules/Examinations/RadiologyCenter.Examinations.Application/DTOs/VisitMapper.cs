using RadiologyCenter.BuildingBlocks.Domain.Specifications;
using RadiologyCenter.Examinations.Application.Abstractions;

namespace RadiologyCenter.Examinations.Application.DTOs;

public static class VisitMapper
{
    public static VisitDto Map(Visit visit, IReadOnlyDictionary<Guid, string> examinationTypeNames) =>
        new(
            visit.Id,
            visit.PatientId,
            visit.AppointmentId,
            visit.VisitedAt,
            visit.Status.Name,
            visit.Notes,
            visit.Examinations
                .Select(e => MapExamination(e, examinationTypeNames))
                .ToList());

    public static ExaminationDto MapExamination(
        Examination examination,
        IReadOnlyDictionary<Guid, string> examinationTypeNames) =>
        new(
            examination.Id,
            examination.VisitId,
            examination.ExaminationTypeId,
            examinationTypeNames.TryGetValue(examination.ExaminationTypeId, out var name) ? name : string.Empty,
            examination.ReferringDoctor,
            examination.ClinicalIndication,
            examination.Priority.Name,
            examination.Status.Name,
            examination.ScheduledAt,
            examination.StartedAt,
            examination.CompletedAt,
            examination.PerformedByUserId,
            examination.Notes,
            examination.CancellationReason,
            examination.Items
                .Select(i => new ExaminationItemDto(
                    i.Id,
                    i.ItemId,
                    i.Quantity,
                    i.IsContrast,
                    i.IsRequired,
                    i.Notes))
                .ToList());

    public static async Task<IReadOnlyDictionary<Guid, string>> LoadExaminationTypeNamesAsync(
        IEnumerable<Guid> examinationTypeIds,
        IExaminationTypeRepository examinationTypeRepository,
        CancellationToken ct)
    {
        var ids = examinationTypeIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<Guid, string>();

        var spec = new DynamicSpecification<ExaminationType>();
        spec.AddCriteria(t => ids.Contains(t.Id));
        var types = await examinationTypeRepository.FindAsync(spec, ct);
        return types.ToDictionary(t => t.Id, t => t.Name);
    }
}
