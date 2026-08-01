using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetVisitById;

public static class GetVisitByIdQueryHandler
{
    public static async Task<Result<VisitDto>> HandleAsync(
        GetVisitByIdQuery query,
        IVisitRepository visitRepository,
        IExaminationTypeRepository examinationTypeRepository,
        CancellationToken ct)
    {
        var visit = await visitRepository.GetWithExaminationsAsync(query.Id, ct);
        if (visit is null)
            return Result.Failure<VisitDto>(Error.NotFound("Visit", query.Id));

        var examinationTypeIds = visit.Examinations.Select(e => e.ExaminationTypeId).ToList();
        var typeNames = await VisitMapper.LoadExaminationTypeNamesAsync(examinationTypeIds, examinationTypeRepository, ct);

        return Result.Success(VisitMapper.Map(visit, typeNames));
    }
}
