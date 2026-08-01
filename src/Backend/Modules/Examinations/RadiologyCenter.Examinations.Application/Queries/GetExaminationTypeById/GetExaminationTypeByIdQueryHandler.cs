using Mapster;
using RadiologyCenter.Examinations.Application.Abstractions;
using RadiologyCenter.Examinations.Application.DTOs;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationTypeById;

public static class GetExaminationTypeByIdQueryHandler
{
    public static async Task<Result<ExaminationTypeDto>> HandleAsync(
        GetExaminationTypeByIdQuery query,
        IExaminationTypeRepository examinationTypeRepository,
        CancellationToken ct)
    {
        var examinationType = await examinationTypeRepository.GetWithItemsAsync(query.Id, ct);
        if (examinationType is null)
            return Result.Failure<ExaminationTypeDto>(Error.NotFound("ExaminationType", query.Id));

        return Result.Success(examinationType.Adapt<ExaminationTypeDto>());
    }
}
